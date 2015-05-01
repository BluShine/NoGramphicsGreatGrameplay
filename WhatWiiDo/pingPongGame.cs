using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using IrrKlang;

namespace WhatWiiDo
{
    class PingPongGame : Minigame
    {
        Dictionary<Guid, PongPlayer> pongPlayers;
        ISoundEngine soundEngine;
        List<Guid> playerOrder;
        int currentPlayer;

        int ballTimer = 0;

        static int PINGTIME = 500;
        static int PONGTIME = 300;
        static int GRACETIME = 200;
        static int MISSRUMBLE = 100;

        static int VOLLEYNEEDEDPERPLAYER = 3;

        int volleys = 0;
        PongState state = PongState.start;

        Random random;

        private enum PongState
        {
            start, ping, pong, miss
        }

        public PingPongGame(Dictionary<Guid, Wiimote> players)
        {
            soundEngine = new ISoundEngine();
            random = new Random();
            pongPlayers = new Dictionary<Guid,PongPlayer>();
            playerOrder = new List<Guid>();

            foreach(Guid id in players.Keys) {
                pongPlayers.Add(id, new PongPlayer());
                playerOrder.Insert(random.Next(playerOrder.Count), id);
            }
        }

        public void update(Dictionary<Guid, Wiimote> players, int deltaTime)
        {

            bool serve = false;
            bool hit = false;

            if (state == PongState.start)
            {
                serve = true;
            }

            if(state == PongState.miss) {
                ballTimer -= deltaTime;
                if(ballTimer <= 0) {
                    ballTimer = 0;
                    players[playerOrder[currentPlayer]].SetRumble(false);
                    currentPlayer = 0;
                    state = PongState.start;
                }
            }

            if (state == PongState.pong)
            {
                ballTimer -= deltaTime;
                if (ballTimer < GRACETIME)
                {
                    hit = true;
                }
                if (ballTimer <= 0)
                {
                    volleys = 0;
                    state = PongState.miss;
                    ballTimer = MISSRUMBLE;
                    soundEngine.Play2D("../../sounds/pingPong/pong_whoosh_miss.wav");
                    players[playerOrder[currentPlayer]].SetRumble(true);
                }
            }

            if (state == PongState.ping)
            {
                ballTimer -= deltaTime;
                if (ballTimer <= 0)
                {
                    state = PongState.pong;
                    ballTimer = PONGTIME;
                    soundEngine.Play2D("../../sounds/pingPong/pong_bounce.wav");
                }
            }

            foreach (Guid id in players.Keys)
            {
                if (id == playerOrder[currentPlayer])
                {
                    if(pongPlayers[id].update(players[id], deltaTime, soundEngine, hit, serve)) {
                        Console.WriteLine("hit");
                        state = PongState.ping;
                        ballTimer = PINGTIME;
                        currentPlayer++;
                        currentPlayer = currentPlayer % playerOrder.Count;
                        if (currentPlayer == 0)
                        {
                            Console.WriteLine("volley!");
                            volleys++;
                        }
                    }
                }
                else
                {
                    pongPlayers[id].update(players[id], deltaTime, soundEngine, false, false);
                }
            }
        }

        public bool isOver()
        {
            return volleys >= VOLLEYNEEDEDPERPLAYER;
        }

        public class PongPlayer
        {
            buttonHandler buttons;

            static int SWINGCOOLDOWN = 300;
            int swingTimer = 0;

            public PongPlayer()
            {
                buttons = new buttonHandler();
            }

            public bool update(Wiimote mote, int deltaTime, ISoundEngine soundEngine, bool hitItNow, bool serveNow) {
                List<List<wiiButton>> buttonUpdates = buttons.update(mote);

                swingTimer -= deltaTime;
                swingTimer = Math.Max(0, swingTimer);

                if (serveNow && buttonUpdates[0].Contains(wiiButton.A) || buttonUpdates[0].Contains(wiiButton.B))
                {
                    soundEngine.Play2D("../../sounds/pingPong/pong_hit_1.wav");
                    return true;
                }

                if (swingTimer == 0)
                {
                    if (Math.Abs(mote.WiimoteState.AccelState.Values.X) +
                        Math.Abs(mote.WiimoteState.AccelState.Values.Y) +
                        Math.Abs(mote.WiimoteState.AccelState.Values.Z) > 6)
                    {
                        swingTimer = SWINGCOOLDOWN;
                        if (hitItNow)
                        {
                            soundEngine.Play2D("../../sounds/pingPong/pong_hit_2.wav");
                            return true;
                        }
                        else
                        {
                            soundEngine.Play2D("../../sounds/pingPong/pong_whoosh.wav");
                        }
                    }
                }
                return false;
            }
        }
    }
}
