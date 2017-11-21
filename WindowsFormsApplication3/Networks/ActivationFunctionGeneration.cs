using System;
using FANNCSharp;

namespace FinancePermutator.Networks
{
	class ActivationFunctionGeneration
	{
		public static Random random;

		public static ActivationFunction GetRandomActivationFunction()
		{
			random = new Random(DateTime.Now.Millisecond);
			switch (random.Next(15))
			{
				case 1:
					return ActivationFunction.COS_SYMMETRIC;
					break;
				case 2:
					return ActivationFunction.ELLIOT;
					break;
				case 3:
					return ActivationFunction.ELLIOT_SYMMETRIC;
					break;
				case 4:
					return ActivationFunction.GAUSSIAN;
					break;
				case 5:
					return ActivationFunction.GAUSSIAN_STEPWISE;
					break;
				case 6:
					return ActivationFunction.GAUSSIAN_SYMMETRIC;
					break;
				case 7:
					return ActivationFunction.LINEAR;
					break;
				case 8:
					return ActivationFunction.LINEAR_PIECE;
					break;
				case 9:
					return ActivationFunction.LINEAR_PIECE_SYMMETRIC;
					break;
				case 10:
					return ActivationFunction.SIGMOID;
					break;
				case 11:
					return ActivationFunction.SIGMOID_STEPWISE;
					break;
				case 12:
					return ActivationFunction.SIGMOID_SYMMETRIC;
					break;
				case 13:
					return ActivationFunction.SIGMOID_SYMMETRIC_STEPWISE;
					break;
				case 14:
					return ActivationFunction.SIN_SYMMETRIC;
					break;
				default:
				case 15:
					return ActivationFunction.THRESHOLD;
					break;
			}
		}
	}
}