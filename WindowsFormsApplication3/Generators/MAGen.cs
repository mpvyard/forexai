using System;

namespace FinancePermutator.Generators
{
	internal static class MaGen
	{
		public static int GetRandom(int seed)
		{

			// 	int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };
			return 2 + XRandom.next(253);
		}
	}
}