using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class TwoInts
	{
		public int a;
		public int b;

		public TwoInts(int _a, int _b)
		{
			a = _a;
			b = _b;
		}

		public override bool Equals(object obj)
		{
			TwoInts o = (TwoInts)obj;
			return (a == o.a && b == o.b);
		}

		public override int GetHashCode()
		{
			int i;
			i = a.GetHashCode() * b.GetHashCode();
			return i;
		}
	}
}
