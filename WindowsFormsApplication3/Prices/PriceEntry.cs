namespace FinancePermutator.Prices
{
	using System;

	internal struct PriceEntry
	{
		public DateTime Date;

		public double Open;
		public double Close;
		public double High;
		public double Low;
		public double Vol;
	}
}