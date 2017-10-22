using System;
using System.Reflection;
using FinancePermutator.Generators;

namespace FinancePermutator.Function
{
	class FunctionParameters
	{
		public Random Random;
		public int ParamIndex;
		public int NumData;
		public int OutBegIdx = 0;
		public int Offset;
		public int[] OutInteger = new int[1000];
		public double[] OutReal = new double[1000];

		public int RandomSeed { get; }

		public object[] Arguments { get; }

		public int OutIndex { get; }

		public int OutNbElement { get; }

		/*░░ ♡ ▄▀▀▀▄░░░ 
		▄███▀░◐░░░▌░░░░░░░ 
		░░░░▌░░░░░▐░░░░░░░ 
		░░░░▐░░░░░▐░░░░░░░ 
		░░░░▌░░░░░▐▄▄░░░░░ 
		░░░░▌░░░░▄▀▒▒▀▀▀▀▄ 
		░░░▐░░░░▐▒▒▒▒▒▒▒▒▀▀▄ 
		░░░▐░░░░▐▄▒▒▒▒▒▒▒▒▒▒▀▄ 
		░░░░▀▄░░░░▀▄▒▒▒▒▒▒▒▒▒▒▀▄ 
		░░░░░░▀▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▀▄ 
		░░░░░░░░░░░▌▌░▌▌░░░░░ 
		░░░░░░░░░░░▌▌░▌▌░░░░░ 
		░░░░░░░░░▄▄▌▌▄▌▌░░░░░*/
		public FunctionParameters(MethodInfo methodInfo, int numdata, int offset, int randomSeed = 0)
		{
			NumData = numdata;
			Offset = offset;
			RandomSeed = randomSeed > 0 ? randomSeed : DateTime.Now.Millisecond;
			Random = new Random(RandomSeed);

			Program.Form.ConfigurationAddLine($"Function {methodInfo.Name}\r\n");

			// DumpParams(methodInfo);
			Arguments = new object[methodInfo.GetParameters().Length];

			//debug($"function method {methodInfo.Name} offset {offset} numdata {NumData} randomSeed {randomSeed}");

			foreach (ParameterInfo param in methodInfo.GetParameters())
			{
				switch (param.Name)
				{
					case "optInVFactor":
						Arguments[ParamIndex] = 0;
						break;
					case "outMACDSignal":
						Arguments[ParamIndex] = 0;
						break;
					case "outMACDHist":
						Arguments[ParamIndex] = 0;
						break;

					case "outMin":
						Arguments[ParamIndex] = 0;
						break;
					case "optInNbDevUp":
						Arguments[ParamIndex] = 0.0;
						break;
					case "outMACD":
						Arguments[ParamIndex] = 0.0;
						break;
					case "outLeadSine":
						Arguments[ParamIndex] = 0.0;
						break;
					case "outSine":
						Arguments[ParamIndex] = 0.0;
						break;
					case "optInMinPeriod":
					case "optInMaxPeriod":
					case "optInSignalPeriod":
						Arguments[ParamIndex] = MaTypeGen.GetRandom();

						// debug($"{param.Name} optInSignalPeriod=" + arguments[paramIndex]);
						break;
					case "optInMAType":
						Arguments[ParamIndex] = MaTypeGen.GetRandom();

						// debug($"{param.Name} optInMAType=" + arguments[paramIndex]);
						break;
					case "optInNbDev":
						Arguments[ParamIndex] = 0.0;
						break;
					case "inReal0":
					case "inReal1":
					case "inReal":
						var index = Random.Next(3);
						switch (index)
						{
							case 0:
								Arguments[ParamIndex] = ForexPrices.GetOpen(NumData, Offset);
								break;
							case 1:
								Arguments[ParamIndex] = ForexPrices.GetClose(numdata, Offset);
								break;
							case 2:
								Arguments[ParamIndex] = ForexPrices.GetHigh(NumData, Offset);
								break;
							case 3:
								Arguments[ParamIndex] = ForexPrices.GetLow(NumData, Offset);
								break;
						}

						//debug($"real {param.Name}[0]: " + ((double[])Arguments[ParamIndex])[0]);
						break;
					case "optInMaximum":
						Arguments[ParamIndex] = 0.0;
						break;
					case "optInSlowD_MAType":
					case "optInFastD_MAType":
					case "optInSlowK_MAType":
						Arguments[ParamIndex] = MaTypeGen.GetRandom();

						// debug($"{param.Name} optMAtype=" + arguments[paramIndex]);
						break;
					case "optInAccelerationShort":
					case "optInAccelerationMaxShort":
					case "optInAccelerationInitShort":
					case "optInAccelerationMaxLong":
					case "optInAccelerationLong":
					case "optInAccelerationInitLong":
					case "optInAcceleration":
						Arguments[ParamIndex] = 0.0;
						break;

					case "optInOffsetOnReverse":
						Arguments[ParamIndex] = 0;
						break;
					case "optInSlowK_Period":
					case "optInFastK_Period":
					case "optInSlowD_Period":
					case "optInFastD_Period":
						Arguments[ParamIndex] = MaGen.GetRandom();

						// debug($"{param.Name} optDKPeriod=" + arguments[paramIndex]);
						break;
					case "optInSlowPeriod":
					case "optInFastPeriod":
						Arguments[ParamIndex] = MaGen.GetRandom();

						// debug($"{param.Name} optSlowFastPeriod=" + arguments[paramIndex]);
						break;

					case "optInTimePeriod1":
					case "optInTimePeriod3":
					case "optInTimePeriod2":
					case "optInTimePeriod":
						Arguments[ParamIndex] = MaGen.GetRandom();

						// debug($"{param.Name} optInTimePeriod=" + arguments[paramIndex]);
						break;
					case "optInPenetration":
						Arguments[ParamIndex] = 0;
						break;
					case "optInStartValue":
						Arguments[ParamIndex] = 0;
						break;
					case "startIdx":
						Arguments[ParamIndex] = 0;
						break;
					case "endIdx":
						Arguments[ParamIndex] = NumData - 1;
						break;
					case "inOpen":
						Arguments[ParamIndex] = ForexPrices.GetOpen(NumData, Offset);
						break;
					case "inHigh":
						Arguments[ParamIndex] = ForexPrices.GetHigh(NumData, Offset);
						break;
					case "inLow":
						Arguments[ParamIndex] = ForexPrices.GetLow(NumData, Offset);
						break;
					case "inClose":
						Arguments[ParamIndex] = ForexPrices.GetClose(NumData, Offset);
						break;
					case "inVolume":
						Arguments[ParamIndex] = ForexPrices.GetVolume(NumData, Offset);
						break;
					case "outBegIdx":
						Arguments[ParamIndex] = this.OutBegIdx;
						break;
					case "outNBElement":
						Arguments[ParamIndex] = OutNbElement;
						OutNbElement = ParamIndex;
						break;
					case "outInteger":
						Arguments[ParamIndex] = OutInteger;
						OutIndex = ParamIndex;
						break;
					case "outReal":
						Arguments[ParamIndex] = OutReal;
						OutIndex = ParamIndex;
						break;
					case "outAroonUp":
						Arguments[ParamIndex] = new double[1000];
						break;
					case "outAroonDown":
						Arguments[ParamIndex] = new double[1000];
						break;
					case "outSlowD":
					case "outSlowK":
					case "outFastD":
					case "outFastK":
						Arguments[ParamIndex] = new double[1000];
						break;

					default:
						Tools.debug($"nothing found for {param.Name}");
						break;
				}

				ParamIndex++;

				Program.Form.ConfigurationAddLine($"{ParamIndex} {param.Name}: {Arguments[ParamIndex - 1]}\r\n");
			}

			// 	DumpArguments();
		}

		private void DumpArguments()
		{
			Tools.debug($"arguments: {Arguments.Length}");
			int argIdx = 0;
			foreach (object o in Arguments)
				if (o != null)
					Tools.debug($" +arg{argIdx++,-2:00} {o.GetType()} {o}");
		}

		private static void DumpParams(MethodInfo methodInfo)
		{
			int idx = 0;
			foreach (var pi in methodInfo.GetParameters())
				Tools.debug($" prm{idx++,2:00} {pi.ParameterType} {pi.Name}");
		}
	}
}