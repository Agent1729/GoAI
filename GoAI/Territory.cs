using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class Territory
	{
		public List<ScoreMarker> scoreMarkers;
		public List<Group> groups;
		public int player;

		public Territory(ScoreMarker s)
		{
			scoreMarkers = new List<ScoreMarker>();
			scoreMarkers.Add(s);
			groups = s.groups;
			player = 0;
		}

		public void find(Board b)
		{
			//List<ScoreMarker> toExplore = scoreMarkers[0].getNeighbors(b);

			//while (toExplore.Count > 0)
			//{
			//	ScoreMarker s = toExplore[0];

			//	if (b.getStone(s.x, s.y) == null)
			//	{
			//		scoreMarkers.Add(s);
			//		s.territory = this;
			//		groups = groups.Union(s.groups).ToList();
			//		toExplore = toExplore.Union(s.getNeighbors(b)).ToList();
			//		toExplore = toExplore.Except(scoreMarkers).ToList();
			//	}
			//	else
			//	{
			//		toExplore.RemoveAt(0);
			//	}
			//}



			int territoryNum = b.helper[scoreMarkers[0].x][scoreMarkers[0].y];
			scoreMarkers = new List<ScoreMarker>();
			groups = new List<Group>();
			for(int i=0; i<b.width; i++)
			{
				for(int j=0; j<b.height; j++)
				{
					if(b.helper[i][j]==territoryNum)
					{
						ScoreMarker s = b.scoreMarkers[i][j];
						scoreMarkers.Add(s);
						s.territory = this;
						//groups = groups.Union(s.groups).ToList();
						groups.AddRange(s.groups);
					}
				}
			}
			groups = groups.Distinct().ToList();

			bool hasP1 = false;
			bool hasP2 = false;
			foreach (Group g in groups)
			{
				if (g.player == 1) hasP1 = true;
				if (g.player == 2) hasP2 = true;
			}

			if (hasP1 && hasP2) player = 0;
			if (hasP1 && !hasP2) player = 1;
			if (!hasP1 && hasP2) player = 2;
		}
	}
}
