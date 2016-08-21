using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class AI0 : AI
	{
		public int player;

		public double groupSizeMod = 1;
		public double libertyMod = 1;
		public double eyeMod = 1;
		public double territoryMod = 1;
		
		public override TwoInts takeTurn(Board b)
		{
			//Console.WriteLine("Starting AI0.takeTurn");
			Random r = new Random();
			int x = -1;
			int y = -1;
			double[][] moves = new double[b.width][];
			for (int i = 0; i < b.width; i++)
			{
				moves[i] = new double[b.height];
				for(int j=0; j<b.height; j++)
					moves[i][j] = 0.0;
			}

			//Set values
			for (int i = 0; i < b.width; i++)
			{
				for(int j=0; j<b.height; j++)
				{
					if (b.getStone(i, j) != null)
						continue;
					//moves[i][j] = calculateOdds(b.width, i, b.height, j);
					moves[i][j] = calculateOdds(b, i, j);
				}
			}

			//Get the random val
			double total = 0;
			for (int i = 0; i < b.width; i++)
				for (int j = 0; j < b.height; j++)
					total+=moves[i][j];
			double d = r.NextDouble() * total;

			//Find which cell it corresponds to
			for (int i = 0; i < b.width; i++)
			{
				for (int j = 0; j < b.height; j++)
				{
					d -= moves[i][j];
					if(d < 0)
					{
						x = i;
						y = j;
						break;
					}
				}
				if (d < 0)
					break;
			}

			//Console.WriteLine("Ending with " + x + "," + y+" it has a value of: "+moves[x][y]);
			//Console.WriteLine("Finishing AI0.takeTurn");
			//Console.ReadLine();
			return new TwoInts(x, y);
		}

		public double calculateOdds(int w, int i, int h, int j)
		{
			return ((double)(i + 1) / (double)(w + 1)) * ((double)(j + 1) / (double)(h + 1));
		}

		public double calculateOdds(Board b, int x, int y)
		{
			Board b2 = b.clone();
			b2.placePiece(x, y, player, true);

			//If self capture
			if (b2.getStone(x, y) == null)
			{
				//Console.WriteLine("Play at " + x + "," + y + " would result in self capture, setting to 0");
				return 0;
			}

			//Find differences between b/b2 groups
			List<GroupDifference> diffs = new List<GroupDifference>();
			foreach(Group g in b.groups)
			{
				Stone s = g.stones[0];
				Stone s2 = b2.getStone(s.x, s.y);
				if(s2==null)
				{
					//This group got captured
					diffs.Add(new GroupDifference(g.player, -g.stones.Count, 0, -g.getLiberties(b).Count, -g.getEyes(b).Count));
					continue;
				}
				Group g2 = s2.group;
				//Calculate differences between g and g2
				diffs.Add(new GroupDifference(g.player, g2.stones.Count - g.stones.Count, g2.stones.Count, g2.getLiberties(b2).Count - g.getLiberties(b).Count, g2.getEyes(b2).Count - g.getEyes(b).Count));
			}

			//If placed a solitary stone
			if(b2.getStone(x,y).group.stones.Count==1)
			{
				//Add as a new group difference
				Stone s2 = b2.getStone(x, y);
				diffs.Add(new GroupDifference(s2.player, 1, 1, s2.group.getLiberties(b2).Count, s2.group.getEyes(b2).Count));
			}

			//Find diferences between b/b2 territories
			int[] territory = new int[3];
			foreach (Territory t in b.territories)
				territory[t.player] += t.scoreMarkers.Count;
			int[] territory2 = new int[3];
			foreach (Territory t in b2.territories)
				territory2[t.player] += t.scoreMarkers.Count;
			int[] territoryDiff = new int[3];
			for (int i = 0; i < 3; i++)
				territoryDiff[i] = territory2[i] - territory[i];

			//Calculate score of those differences
			return calculateScoreOfDiffs(diffs, territoryDiff);
		}

		public double calculateScoreOfDiffs(List<GroupDifference> diffs, int[] territoryDiff)
		{
			double score = 0;

			foreach(GroupDifference gd in diffs)
			{
				double tempScore = Math.Sqrt((double)gd.stoneCount * groupSizeMod) * ((double)gd.libertyDiff * libertyMod + (double)gd.eyesDiff * eyeMod);
				if (gd.player != player) tempScore *= -1;
				score += tempScore;
			}
			for (int i = 1; i < 3; i++)
			{
				double tempScore = (double)territoryDiff[i] * territoryMod;
				if (i != player) tempScore *= -1;
				score += tempScore;
			}

			if (score < 0) score = 0;
			return score;
		}

		public void setAI(int _player, double gsm, double lm, double em, double tm)
		{
			player = _player;
			groupSizeMod = gsm;
			libertyMod = lm;
			eyeMod = em;
			territoryMod = tm;
		}

		public struct GroupDifference
		{
			public int player;
			public int stoneCountDiff;
			public int stoneCount;
			public int libertyDiff;
			public int eyesDiff;

			public GroupDifference(int _p, int _sd, int _s, int _l, int _e)
			{
				player = _p;
				stoneCountDiff = _sd;
				stoneCount = _s;
				libertyDiff = _l;
				eyesDiff = _e;
			}
		}
	}
}
