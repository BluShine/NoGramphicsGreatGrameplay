using System;
using System.Collections.Generic;
using WhatWiiDo;
using WiimoteLib;
using System.Media;


public class Maze : Minigame
{
	int numPlayers;
	int mazeSize;
	MazeRoom[,] rooms;
    Dictionary<Guid, Player> players;

    static float[] spawnLocations = { 0, 0, 1, 1, 0, 1, 1, 0, .5f, .5f, 0, .5f, .5f, 0, 1, .5f, .5f, 1 };

    public Maze(Dictionary<Guid, Wiimote> input)
	{
		numPlayers = input.Count;
		mazeSize = numPlayers + 2;
		rooms = new MazeRoom[mazeSize, mazeSize];
        players = new Dictionary<Guid, Player>(numPlayers);
        int i = 0;
        foreach(Guid ident in input.Keys)
        {
            players.Add(ident, new Player(ident, (int)(spawnLocations[i]*(mazeSize-1)), (int)(spawnLocations[i+1]*(mazeSize-1))));
            i += 2;
        }
        buildMaze();
        //printMaze();
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
                if(rooms[j,i].rightWall)
                    Console.Write("  ║");
                else
                    Console.Write("   ");
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
        foreach (Guid ident in input.Keys)
        {
            Player currPlayer = players[ident];
            currPlayer.deltaT(1);
            if(currPlayer.down()) continue;
            Wiimote mote = input[ident];
            bool[] moveDirs = new bool[4];
            for(int i = 0; i < 4; i++)
            {
                moveDirs[i] = !blockedMoveDir(currPlayer.x, currPlayer.y, i);
            }
            if (mote.WiimoteState.AccelState.Values.X > 2)
            {
                if(!moveDirs[2])
                {
                    Console.WriteLine("Left Movement Blocked");
                }
                currPlayer.setDownTime(30);
            }
            else if (mote.WiimoteState.AccelState.Values.X < -2)
            {
                if (!moveDirs[0])
                {
                    Console.WriteLine("Right Movement Blocked");
                }
                currPlayer.setDownTime(30);
            }
            else if (mote.WiimoteState.AccelState.Values.Z > 2)
            {
                if (!moveDirs[1])
                {
                    Console.WriteLine("Backward Movement Blocked");
                }
                currPlayer.setDownTime(30);
            }
            else if (mote.WiimoteState.AccelState.Values.Z < -2)
            {
                if (!moveDirs[3])
                {
                    Console.WriteLine("Forward Movement Blocked");
                }
                currPlayer.setDownTime(30);
            }

            if(mote.WiimoteState.ButtonState.Right)
            {
                if(moveDirs[0])
                    currPlayer.x += 1;
                else
                    Console.WriteLine("Ow!");
                currPlayer.setDownTime(30);
            }
            else if(mote.WiimoteState.ButtonState.Up)
            {
                if(moveDirs[3])
                    currPlayer.y -= 1;
                else
                    Console.WriteLine("Ow!");
                currPlayer.setDownTime(30);
            }
            else if(mote.WiimoteState.ButtonState.Left)
            {
                if(moveDirs[2])
                    currPlayer.x -= 1;
                else
                    Console.WriteLine("Ow!");
                currPlayer.setDownTime(30);
            }
            else if(mote.WiimoteState.ButtonState.Down)
            {
                if(moveDirs[1])
                    currPlayer.y += 1;
                else
                    Console.WriteLine("Ow!");
                currPlayer.setDownTime(30);
            }
        }
    }

    public bool isOver()
    {
        if(playerConvergence())
        {
            Console.WriteLine("You Win!");
            return true;
        }
        return false;
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

    bool playerConvergence()
    {
        bool res = true;
        int allx = -1;
        int ally = -1;
        foreach(Player p in players.Values)
        {
            if (allx < 0)
            {
                allx = p.x;
                ally = p.y;
            }
            else
            {
                res &= p.x == allx;
                res &= p.y == ally;
            }
        }
        return res;
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
    int facing;
    int downTime;

    public Player(Guid aI, int ix, int iy)
    {
        attachedInput = aI;
        x = ix;
        y = iy;
        facing = 3;
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
}
