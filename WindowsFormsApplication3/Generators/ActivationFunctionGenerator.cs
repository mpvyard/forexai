using FANNCSharp;

namespace FinancePermutator.Generators
{
	static class ActivationFunctionGenerator
	{
		public static ActivationFunction GetRandomFunction()
		{
			switch(XRandom.next(15))
			{
				default:
				case 1:
					return ActivationFunction.SIGMOID_SYMMETRIC_STEPWISE;
				case 2:
					return ActivationFunction.ELLIOT;
				case 3:
					return ActivationFunction.ELLIOT_SYMMETRIC;
				case 4:
					return ActivationFunction.GAUSSIAN;
				case 5:
					return ActivationFunction.GAUSSIAN_STEPWISE;
				case 6:
					return ActivationFunction.GAUSSIAN_SYMMETRIC;
				case 7:
					return ActivationFunction.LINEAR;
				case 8:
					return ActivationFunction.LINEAR_PIECE;
				case 9:
					return ActivationFunction.LINEAR_PIECE_SYMMETRIC;
				case 10:
					return ActivationFunction.SIGMOID;
				case 11:
					return ActivationFunction.SIGMOID_STEPWISE;
				case 12:
					return ActivationFunction.SIGMOID_SYMMETRIC;
				case 13:
					return ActivationFunction.COS_SYMMETRIC;
				case 14:
					return ActivationFunction.SIN_SYMMETRIC;
				case 15:
					return ActivationFunction.THRESHOLD;
			}
		}
	}
}