using System;
using System.Collections.Generic;
using WhatWiiDo;
using WiimoteLib;
using System.Media;
using IrrKlang;


public class Maze : Minigame
{
	int numPlayers;
	int mazeSize;
	MazeRoom[,] rooms;
    Dictionary<Guid, Player> players;
    bool winFlag;

    ISoundEngine soundEngine;
    SoundManager claps;

    static float[] spawnLocations = { 0, 0, 1, 1, 0, 1, 1, 0, .5f, .5f, 0, .5f, .5f, 0, 1, .5f, .5f, 1 };

    public Maze(Dictionary<Guid, Wiimote> input)
	{
		numPlayers = input.Count;
		mazeSize = numPlayers + 2;
		rooms = new MazeRoom[mazeSize, mazeSize];
        players = new Dictionary<Guid, Player>(numPlayers);
        winFlag = false;
        soundEngine = new ISoundEngine();
        claps = new SoundManager();

        int i = 0;
        foreach(Guid ident in input.Keys)
        {
            players.Add(ident, new Player(ident, (int)(spawnLocations[i]*(mazeSize-1)), (int)(spawnLocations[i+1]*(mazeSize-1))));
            i += 2;
        }
        buildMaze();
        printMaze();
	}

	void buildMaze() {

        for(int i = 0; i < rooms.GetLength(0); i++)
        {
            for(int j = 0; j < rooms.GetLength(1); j++)
            {
                rooms[i,j] = new MazeRoom();
            }
        }

		int x = 0;
		int y = 0;
		Stack<Point> path = new Stack<Point>();
        Random gen = new Random();

		while(true)
		{
			rooms[x,y].visit();
            Console.WriteLine(x + ", " + y);
			if(surrounded(x, y))
			{
				if(path.Count == 0)
				{
					break;
				}
				else
				{
					Point bt = path.Pop();
                    x = bt.X;
					y = bt.Y;
					continue;
				}
			}
			int dir = gen.Next(4);
			while(blockedBuildDir(x, y, dir))
			{
				dir = gen.Next(4);
			}
            Point here = new Point();
            here.X = x;
            here.Y = y;
			path.Push(here);
			switch (dir) {
			case 0:
				rooms[x,y].breakRightWall();
				x += 1;
				break;
			case 1:
				rooms[x,y].breakBotWall();
				y += 1;
				break;
			case 2:
				rooms[x-1,y].breakRightWall();
				x -= 1;
				break;
			case 3:
				rooms[x,y-1].breakBotWall();
				y -= 1;
				break;
			}
		}

	}

	bool surrounded(int x, int y)
	{
		bool left = (x-1) < 0 || rooms[x-1,y].beenVisited();
		bool up = (y-1) < 0 || rooms[x,y-1].beenVisited();
		bool right = (x+1) >= mazeSize || rooms[x+1,y].beenVisited();
		bool down = (y+1) >= mazeSize || rooms[x,y+1].beenVisited();
		return left && up && right && down;
	}

	bool blockedBuildDir(int x, int y, int dir)
	{
		switch (dir) {
		case 0:
			return (x+1) >= mazeSize || rooms[x+1,y].beenVisited();
		case 1:
			return (y+1) >= mazeSize || rooms[x,y+1].beenVisited();
		case 2:
			return (x-1) < 0 || rooms[x-1,y].beenVisited();
		case 3:
			return (y-1) < 0 || rooms[x,y-1].beenVisited();
		}
		return true;
	}

    void printMaze()
    {
        List<Point> playerLocs = getPlayerLocations();

        Console.Write("║");
        for(int i = 0; i < rooms.GetLength(0); i++)
        {
            Console.Write("═══");
        }
        Console.Write("\n");
        for(int i = 0; i < rooms.GetLength(0); i++)
        {
            Console.Write("║");
            for(int j = 0; j < rooms.GetLength(1); j++)
            {
                Point ts = new Point();
                ts.X = j;
                ts.Y = i;
                if (playerLocs.Contains(ts))
                    Console.Write(playerLocs.IndexOf(ts));
                else
                    Console.Write(" ");
                if(rooms[j,i].rightWall)
                    Console.Write(" ║");
                else
                    Console.Write("  ");
            }
            Console.Write("\n");
            Console.Write("║");
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (rooms[j,i].rightWall && rooms[j,i].botWall)
                    Console.Write("══╝");
                 else if(rooms[j,i].rightWall)
                    Console.Write("  ║");
                else if(rooms[j,i].botWall)
                    Console.Write("═══");
                else
                    Console.Write("   ");
            }
            Console.Write("\n");
        }
    }

    public void update(Dictionary<Guid, Wiimote> input, int deltaTime)
    {
        List<Point> playerLocs = getPlayerLocations();

        claps.update();

        foreach (Guid ident in input.Keys)
        {
            Player currPlayer = players[ident];
            currPlayer.deltaT(deltaTime);
            if(currPlayer.down()) continue;
            Point currPlayerLoc = currPlayer.getLocation();
            Wiimote mote = input[ident];
            mote.SetRumble(false);
            bool[] moveDirs = new bool[4];
            for(int i = 0; i < 4; i++)
            {
                moveDirs[i] = !blockedMoveDir(currPlayer.x, currPlayer.y, i);
            }
            bool tapped = false;
            bool wallHit = false;
            int hiFives = 0;

            if (mote.WiimoteState.AccelState.Values.X > 2)
            {
                if(!moveDirs[2])
                {
                    Console.WriteLine("Left Movement Blocked");
                    wallHit = true;
                }
                tapped = true;
            }
            else if (mote.WiimoteState.AccelState.Values.X < -2)
            {
                if (!moveDirs[0])
                {
                    Console.WriteLine("Right Movement Blocked");
                    wallHit = true;
                }
                tapped = true;
            }
            else if (mote.WiimoteState.AccelState.Values.Z > 2)
            {
                if (!moveDirs[1])
                {
                    Console.WriteLine("Backward Movement Blocked");
                    wallHit = true;
                }
                tapped = true;
            }
            else if (mote.WiimoteState.AccelState.Values.Z < -2)
            {
                int whoElseIsHere = occupiedCount(playerLocs, currPlayerLoc);
                if(whoElseIsHere > 1)
                {
                    hiFives = whoElseIsHere - 1;
                }
                else if (!moveDirs[3])
                {
                    Console.WriteLine("Forward Movement Blocked");
                    wallHit = true;
                }
                tapped = true;
            }
            if(tapped)
            {
                currPlayer.setDownTime(200);
                if (wallHit)
                {
                    mote.SetRumble(true);
                    soundEngine.Play2D("../../sounds/maze/knock_notebook.wav");
                    foreach(Guid odent in input.Keys)
                    {
                        Point loc = players[odent].getLocation();
                        if(Math.Abs(loc.X - currPlayerLoc.X) == 1 || Math.Abs(loc.Y - currPlayerLoc.Y) == 1)
                        {
                            input[odent].SetRumble(true);
                            players[odent].setDownTime(100);
                        }
                    }
                }
                else if(hiFives > 0)
                {
                    if (hiFives == numPlayers - 1)
                    {
                        claps.addSound(soundEngine.Play2D("../../sounds/maze/enthusiastic_yes.wav"));
                        winFlag = true;
                    }
                    while (hiFives > 0)
                    {
                        hiFives--;
                        if (hiFives > 0)
                            claps.addSound(soundEngine.Play2D("../../sounds/maze/slap_2.wav"));
                        else
                            claps.addSound(soundEngine.Play2D("../../sounds/maze/slap_1.wav"));
                    }
                }
                else
                {
                    soundEngine.Play2D("../../sounds/maze/pong_whoosh.wav").Volume = .4f;
                }
            }

            bool moved = false;
            bool smack = false;
            if(mote.WiimoteState.ButtonState.Right)
            {
                if(moveDirs[0])
                {
                    currPlayer.x += 1;
                    moved = true;
                }
                else
                {
                    smack = true;
                }
            }
            else if(mote.WiimoteState.ButtonState.Up)
            {
                if (moveDirs[3])
                {
                    currPlayer.y -= 1;
                    moved = true;
                }
                else
                {
                    smack = true;
                }
            }
            else if(mote.WiimoteState.ButtonState.Left)
            {
                if (moveDirs[2])
                {
                    currPlayer.x -= 1;
                    moved = true;
                }
                else
                {
                    smack = true;
                }
            }
            else if(mote.WiimoteState.ButtonState.Down)
            {
                if (moveDirs[1])
                {
                    currPlayer.y += 1;
                    moved = true;
                }
                else
                {
                    smack = true;
                }
            }

            if(moved)
            {
                currPlayer.setDownTime(200);
                printMaze();
            }
            else if(smack)
            {
                Console.WriteLine("Ow!");
                soundEngine.Play2D("../../sounds/maze/knock_table.wav");
                currPlayer.setDownTime(700);
                mote.SetRumble(true);
            }
        }
    }

    public bool isOver()
    {
        if (winFlag && claps.empty())
            Console.WriteLine("You Win!");
        return winFlag && claps.empty();
    }

    bool blockedMoveDir(int x, int y, int dir)
    {
        switch (dir)
        {
            case 0:
                return (x + 1) >= mazeSize || rooms[x, y].rightWall;
            case 1:
                return (y + 1) >= mazeSize || rooms[x, y].botWall;
            case 2:
                return (x - 1) < 0 || rooms[x - 1, y].rightWall;
            case 3:
                return (y - 1) < 0 || rooms[x, y - 1].botWall;
        }
        return true;
    }

    List<Point> getPlayerLocations()
    {
        List<Point> locs = new List<Point>(players.Count);
        foreach (Player p in players.Values)
        {
            locs.Add(p.getLocation());
        }
        return locs;
    }

    int occupiedCount(List<Point> locs, Point target)
    {
        int count = 0;
        foreach(Point loc in locs)
        {
            if(loc.Equals(target))
                count++;
        }
        return count;
    }
}

public class MazeRoom
{
	public bool rightWall;
	public bool botWall;
	bool visited;

	public MazeRoom()
	{
		visited = false;
		rightWall = true;
		botWall = true;
	}

	public void visit()
	{
		visited = true;
	}

	public bool beenVisited()
	{
		return visited;
	}

	public void breakRightWall()
	{
		rightWall = false;
	}

	public void breakBotWall()
	{
		botWall = false;
	}
}

public class Player
{
    Guid attachedInput;
    public int x;
    public int y;
    //int facing;
    int downTime;

    public Player(Guid aI, int ix, int iy)
    {
        attachedInput = aI;
        x = ix;
        y = iy;
        //facing = 3;
        downTime = 0;
    }

    public bool down()
    {
        return downTime > 0;
    }

    public void setDownTime(int ndt)
    {
        downTime = ndt;
    }

    public void deltaT(int dt)
    {
        downTime -= dt;
    }

    public Point getLocation()
    {
        Point p = new Point();
        p.X = x;
        p.Y = y;
        return p;
    }
}

public class SoundManager
{
    ISound lastSoundPlayed;
    public Stack<ISound> soundsToPlay;

    public SoundManager()
    {
        soundsToPlay = new Stack<ISound>();
    }

    public void addSound(ISound s)
    {
        Console.WriteLine("Adding sound" + s.ToString());
        if (lastSoundPlayed == null || lastSoundPlayed.Finished)
        {
            lastSoundPlayed = s;
            lastSoundPlayed.Paused = false;
        }
        else
        {
            s.Paused = true;
            soundsToPlay.Push(s);
        }
    }

    public void update()
    {
        if((lastSoundPlayed == null || lastSoundPlayed.Finished) && soundsToPlay.Count > 0)
        {
            lastSoundPlayed = soundsToPlay.Pop();
            lastSoundPlayed.Paused = false;
            Console.WriteLine("Playing sound" + lastSoundPlayed.ToString());
        }
    }

    public bool empty()
    {
        return lastSoundPlayed.Finished && soundsToPlay.Count == 0;
    }

}