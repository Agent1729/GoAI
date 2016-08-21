using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class ScoreMarker
	{
		public int score = 0;
		public int x;
		public int y;
		public List<Group> groups;
		public Territory territory;

		public ScoreMarker(int _x, int _y)
		{
			x = _x;
			y = _y;
			groups = new List<Group>();
			territory = null;
		}

		public void searchNearby(Board b)
		{
			groups = new List<Group>();
			territory = null;

			if (b.getStone(x, y) != null)
				return;

			Stone left, right, up, down;
			left = b.getStone(x - 1, y);
			right = b.getStone(x + 1, y);
			up = b.getStone(x, y - 1);
			down = b.getStone(x, y + 1);

			if (left != null) if (!groups.Contains(left.group)) groups.Add(left.group);
			if (right != null) if (!groups.Contains(right.group)) groups.Add(right.group);
			if (up != null) if (!groups.Contains(up.group)) groups.Add(up.group);
			if (down != null) if (!groups.Contains(down.group)) groups.Add(down.group);
		}

		public Territory setNewTerritory(Board b)
		{
			if (b.getStone(x, y) != null) return null;
			territory = new Territory(this);
			territory.find(b);
			return territory;
		}

		public List<ScoreMarker> getNeighbors(Board b)
		{
			//List<TwoInts> neighbors = new List<TwoInts>();
			//if (x > 0) neighbors.Add(new TwoInts(x - 1, y));
			//if (x < b.width - 1) neighbors.Add(new TwoInts(x + 1, y));
			//if (y > 0) neighbors.Add(new TwoInts(x, y - 1));
			//if (y < b.height - 1) neighbors.Add(new TwoInts(x, y + 1));

			List<ScoreMarker> neighbors = new List<ScoreMarker>();
			if (b.getStone(x, y) != null) return neighbors;
			if (x > 0) neighbors.Add(b.scoreMarkers[x - 1][y]);
			if (x < b.width - 1) neighbors.Add(b.scoreMarkers[x + 1][y]);
			if (y > 0) neighbors.Add(b.scoreMarkers[x][y - 1]);
			if (y < b.height - 1) neighbors.Add(b.scoreMarkers[x][y + 1]);

			return neighbors;
		}

		public override bool Equals(object obj)
		{
			ScoreMarker sm = (ScoreMarker)obj;
			return x == sm.x && y == sm.y;
		}
		public override int GetHashCode()
		{
			return x+100*y;
		}
	}
}
