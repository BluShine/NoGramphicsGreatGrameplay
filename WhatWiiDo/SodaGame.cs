using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using System.Media;
using IrrKlang;

namespace WhatWiiDo
{
    public class SodaGame : Minigame
    {

        Dictionary<Guid, sodaCan> sodas;

        ISoundEngine soundEngine;

        public SodaGame(Dictionary<Guid, Wiimote> players)
        {
            soundEngine = new ISoundEngine();

            sodas = new Dictionary<Guid, sodaCan>();
            foreach (Guid id in players.Keys)
            {
                sodas.Add(id, new sodaCan());
            }
        }

        public void update(Dictionary<Guid, Wiimote> players, int deltaTime)
        {
            foreach (Guid id in players.Keys)
            {
                sodas[id].update(players[id], deltaTime, soundEngine);
            }
        }

        public bool isOver()
        {
            bool allDone = true;
            foreach (sodaCan s in sodas.Values)
            {
                if (s.finishedMilis > 0)
                {
                    allDone = false;
                }
            }
            return allDone;
        }

        private class sodaCan
        {
            Random random;
            buttonHandler buttons;
            public int shakes = 0;
            public int finishedMilis = 4000;
            bool lastShakeUp = false;

            static int shakesNeeded = 20;
            static String[] canSounds = { 
                                            "../../sounds/soda/soda_hit_1.wav",
                                            "../../sounds/soda/soda_hit_2.wav",
                                            "../../sounds/soda/soda_hit_3.wav",
                                            "../../sounds/soda/soda_hit_4.wav",
                                            "../../sounds/soda/soda_tab_1.wav",
                                            "../../sounds/soda/soda_tab_2.wav"};

            public sodaCan()
            {
                buttons = new buttonHandler();
                random = new Random();
            }

            public void update(Wiimote mote, int deltaTime, ISoundEngine soundEngine)
            {
                if (shakes == -1)
                {
                    finishedMilis -= deltaTime;
                }
                else
                {
                    List<List<wiiButton>> buttonList = buttons.update(mote);

                    if (mote.WiimoteState.AccelState.Values.Y < -2 && !lastShakeUp)
                    {
                        shakes++;
                        float shakeSpeed = 1f + 2f * (((float)Math.Min(shakes, shakesNeeded)) / ((float)shakesNeeded));
                        soundEngine.Play2D("../../sounds/soda/soda_shake_2.wav").PlaybackSpeed = shakeSpeed;
                        lastShakeUp = true;
                    }
                    if (mote.WiimoteState.AccelState.Values.Y > 2 && lastShakeUp)
                    {
                        lastShakeUp = false;
                    }

                    if (buttonList[0].Contains(wiiButton.B))
                    {
                        buttonList[0].Remove(wiiButton.B);
                        if (shakes > shakesNeeded)
                        {
                            soundEngine.Play2D("../../sounds/soda/soda_fizz_short.wav");
                            soundEngine.Play2D("../../sounds/soda/soda_open_hiss_only.wav");
                            shakes = -1;
                        }
                        else
                        {
                            soundEngine.Play2D("../../sounds/soda/soda_open_full.wav");
                            shakes = 0;
                        }
                    }

                    if (buttonList[0].Count > 0)
                    {
                        soundEngine.Play2D(canSounds[random.Next(canSounds.Length)]);
                    }
                }
            }
        }
    }
}
