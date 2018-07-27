using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePermutator
{
	public static class ForexExtensions
	{
		public static int rnd(this int prefix)
		{
			return new Random().Next(0, prefix);
		}

		public static int rnd(this double prefix)
		{
			return new Random().Next(0, (int) prefix);
		}
	}
}