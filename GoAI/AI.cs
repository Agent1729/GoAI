using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class AI
	{
		public virtual TwoInts takeTurn(Board b)
		{
			return new TwoInts(0, 0);
		}

		public virtual void setAI(int _player, List<double> aivals)
		{

		}
	}
}
