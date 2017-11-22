using System;

namespace FinancePermutator.Generators
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