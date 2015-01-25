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
        static int shakesNeeded = 20;
        int shakes = 0;
        bool lastShakeUp = true;
        bool bPressed = false;

        ISoundEngine soundEngine;

        public SodaGame()
        {
            soundEngine = new ISoundEngine();
        }

        public void update(Dictionary<Guid, Wiimote> players, int deltaTime)
        {
            foreach (Wiimote mote in players.Values)
            {
                if (mote.WiimoteState.AccelState.Values.Y < -2 && !lastShakeUp)
                {
                    shakes++;
                    float shakeSpeed = 1f + 2f * (((float) Math.Min(shakes, shakesNeeded)) / ((float) shakesNeeded));
                    soundEngine.Play2D("../../sounds/soda/soda_shake_2.wav").PlaybackSpeed = shakeSpeed;
                    lastShakeUp = true;
                }
                if (mote.WiimoteState.AccelState.Values.Y > 2 && lastShakeUp)
                {
                    lastShakeUp = false;
                }

                if (mote.WiimoteState.ButtonState.B && !bPressed)
                {
                    bPressed = true;
                    if (shakes > shakesNeeded)
                    {
                        soundEngine.Play2D("../../sounds/soda/soda_fizz_short.wav");
                        soundEngine.Play2D("../../sounds/soda/soda_open_hiss_only.wav");
                    }
                    else
                    {
                        soundEngine.Play2D("../../sounds/soda/soda_open_full.wav");
                    }
                    shakes = 0;
                }
                if (!mote.WiimoteState.ButtonState.B)
                {
                    bPressed = false;
                }
            }
        }

        public bool isOver()
        {
            return false;
        }
    }
}
