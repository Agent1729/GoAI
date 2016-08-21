using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class RandomAI : AI
	{
		public override TwoInts takeTurn(Board b)
		{
			Random r = new Random();
			int x = r.Next(b.width);
			int y = r.Next(b.height);
			return new TwoInts(x, y);
		}
	}
}
