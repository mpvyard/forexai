using System;
using TicTacTec.TA.Library;

namespace WindowsFormsApplication3
{
	internal class MATypeGen
	{
		private static readonly Random _random = new Random(DateTime.Now.Millisecond);

		public static Core.MAType GetRand()
		{
			var randomIndex = _random.Next(9);
			switch (randomIndex)
			{
				case 0:
					return Core.MAType.Sma;
				case 1:
					return Core.MAType.Dema;
				case 2:
					return Core.MAType.Ema;
				case 3:
					return Core.MAType.Kama;
				case 4:
					return Core.MAType.Mama;
				case 5:
					return Core.MAType.T3;
				case 6:
					return Core.MAType.Tema;
				case 7:
					return Core.MAType.Trima;
				case 8:
				default:
					return Core.MAType.Wma;
			}
		}
	}
}