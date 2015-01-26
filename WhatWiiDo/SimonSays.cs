using IrrKlang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo 
{
    class SimonGame : Minigame
    {
        Array simonValues = Enum.GetValues(typeof(wiiButton));
        Random random = new Random();
        List<wiiButton> commandList;
        Dictionary<Guid, SimonPlayer> simonPlayers;
        static String simonSoundDirectory = "../../sounds/simon/";
        SimonGameState gamestate, pause_queuedGamestate;
        ISoundEngine soundEngine;
        ISound currentlySpokenCommand, playingTone;
        int currentLevel, targetLevel, speakIndex, pause_time, pause_elapsedTime;
        wiiButton nextButton;
        PausedInfo pauseInfo;

        public SimonGame(Dictionary<Guid, Wiimote> players)
        {
            targetLevel = 3;
            commandList = new List<wiiButton>(targetLevel);
            soundEngine = new ISoundEngine();
            gamestate = SimonGameState.SELECT_NEW_COMMAND;

            simonPlayers = new Dictionary<Guid, SimonPlayer>();
            foreach (Guid id in players.Keys)
            {
                simonPlayers.Add(id, new SimonPlayer());
            }
            SimonPlayer.initCommandList(targetLevel);
        }

        public void update(Dictionary<Guid, Wiimote> players, int deltaTime)
        {
             /**
             * Game Flow:
             * START OF ROUND : 
             * COMPUTER TURN: Pick a new command, add it to the list
             * COMPUTER SPEAK: Say all of the current commands
             * -Turn on input-
             * PLAYER TURN: Each player has to enter the sequence on their own, if any fuck up, restart game
             * -turn off input-
             * END OF ROUND if all are good, go to next round
             * 
             */
            switch (gamestate) {
                case SimonGameState.SELECT_NEW_COMMAND:
                    System.Console.WriteLine("SELECT_NEW_COMMAND");
                    nextButton = (wiiButton)simonValues.GetValue(random.Next(simonValues.Length));
                    commandList.Add(nextButton);
                    gamestate = SimonGameState.SPEAK_COMMAND;
                    speakIndex = 0;
                    break;
                case SimonGameState.SPEAK_COMMAND:
                    System.Console.WriteLine("SPEAK_COMMAND");
                    if (speakIndex < commandList.Count){
                        playingTone = soundEngine.Play2D(CommonSounds.buttonSounds[commandList[speakIndex]]);
                        gamestate = SimonGameState.WAIT_FOR_TONE;
                        pauseInfo = new AndPause(new TimedPause(200, null),
                                                 new SoundPause(playingTone, null),
                                                 SimonGameState.SPEAK_COMMAND);
                    } else {
                        gamestate = SimonGameState.PLAYER_TURN_BEGIN;
                    }
                    speakIndex++;
                    break;
                case SimonGameState.PLAYER_TURN_BEGIN:
                    System.Console.WriteLine("PLAYER_TURN_BEGIN");
                     SimonPlayer.addNewCommandToList(nextButton);
                     foreach(Guid id in players.Keys){
                         simonPlayers[id].reset();
                     }
                     gamestate = SimonGameState.WAIT_FOR_TONE;
                     playingTone = soundEngine.Play2D(CommonSounds.Go);
                     pauseInfo = new SoundPause(playingTone,
                                                SimonGameState.PLAYER_TURN);
                     break;
                case SimonGameState.PLAYER_TURN:
                    System.Console.WriteLine("PLAYER_TURN");
                    bool done = true;
                    foreach(Guid id in players.Keys){
                        if (!simonPlayers[id].isFinished()){
                            bool playerStatus = simonPlayers[id].update(players[id], deltaTime, soundEngine);
                            if (!playerStatus){
                                gamestate = SimonGameState.PLAYER_FAILURE;
                                done = false;
                                break;
                            }
                        }
                        done = simonPlayers[id].isFinished() && done;
                    }
                    if(done){
                        gamestate = SimonGameState.PLAYER_SUCCESS;
                    }
                    break;
                case SimonGameState.PLAYER_FAILURE:
                    System.Console.WriteLine("PLAYER_FAILURE");
                    playingTone = soundEngine.Play2D(simonSoundDirectory + "simon_fail.wav");
                    currentLevel = 0;
                    commandList = new List<wiiButton>(targetLevel);
                    SimonPlayer.initCommandList(targetLevel);
                    foreach(SimonPlayer s in simonPlayers.Values){
                        s.reset();
                    }
                    gamestate = SimonGameState.WAIT_FOR_TONE;
                    pauseInfo = new SoundAndTimePause(playingTone, 500, SimonGameState.SELECT_NEW_COMMAND);
                    break;
                case SimonGameState.PLAYER_SUCCESS:
                    System.Console.WriteLine("PLAYER_SUCCESS");
                    playingTone = soundEngine.Play2D(CommonSounds.Yes);
                    gamestate = SimonGameState.WAIT_FOR_TONE;
                    SimonGameState nextState;
                    if(currentLevel == targetLevel){
                        nextState = SimonGameState.VICTORY;
                    } else {
                        nextState = SimonGameState.SELECT_NEW_COMMAND;
                        currentLevel++;
                    }
                    pauseInfo = new AndPause(new VibrateDuringPause(players.Values,
                                                                    new TimedPause(400, null)),
                                             new SoundPause(playingTone, null),
                                             nextState);
                    break;
                case SimonGameState.VICTORY:
                    System.Console.WriteLine("Veectoriii");
                    break;
                case SimonGameState.WAIT_FOR_TONE:
                    if (pauseInfo.update(deltaTime))
                    {
                        gamestate = (SimonGameState)pauseInfo.nextState();
                    }
                    break;

            }
            
        }

        public bool isOver()
        {
            return gamestate == SimonGameState.VICTORY;
        }


        enum SimonGameState { SELECT_NEW_COMMAND, SPEAK_COMMAND, PLAYER_TURN_BEGIN, PLAYER_TURN, PLAYER_SUCCESS, PLAYER_FAILURE, WAIT_FOR_TONE, VICTORY};
        class SimonPlayer
        {
            private static List<wiiButton> commandList;
            private static int commandSize;
            private int wait_targetMilli = 300;
            private SimonPlayerState playerstate, wait_nextPlayerState;
            private int commandIndex, wait_elapsedMilli;
            private ISound playingSound;
            buttonHandler buttons;
            private PausedInfo pausedInfo;

            public static void initCommandList(int listSize){
                commandList = new List<wiiButton>(listSize);
                commandSize = listSize;
            }
            
            public SimonPlayer()
            {
                commandIndex = 0;
                buttons = new buttonHandler();
                playerstate = SimonPlayerState.WAITING_FOR_INPUT;
            }

            public static void addNewCommandToList(wiiButton command)
            {
                SimonPlayer.commandList.Add(command);
            }

            public void reset()
            {
                playerstate = SimonPlayerState.WAITING_FOR_INPUT;
                commandIndex = 0;
            }
            
            public bool isFinished(){
                return playerstate == SimonPlayerState.SUCCESS;
            }

            internal bool update(Wiimote wiimote, int deltaTime, ISoundEngine soundEngine)
            {
                List<List<wiiButton>> buttonList = buttons.update(wiimote);
                switch (playerstate)
                {
                    case SimonPlayerState.WAITING_FOR_INPUT:
                        if (commandIndex >= SimonPlayer.commandList.Count)
                        {
                            playerstate = SimonPlayerState.PLAYING_INPUT_SOUND;
                            playingSound = soundEngine.Play2D(simonSoundDirectory + "seq_correct.wav");
                            pausedInfo = 
                                new VibrateDuringPause(wiimote,
                                                       new SoundPause(playingSound,
                                                                      SimonPlayerState.SUCCESS));
                        }
                        else if (buttonList[0].Contains(SimonPlayer.commandList[commandIndex]))
                        {
                            commandIndex++;
                            playingSound = soundEngine.Play2D(simonSoundDirectory + "button_input.wav");
                            playerstate = SimonPlayerState.PLAYING_INPUT_SOUND;
                            pausedInfo =
                                new VibrateDuringPause(wiimote,
                                                       new SoundPause(playingSound,
                                                                      SimonPlayerState.WAITING_FOR_INPUT));
                        } else if (buttonList[0].Count != 0)
                        {
                            return false;
                        }
                        break;
                    case SimonPlayerState.PLAYING_INPUT_SOUND:
                        if (pausedInfo.update(deltaTime))
                        {
                            playerstate = (SimonPlayerState)pausedInfo.nextState();
                        }
                        break;
                }
                return true;
            }

            private enum SimonPlayerState { SUCCESS, PLAYING_INPUT_SOUND, WAITING_FOR_INPUT}
        }
    }
}
