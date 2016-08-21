using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class Stone
	{
		public int x;
		public int y;
		public int player;
		public Group group;

		public Stone(int _x, int _y, int _p, Board _b)
		{
			x = _x;
			y = _y;
			player = _p;
			group = null;
			addToGroup(_b);
		}
		public Stone(int _x, int _y, int _p, Board _b, bool shouldAddToGroup)
		{
			x = _x;
			y = _y;
			player = _p;
			group = null;
			//addToGroup(_b);
		}
		public int liberties(Board b)
		{
			int l = 0;
			if (b.getStone(x - 1, y) == null && x != 0) l++;
			if (b.getStone(x + 1, y) == null && x != b.width - 1) l++;
			if (b.getStone(x, y - 1) == null && y != 0) l++;
			if (b.getStone(x, y + 1) == null && x != b.height - 1) l++;
			return l;
		}
		public List<TwoInts> getLiberties(Board b)
		{
			List<TwoInts> l = new List<TwoInts>();
			if (b.getStone(x - 1, y) == null && x != 0) l.Add(new TwoInts(x - 1, y));
			if (b.getStone(x + 1, y) == null && x != b.width - 1) l.Add(new TwoInts(x + 1, y));
			if (b.getStone(x, y - 1) == null && y != 0) l.Add(new TwoInts(x, y - 1));
			if (b.getStone(x, y + 1) == null && y != b.height - 1) l.Add(new TwoInts(x, y + 1));
			return l;
		}

		public void addToGroup(Board b)
		{
			Stone left, right, up, down;
			left = b.getStone(x - 1, y);
			right = b.getStone(x + 1, y);
			up = b.getStone(x, y - 1);
			down = b.getStone(x, y + 1);

			group = new Group(this, b);
			if (left != null && left.player == player && left.group != group) group.combineGroups(left.group, b);
			if (right != null && right.player == player && right.group != group) group.combineGroups(right.group, b);
			if (up != null && up.player == player && up.group != group) group.combineGroups(up.group, b);
			if (down != null && down.player == player && down.group != group) group.combineGroups(down.group, b);
		}

		public void kill(Board b)
		{
			b.b[x][y] = null;
			group = null;
		}


		public Stone clone(Board b)
		{
			Stone stone2 = new Stone(x, y, player, b);
			return stone2;
		}
	}
}
