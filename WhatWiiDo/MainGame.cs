﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using System.Media;

namespace WhatWiiDo
{
    class MainGame
    {
        Dictionary<Guid, Wiimote> players;
        WiimoteCollection wiimoteCollection;
        Boolean running = true;
        static float FPS = 100;

        Minigame currentGame;

        public MainGame()
        {
            Load();

            List<Minigame> gameList = new List<Minigame>();
            //gameList.Add(new SodaGame(players));
            gameList.Add(new Maze(players));

            currentGame = gameList[0];

            int elapsedMilis = 1;

            while (running)
            {
                DateTime last = DateTime.Now;

                currentGame.update(players, elapsedMilis);
                if (currentGame.isOver())
                {
                    gameList.Remove(currentGame);
                    if (gameList.Count > 0)
                    {
                        currentGame = gameList[0];
                    }
                    else
                    {
                        running = false;
                    }
                }

                TimeSpan elapsed = DateTime.Now - last;
                elapsedMilis = elapsed.Milliseconds;
            }

            Console.ReadLine();
        }

        private void Load()
        {
            wiimoteCollection = new WiimoteCollection();
            players = new Dictionary<Guid, Wiimote>();
            int index = 1;

            try
            {
                wiimoteCollection.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                Console.WriteLine("WiiMote not found" + ex.Message);
            }
            catch(WiimoteException ex)
			{
                Console.WriteLine("Wiimote error" + ex.Message);
            }
			catch(Exception ex)
			{
                Console.WriteLine("Unknown error" + ex.Message);
            }

            foreach(Wiimote mote in wiimoteCollection) {

                //set callback function
                mote.WiimoteChanged += moteChanged;

                //connect it
                mote.Connect();
                if(mote.WiimoteState.ExtensionType == ExtensionType.BalanceBoard) {
                    Console.WriteLine("Why would you connect a balance board!?!?");
                } 
                else {
                    mote.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                }

                //add it to our dictionary
                players.Add(mote.ID, mote);

                mote.SetLEDs(index++);
            }
        }

        void moteChanged(object sender, WiimoteChangedEventArgs args)
        {
            Wiimote mote = (Wiimote) sender;
            //Console.WriteLine(mote.ID + " did a thing.");
        }
    }
}
