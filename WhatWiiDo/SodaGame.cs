using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using System.Media;

namespace WhatWiiDo
{
    public class SodaGame : Minigame
    {
        static int shakesNeeded = 10;
        int shakes = 0;
        bool lastShakeUp = true;
        bool bPressed = false;

        SoundPlayer shakeSound;
        SoundPlayer openSound;
        SoundPlayer fizzSound;

        public SodaGame()
        {
            shakeSound = new SoundPlayer(Properties.Resources.soda_shake);
            openSound = new SoundPlayer(Properties.Resources.soda_open_full);
            fizzSound = new SoundPlayer(Properties.Resources.soda_fizz_short);
        }

        public void update(Dictionary<Guid, Wiimote> players)
        {
            foreach (Wiimote mote in players.Values)
            {
                if (mote.WiimoteState.AccelState.Values.Y < -2 && !lastShakeUp)
                {
                    shakes++;
                    shakeSound.Play();
                    lastShakeUp = true;
                }
                if (mote.WiimoteState.AccelState.Values.Y > 2 && lastShakeUp)
                {
                    lastShakeUp = false;
                }

                if (mote.WiimoteState.ButtonState.B && !bPressed)
                {
                    bPressed = true;
                    openSound.Play();
                    if (shakes > shakesNeeded)
                    {
                        fizzSound.Play();
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
            if (shakes > shakesNeeded)
            {
                Console.WriteLine("ready!");
                return false;
            }
            return false;
        }
    }
}
