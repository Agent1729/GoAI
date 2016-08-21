using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class Board
	{
		public int width;
		public int height;
		public Stone[][] b;
		public int[][] helper;
		public List<Group> groups;
		public ScoreMarker[][] scoreMarkers;
		public List<Territory> territories;
		public int[] playerScore;

		//public int BOARDSIZE;

		public Board(int sizex, int sizey)
		{
			//BOARDSIZE = size;
			width = sizex;
			height = sizey;
			b = new Stone[width][];
			for (int i = 0; i < width; i++)
			{
				b[i] = new Stone[height];
				for (int j = 0; j < height; j++)
					b[i][j] = null;
			}
			helper = new int[width][];
			for (int i = 0; i < width; i++)
			{
				helper[i] = new int[height];
				for (int j = 0; j < height; j++)
					helper[i][j] = 0;
			}
			groups = new List<Group>();
			scoreMarkers = new ScoreMarker[width][];
			for (int i = 0; i < width; i++)
			{
				scoreMarkers[i] = new ScoreMarker[height];
				for (int j = 0; j < height; j++)
					scoreMarkers[i][j] = new ScoreMarker(i, j);
			}
			territories = new List<Territory>();
			playerScore = new int[2] { 0, 0 };
		}

		public int getWinner()
		{
			int p1 = playerScore[0];
			int p2 = playerScore[1];
			int p1s = playerScore[0];
			int p2s = playerScore[1];
			int p1t = 0;
			int p2t = 0;
			foreach (Territory t in territories)
			{
				if (t.player == 1)
					p1t += t.scoreMarkers.Count;
				if (t.player == 2)
					p2t += t.scoreMarkers.Count;
			}
			p1 = p1s + p1t;
			p2 = p2s + p2t;
			if (p1 > p2)
			{
				return 1;
			}
			else
			{
				return 2;
			}
		}

		public bool placePiece(int _x, int _y, int _player, bool setExtras)
		{
			if (placeNewStone(_x, _y, _player))
			{
				setHelper();
				int i = 0;
				foreach (Group g in groups)
					if(setExtras)
						g.hasChanged = true;

				Group toRemove = null;
				bool somethingRemoved;

				do
				{
					somethingRemoved = false;
					for (i = 0; i < groups.Count; i++)
					{
						if (groups[i].stones == null) continue;
						if (groups[i].getLiberties(this).Count == 0)
						{
							if (groups[i].player == _player)
							{
								toRemove = groups[i];
							}
							else
							{
								//groups[i].kill(this);
								//toRemove = null;
								toRemove = groups[i];
								break;
							}
						}
					}
					if (toRemove != null)
					{
						toRemove.kill(this);
						toRemove = null;
						somethingRemoved = true;
						foreach (Group g in groups)
							if (setExtras)
								g.hasChanged = true;
					}
				} while (somethingRemoved);

				i = 0;
				//Potential infinite loop?
				//Remove all empty groups
				while (i < groups.Count)
				{
					foreach (Group g in groups)
					{
						if (g.stones == null)
						{
							groups.Remove(g);
							i = 0;
							break;
						}
						i++;
					}
				}
				foreach (Group g in groups)
					if (setExtras)
						g.hasChanged = true;

				//curplayer = 3 - curplayer;

				if (setExtras)
				{
					setScoreMarkers();
					setHelperTerritories();
					setTerritories();
					setEyes();
				}
				return true;
			}
			return false;
		}

		public bool placeNewStone(int x, int y, int player)
		{
			if (x < 0 || x >= width) return false;
			if (y < 0 || y >= height) return false;
			if(b[x][y]==null)
			{
				b[x][y] = new Stone(x,y,player,this);
				return true;
			}
			return false;
		}

		public void setHelper()
		{
			int i;
			for (i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					helper[i][j] = 0;
			i = 1;
			foreach(Group g in groups)
			{
				foreach(Stone s in g.stones)
					helper[s.x][s.y] = i;
				i++;
			}
		}

		public Stone getStone(int x, int y)
		{
			if (x < 0 || x >= width) return null;
			if (y < 0 || y >= height) return null;
			return b[x][y];
		}

		public void setScoreMarkers()
		{
			for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					scoreMarkers[i][j].searchNearby(this);
		}

		public void setTerritories()
		{
			territories = new List<Territory>();
			for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					if (scoreMarkers[i][j].territory == null)
					{
						Territory t = scoreMarkers[i][j].setNewTerritory(this);
						if (t != null)
							territories.Add(t);
					}
		}

		public void setHelperTerritories()
		{
			int num = -1;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (helper[i][j] == 0)
					{
						recursiveSetHelperTerritory(i, j, num);
						num--;
					}
				}
			}
		}

		public void recursiveSetHelperTerritory(int x, int y, int num)
		{
			helper[x][y] = num;
			if (x > 0 && helper[x - 1][y] == 0) recursiveSetHelperTerritory(x - 1, y, num);
			if (x < width - 1 && helper[x + 1][y] == 0) recursiveSetHelperTerritory(x + 1, y, num);
			if (y > 0 && helper[x][y - 1] == 0) recursiveSetHelperTerritory(x, y - 1, num);
			if (y < height - 1 && helper[x][y + 1] == 0) recursiveSetHelperTerritory(x, y + 1, num);
		}

		public void setEyes()
		{
			foreach(Group g in groups)
			{
				g.getEyes(this);
			}
		}

		public void findGroups()
		{
			groups = new List<Group>();
			//Setup preliminary helper
			for(int i=0; i<width; i++)
			{
				for(int j=0; j<height; j++)
				{
					helper[i][j] = 0;
					if (b[i][j] != null)
						helper[i][j] = -1;
				}
			}

			int num = 1;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (helper[i][j] == -1)
					{
						recursiveFindGroup(i, j, num, null);
						num++;
					}
				}
			}
		}

		public void recursiveFindGroup(int x, int y, int num, Group g)
		{
			helper[x][y] = num;
			Stone s = b[x][y];
			if (g == null)
			{
				g = new Group(s, this);
				//groups.Add(g);
			}
			else { g.stones.Add(s); }
			s.group = g;
			if (x > 0 && helper[x - 1][y] == -1 && b[x - 1][y].player == s.player) recursiveFindGroup(x - 1, y, num, g);
			if (x < width - 1 && helper[x + 1][y] == -1 && b[x + 1][y].player == s.player) recursiveFindGroup(x + 1, y, num, g);
			if (y > 0 && helper[x][y - 1] == -1 && b[x][y - 1].player == s.player) recursiveFindGroup(x, y - 1, num, g);
			if (y < height - 1 && helper[x][y + 1] == -1 && b[x][y + 1].player == s.player) recursiveFindGroup(x, y + 1, num, g);
		}

		public List<Group> findDeadGroups()
		{
			List<Group> deadGroups = new List<Group>();
			foreach (Group g in groups)
			{
				bool hasFalseEye = false;
				List<Territory> eyes = g.getEyes(this);
				foreach (Territory t in eyes)
				{
					if (t.player != g.player)
					{
						hasFalseEye = true;
						break;
					}
				}
				if (!hasFalseEye)
					continue;

				//Has a false eye

				if (g.stones.Count < 6)
				{
					deadGroups.Add(g);
					continue;
				}
			}
			return deadGroups;
		}


		public Board clone()
		{
			Board board2 = new Board(width, height);

			board2.width = width;
			board2.height = height;

			//Add stones to board
			List<Stone> toAdd = new List<Stone>();
			for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					if (b[i][j] != null)
						toAdd.Add(b[i][j]);
			for (int i = 0; i < toAdd.Count; i++)
			{
				Stone s = toAdd[i];
				if (i == toAdd.Count - 1)
				{
					board2.findGroups();
					board2.placePiece(s.x, s.y, s.player, true);
				}
				else
				{
					//board2.placePiece(i, j, b[i][j].player, false);
					board2.b[s.x][s.y] = new Stone(s.x, s.y, s.player, board2, false);
				}
			}

			//Stone[][] b clone
			//for (int i = 0; i < width; i++)
			//	for (int j = 0; j < height; j++)
			//		board2.b[i][j] = b[i][j].clone(board2);

			////List<Group> groups clone
			//foreach (Group g in groups)
			//	board2.groups.Add(g.clone());

			////ScoreMarker[][] scoreMarkers clone
			//for (int i = 0; i < width; i++)
			//	for (int j = 0; j < height; j++)
			//		board2.scoreMarkers[i][j] = scoreMarkers[i][j].clone();

			////List<Territory> territories clone
			//foreach (Territory t in territories)
			//	board2.territories.Add(t.clone());

			////int[2] playerScore clone
			//board2.playerScore[0] = playerScore[0];
			//board2.playerScore[1] = playerScore[1];

			return board2;
		}


		public void printScore(bool useTerritories)
		{
			int p1 = playerScore[0];
			int p2 = playerScore[1];
			int p1s = playerScore[0];
			int p2s = playerScore[1];
			int p1t = 0;
			int p2t = 0;
			if (useTerritories)
			{
				foreach (Territory t in territories)
				{
					if (t.player == 1)
						p1t += t.scoreMarkers.Count;
					if (t.player == 2)
						p2t += t.scoreMarkers.Count;
				}
				p1 = p1s + p1t;
				p2 = p2s + p2t;
				Console.Write("P1: " + p1s + " + " + p1t + " = " + p1 + "\t\t" + "P2: " + p2s + " + " + p2t + " + .5 = " + p2 + ".5\n");
			}
			else
			{
				Console.Write("P1: " + p1 + "\t\t" + "P2: " + p2 + "\n");
			}
		}

		public void printWinner()
		{
			int p1 = playerScore[0];
			int p2 = playerScore[1];
			int p1s = playerScore[0];
			int p2s = playerScore[1];
			int p1t = 0;
			int p2t = 0;
			foreach (Territory t in territories)
			{
				if (t.player == 1)
					p1t += t.scoreMarkers.Count;
				if (t.player == 2)
					p2t += t.scoreMarkers.Count;
			}
			p1 = p1s + p1t;
			p2 = p2s + p2t;
			if (p1 > p2)
			{
				Console.Write("Player 1 wins!\t\tBy " + (p1 - p2 - 1) + ".5 points.\n");
			}
			else
			{
				Console.Write("Player 2 wins!\t\tBy " + (p2 - p1) + ".5 points.\n");
			}
		}

		public void printBoard(int cx, int cy)
		{
			Console.Write("\n\n  +");
			for (int i = 0; i < width; i++)
				Console.Write("-");
			Console.Write("+\n");

			for (int j = 0; j < height; j++)
			{
				Console.Write("  |");
				for (int i = 0; i < width; i++)
				{
					if (cx != i || cy != j)
					{
						if (b[i][j] == null) Console.Write(" ");
						else if (b[i][j].player == 1) Console.Write((char)2);
						else if (b[i][j].player == 2) Console.Write((char)1);
					}
					else
					{
						if (b[i][j] == null) { Console.Write((char)183); }
						else if (b[i][j].player == 1) Console.Write((char)164);
						else if (b[i][j].player == 2) Console.Write((char)'O');
					}
				}
				Console.Write("|\n");
			}

			Console.Write("  +");
			for (int i = 0; i < width; i++)
				Console.Write("-");
			Console.Write("+\n\n");

		}

		public void printTerritories()
		{
			foreach (Territory t in territories)
				Console.Write("Territory(" + t.player + ") " + t.scoreMarkers.Count + "\n");
		}

		public void printGroups()
		{
			for (int i = 0; i < groups.Count; i++)
			{
				Console.Write("Group " + groups[i].player + ": ");
				for (int j = 0; j < groups[i].stones.Count; j++)
				{
					Console.Write("(" + groups[i].stones[j].x + "," + groups[i].stones[j].y + ") ");
				}
				Console.Write("\t");
				Console.Write(groups[i].getLiberties(this).Count);
				Console.Write("\n");
			}
		}
	}
}
