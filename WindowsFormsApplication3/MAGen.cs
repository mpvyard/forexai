using System;

namespace WindowsFormsApplication3
{
   internal static class MaGen
	{
		private static readonly Random Random = new Random();

		public static int GetRand()
		{
		//	int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };

			return 2 + Random.Next(252);
		}
	}
}
