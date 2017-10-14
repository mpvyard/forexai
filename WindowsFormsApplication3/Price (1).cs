using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication3
{
	internal class Price
	{
		public DateTime date;
		public double Open;
		public double Close;
		public double High;
		public double Low;
		public double Vol;
	}

	internal class Prices
	{
		public double[] getHigh(int num)
		{
			int startIdx = Program.prices.Count - num;
			double[] copy = new double[num];

			for (int i = startIdx; i < Program.prices.Count(); i++)
			{
				copy[i - startIdx] = Program.prices[i].High;
			}
			return copy;
		}

		public double[] getLow(int num)
		{
			int startIdx = Program.prices.Count - num;
			double[] copy = new double[num];

			for (int i = startIdx; i < Program.prices.Count(); i++)
			{
				copy[i - startIdx] = Program.prices[i].Low;
			}
			return copy;
		}

		public double[] getOpen(int num)
		{
			int startIdx = Program.prices.Count - num;
			double[] copy = new double[num];

			for (int i = startIdx; i < Program.prices.Count(); i++)
			{
				copy[i - startIdx] = Program.prices[i].Open;
			}
			return copy;
		}

		public double[] getClose(int num)
		{
			int startIdx = Program.prices.Count - num;
			double[] copy = new double[num];

			for (int i = startIdx; i < Program.prices.Count(); i++)
			{
				copy[i - startIdx] = Program.prices[i].Close;
			}
			return copy;
		}

		public double[] getVol(int num)
		{
			int startIdx = Program.prices.Count - num;
			double[] copy = new double[num];

			for (int i = startIdx; i < Program.prices.Count(); i++)
			{
				copy[i - startIdx] = Program.prices[i].Vol;
			}
			return copy;
		}
	}
}
