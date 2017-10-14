using System.Linq;

namespace WindowsFormsApplication3
{
	internal class Prices
	{
		public static double[] GetHigh(int num)
		{
			var startIdx = Data.Prices.Count - num;
			var copy = new double[num];

			for (var i = startIdx; i < Data.Prices.Count(); i++)
			{
				copy[i - startIdx] = Data.Prices[i].High;
			}
			return copy;
		}

		public static double[] GetLow(int num)
		{
			var startIdx = Data.Prices.Count - num;
			var copy = new double[num];

			for (var i = startIdx; i < Data.Prices.Count(); i++)
			{
				copy[i - startIdx] = Data.Prices[i].Low;
			}
			return copy;
		}

		public static double[] GetOpen(int num)
		{
			var startIdx = Data.Prices.Count - num;
			var copy = new double[num];

			for (var i = startIdx; i < Data.Prices.Count(); i++)
			{
				copy[i - startIdx] = Data.Prices[i].Open;
			}
			return copy;
		}

		public static double[] GetClose(int num)
		{
			var startIdx = Data.Prices.Count - num;
			var copy = new double[num];

			for (var i = startIdx; i < Data.Prices.Count(); i++)
			{
				copy[i - startIdx] = Data.Prices[i].Close;
			}
			return copy;
		}

		public static double[] GetVolume(int num)
		{
			var startIdx = Data.Prices.Count - num;
			var copy = new double[num];

			for (var i = startIdx; i < Data.Prices.Count(); i++)
			{
				copy[i - startIdx] = Data.Prices[i].Vol;
			}
			return copy;
		}
	}
}
