using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    public class SodaGame : Minigame
    {
        static int shakesNeeded = 50;
        int shakes = 0;
        bool lastShakeUp = true;

        public SodaGame()
        {
        }

        public void update(Dictionary<Guid, Wiimote> players)
        {
            foreach (Wiimote mote in players.Values)
            {
                if (mote.WiimoteState.AccelState.Values.Y < -2 && !lastShakeUp)
                {
                    Console.WriteLine();
                    int i = shakes;
                    while (i > 0)
                    {
                        Console.Write(" ");
                        i--;
                    }
                    Console.Write("fap");
                    lastShakeUp = true;
                    shakes++;
                }
                if (mote.WiimoteState.AccelState.Values.Y > 2 && lastShakeUp)
                {
                    lastShakeUp = false;
                }
            }
        }

        public bool isOver()
        {
            if (shakes > shakesNeeded)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("you win!");
                return true;
            }
            return false;
        }
    }
}
