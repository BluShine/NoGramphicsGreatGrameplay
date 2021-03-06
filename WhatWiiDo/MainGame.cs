﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;
using System.Media;
using IrrKlang;

namespace WhatWiiDo
{
    class MainGame
    {
        Dictionary<Guid, Wiimote> players;
        WiimoteCollection wiimoteCollection;
        static float FPS = 100;

        Minigame currentGame;
        private List<Minigame> gameList;
        ISoundEngine soundEngine;
        int elapsedMilis;

        bool gameRunning = false;

        public MainGame()
        {
            Console.WriteLine("Connect wii remotes and press enter when ready.");
            Console.WriteLine("Make sure that your bluetooth devices shows the right number of controllers connected.");
            Console.WriteLine("They should be called RVL-CNT-01. You may have to manually 'remove' controllers that have become disconnected.");
            Console.ReadLine();

            Load();

            Console.WriteLine("You have " + players.Count + " wii remotes connected. Press enter to start.");
            Console.ReadLine();

            while (true)
            {
                if (gameRunning)
                {
                    run();
                }
                else
                {
                    initGames();
                }
            }
        }

        private void initGames()
        {
            gameList = new List<Minigame>();
            //gameList.Add(new SodaGame(players));
            //gameList.Add(new PingPongGame(players));
            //gameList.Add(new Maze(players));
            gameList.Add(new SimonGame(players));

            elapsedMilis = 1;

            currentGame = gameList[0];

            soundEngine = new ISoundEngine();
            gameRunning = true;
            Console.Clear();
        }

        private void run()
        {
            DateTime last = DateTime.Now;

            currentGame.update(players, elapsedMilis);
            if (currentGame.isOver())
            {
                gameList.Remove(currentGame);
                if (gameList.Count > 0)
                {
                    soundEngine.Play2D("../../sounds/win.wav");
                    currentGame = gameList[0];
                }
                else
                {
                    soundEngine.Play2D("../../sounds/win.wav");
                    gameRunning = false;
                }
            }

            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                Console.WriteLine("RESET!");
                gameRunning = false;
            }

            TimeSpan elapsed = DateTime.Now - last;
            elapsedMilis = elapsed.Milliseconds;
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
