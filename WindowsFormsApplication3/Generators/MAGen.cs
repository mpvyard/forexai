using System;

namespace FinancePermutator.Generators
{
	internal static class MaGen
	{
		public static int GetRandom(int count)
		{
			// 	int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };
			return XRandom.next(2, count - 1);
		}
	}
}