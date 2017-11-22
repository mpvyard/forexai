using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePermutator
{
	public static class XRandom
	{
		public static Random random;

		public static void Init()
		{
			random = new Random(DateTime.Now.Millisecond);
		}

		public static int next(int n)
		{
			return random.Next(n);
		}

		public static int next(int n, int m)
		{
			return random.Next(Math.Min(n, m), Math.Max(n, m));
		}
	}
}