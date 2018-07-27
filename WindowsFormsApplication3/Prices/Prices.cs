﻿namespace FinancePermutator.Prices
{
	internal static class ForexPrices
	{
		public static double[] GetHigh(int num, int offset = 0)
		{
			var copy = new double[num];

			int i = 0;
			for (int index = offset; index < offset + num; index++)
			{
				if (index >= Repository.Prices.Count - 1)
					break;
				copy[i++] = Repository.Prices[index].High;
			}

			return copy;
		}

		public static double[] GetLow(int num, int offset = 0)
		{
			var copy = new double[num];

			int i = 0;
			for (int index = offset; index < offset + num; index++)
			{
				if (index >= Repository.Prices.Count - 1)
					break;
				copy[i++] = Repository.Prices[index].Low;
			}

			return copy;
		}

		public static double[] GetOpen(int num, int offset = 0)
		{
			var copy = new double[num];

			int i = 0;
			for (int index = offset; index < offset + num; index++)
			{
				if (index >= Repository.Prices.Count - 1)
					break;
				copy[i++] = Repository.Prices[index].Open;
			}
			return copy;
		}

		public static double[] GetClose(int num, int offset = 0)
		{
			var copy = new double[num];

			int i = 0;
			for (int index = offset; index < offset + num; index++)
			{
				if (index >= Repository.Prices.Count - 1)
					break;
				copy[i++] = Repository.Prices[index].Close;
			}

			return copy;
		}

		public static double[] GetVolume(int num, int offset = 0)
		{
			var copy = new double[num];

			int i = 0;
			for (int index = offset; index < offset + num; index++)
			{
				if (index >= Repository.Prices.Count - 1)
					break;
				copy[i++] = Repository.Prices[index].Vol;
			}

			return copy;
		}
	}
}