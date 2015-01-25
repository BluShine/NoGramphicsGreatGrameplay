using System;
using System.Collections;
using System.Drawing;


public class Maze
{
	int numPlayers;
	int mazeSize;
	MazeRoom[][] rooms;

	Maze(int nP)
	{
		numPlayers = nP;
		mazeSize = numPlayers + 2;
		rooms = new MazeRoom[mazeSize][mazeSize];
	}

	void buildMaze() {
		int x = 0;
		int y = 0;
		Stack<Point> path = new Stack<Point>();

		while(true)
		{
			rooms[x][y].visit();
			if(surrounded(x, y))
			{
				if(path.Count == 0)
				{
					break;
				}
				else
				{
					Point bt = path.Pop();
					x = bt.x;
					y = bt.y;
					continue;
				}
			}
			Random gen = new Random();
			int dir = gen.Next(0, 3);
			while(blockedDir(x, y, dir))
			{
				dir = gen.Next(0, 3);
			}
			path.Push(new Point(x, y));
			switch (dir) {
			case 0:
				rooms[x][y].breakRightWall();
				x += 1;
				break;
			case 1:
				rooms[x][y].breakBotWall();
				y += 1;
				break;
			case 2:
				rooms[x-1][y].breakRightWall();
				x -= 1;
				break;
			case 3:
				rooms[x][y-1].breakBotWall();
				y -= 1;
				break;
			}
		}

	}

	bool surrounded(int x, int y)
	{
		bool left = (x-1) < 0 || rooms[x-1][y].beenVisited();
		bool up = (y-1) < 0 || rooms[x][y-1].beenVisited();
		bool right = (x+1) >= mazeSize || rooms[x+1][y].beenVisited();
		bool down = (y+1) >= mazeSize || rooms[x][y+1].beenVisited();
		return left && up && right && down;
	}

	bool blockedDir(int x, int y, int dir)
	{
		switch (dir) {
		case 0:
			return (x + 1) >= mazeSize || rooms [x + 1] [y].beenVisited ();
			break;
		case 1:
			return (y + 1) >= mazeSize || rooms [x] [y + 1].beenVisited ();
			break;
		case 2:
			return (x - 1) < 0 || rooms [x - 1] [y].beenVisited ();
			break;
		case 3:
			return (y - 1) < 0 || rooms [x] [y - 1].beenVisited ();
			break;
		}
		return true;
	}


}

public class MazeRoom
{
	bool rightWall;
	bool botWall;
	bool visited;

	MazeRoom()
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
