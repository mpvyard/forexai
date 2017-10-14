namespace FinancePermutator
{
	using System;

	internal static class MaGen
	{
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);

		public static int GetRandom()
		{
			// 	int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };
			return 2 + Random.Next(253);
		}
	}
}