using System;

namespace FinancePermutator.Generators
{
	internal static class MaGen
	{
		private static Random random;

		public static int GetRandom(int seed)
		{
			random = new Random(seed);
			// 	int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };
			return 2 + random.Next(253);
		}
	}
}