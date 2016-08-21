using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class Group
	{
		public int player;
		public List<Stone> stones;
		public int numEyes;
		public bool hasChanged;
		public List<TwoInts> liberties;

		public Group(Stone s, Board b)
		{
			stones = new List<Stone>();
			stones.Add(s);
			player = s.player;
			b.groups.Add(this);
			numEyes = 0;
			hasChanged = true;
			liberties = new List<TwoInts>();
		}

		public List<TwoInts> getLiberties(Board b)
		{
			if (!hasChanged) return liberties;
			List<TwoInts> l = new List<TwoInts>();

			//if (stones == null) return l;
			//for (int i = 0; i < stones.Count; i++)
			//{
			//	List<TwoInts> ls = stones[i].getLiberties(b);
			//	l = l.Union(ls).ToList();
			//}



			//int[][] bl = new int[b.width][];
			//for (int i = 0; i < b.width; i++)
			//{
			//	bl[i] = new int[b.height];
			//	for (int j = 0; j < b.height; j++)
			//	{
			//		bl[i][j] = -1;
			//		if (b.getStone(i, j) != null)
			//			bl[i][j] = 0;
			//	}
			//}
			//foreach (Stone s in stones)
			//	bl[s.x][s.y] = 1;
			//for (int i = 0; i < b.width; i++)
			//{
			//	for (int j = 0; j < b.height; j++)
			//	{
			//		if (i > 0 && bl[i][j] == -1 && bl[i - 1][j] == 1) l.Add(new TwoInts(i, j));
			//		else if (i < b.width - 1 && bl[i][j] == -1 && bl[i + 1][j] == 1) l.Add(new TwoInts(i, j));
			//		else if (j > 0 && bl[i][j] == -1 && bl[i][j - 1] == 1) l.Add(new TwoInts(i, j));
			//		else if (j < b.height - 1 && bl[i][j] == -1 && bl[i][j + 1] == 1) l.Add(new TwoInts(i, j));
			//	}
			//}



			int groupNumber = b.helper[stones[0].x][stones[0].y];
			for (int i = 0; i < b.width; i++)
			{
				for (int j = 0; j < b.height; j++)
				{
					if (b.helper[i][j] > 0)
						continue;
					if (i > 0 && b.helper[i - 1][j] == groupNumber) l.Add(new TwoInts(i, j));
					else if (i < b.width - 1 && b.helper[i + 1][j] == groupNumber) l.Add(new TwoInts(i, j));
					else if (j > 0 && b.helper[i][j - 1] == groupNumber) l.Add(new TwoInts(i, j));
					else if (j < b.height - 1 && b.helper[i][j + 1] == groupNumber) l.Add(new TwoInts(i, j));
				}
			}

			hasChanged = false;
			liberties = l;
			return l;
		}

		public List<Territory> getEyes(Board b)
		{
			List<Territory> eyes = new List<Territory>();
			//List<TwoInts> liberties = getLiberties(b);
			//foreach(TwoInts liberty in liberties)
			//{
			//	Territory eye = b.scoreMarkers[liberty.a][liberty.b].territory;
			//	if (!eyes.Contains(eye))
			//		eyes.Add(eye);
			//}

			foreach (Territory eye in b.territories)
				if (eye.groups.Contains(this))
					eyes.Add(eye);
			numEyes = eyes.Count;
			return eyes;
		}

		public void combineGroups(Group g, Board b)
		{
			for (int i = 0; i < g.stones.Count; i++)
			{
				stones.Add(g.stones[i]);
				g.stones[i].group = this;
			}
			g.stones = null;
			b.groups.Remove(g);
		}

		public void kill(Board b)
		{
			b.playerScore[2 - stones[0].player] += stones.Count;
			for (int i = 0; i < stones.Count; i++)
				stones[i].kill(b);
			stones = null;
			b.groups.Remove(this);
			b.setHelper();
		}



		public override bool Equals(object obj)
		{
			Group o = (Group)obj;
			if (stones == null && o.stones == null) return true;
			if (o.stones == null) return false;
			return stones[0] == o.stones[0];
		}
		public override int GetHashCode()
		{
			if (stones == null) return 0;
			return stones[0].GetHashCode();
		}
	}
}
