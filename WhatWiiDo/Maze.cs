using System;
using System.Collections.Generic;
using WiimoteLib;


public class Maze
{
	int numPlayers;
	int mazeSize;
	MazeRoom[,] rooms;

	public Maze(int nP)
	{
		numPlayers = nP;
		mazeSize = numPlayers + 2;
		rooms = new MazeRoom[mazeSize, mazeSize];
        buildMaze();
        printMaze();
        Console.ReadLine();
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
			while(blockedDir(x, y, dir))
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

	bool blockedDir(int x, int y, int dir)
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
