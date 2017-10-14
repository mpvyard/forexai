using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication3.Forms
{
	class FunctionParameters
	{
		Random random = new Random(DateTime.Now.Millisecond);
		object[] arguments;
		bool bOutReal = false;
		bool hasKnownOut = false;
		int paramIndex = 0;
		int index = 0;
		int numData = 255;
		int outIndex;
		int outBegIdx = 0;
		int outBegIdxIndex = 0;
		int outNBElement = 0;
		int outNBElementIndex = 0;
		int[] outInteger = new int[1000];
		double[] outReal = new double[1000];
		string functionName = "";

		public object getoutReal()
		{
			return arguments[outIndex];
		}

		public int getoutNBElementIndex()
		{
			return outNBElementIndex;
		}
		public int getOutBeginIndex()
		{
			return outBegIdx;
		}

		public int getNBElement()
		{
			return outNBElement;
		}

		public int getOutIndex()
		{
			return outIndex;
		}

		public FunctionParameters(MethodInfo mi)
		{
			functionName = mi.Name;
			Prices prices = new Prices();

			DumpParams(mi);

			arguments = new object[mi.GetParameters().Count()];

			foreach (ParameterInfo param in mi.GetParameters())
			{
				switch (param.Name)
				{
					case "optInVFactor":
						arguments[paramIndex] = 0;
						break;
					case "outMACDSignal":
						arguments[paramIndex] = 0;
						break;
					case "outMACDHist":
						arguments[paramIndex] = 0;
						break;

					case "outMin":
						arguments[paramIndex] = 0;
						break;
					case "optInNbDevUp":
						arguments[paramIndex] = 0.0;
						break;
					case "outMACD":
						arguments[paramIndex] = 0.0;
						break;
					case "outLeadSine":
						arguments[paramIndex] = 0.0;
						break;
					case "outSine":
						arguments[paramIndex] = 0.0;
						break;
					case "optInMinPeriod":
					case "optInMaxPeriod":
					case "optInSignalPeriod":
						arguments[paramIndex] = MATypeGen.GetRand();
						Tools.deb("optInSignalPeriod=" + arguments[paramIndex]);
						break;
					case "optInMAType":
						arguments[paramIndex] = MATypeGen.GetRand();
						Tools.deb("optInMAType=" + arguments[paramIndex]);
						break;
					case "optInNbDev":
						arguments[paramIndex] = (double)0.0;
						break;
					case "inReal0":
					case "inReal1":
					case "inReal":
						index = random.Next(3);
						switch (index)
						{
							case 0:
								arguments[paramIndex] = prices.getOpen(numData);
								break;
							case 1:
								arguments[paramIndex] = prices.getClose(numData);
								break;
							case 2:
								arguments[paramIndex] = prices.getHigh(numData);
								break;
							case 3:
								arguments[paramIndex] = prices.getLow(numData);
								break;
						}
						Tools.deb("inReal: " + index);

						break;
					case "optInMaximum":
						arguments[paramIndex] = (double)0.0;
						break;
					case "optInSlowD_MAType":
					case "optInFastD_MAType":
					case "optInSlowK_MAType":
						arguments[paramIndex] = MATypeGen.GetRand();//MAType.Ema;
						Tools.deb("optMAtype=" + arguments[paramIndex]);
						break;
					case "optInAccelerationShort":
					case "optInAccelerationMaxShort":
					case "optInAccelerationInitShort":
					case "optInAccelerationMaxLong":
					case "optInAccelerationLong":
					case "optInAccelerationInitLong":
					case "optInAcceleration":
						arguments[paramIndex] = (double)0.0;
						break;

					case "optInOffsetOnReverse":
						arguments[paramIndex] = (int)0;
						break;
					case "optInSlowK_Period":
					case "optInFastK_Period":
					case "optInSlowD_Period":
					case "optInFastD_Period":
						arguments[paramIndex] = (int)MAGen.GetRand();
						Tools.deb("optPeriod=" + arguments[paramIndex]);
						break;
					case "optInSlowPeriod":
					case "optInFastPeriod":
						arguments[paramIndex] = (int)MAGen.GetRand();
						Tools.deb("optPeriod=" + arguments[paramIndex]);
						break;

					case "optInTimePeriod1":
					case "optInTimePeriod3":
					case "optInTimePeriod2":
					case "optInTimePeriod":
						arguments[paramIndex] = (int)MAGen.GetRand();
						Tools.deb("optPeriod=" + arguments[paramIndex]);
						break;
					case "optInPenetration":
						arguments[paramIndex] = (int)0;
						break;
					case "optInStartValue":
						arguments[paramIndex] = (int)0;
						break;
					case "startIdx":
						arguments[paramIndex] = (int)0;
						break;
					case "endIdx":
						arguments[paramIndex] = (int)numData - 1;
						break;
					case "inOpen":
						arguments[paramIndex] = prices.getOpen(numData);
						break;
					case "inHigh":
						arguments[paramIndex] = prices.getHigh(numData);
						break;
					case "inLow":
						arguments[paramIndex] = prices.getLow(numData);
						break;
					case "inClose":
						arguments[paramIndex] = prices.getClose(numData);
						break;
					case "inVolume":
						arguments[paramIndex] = prices.getVol(numData);
						break;
					case "outBegIdx":
						arguments[paramIndex] = outBegIdx;
						outBegIdxIndex = paramIndex;
						break;
					case "outNBElement":
						arguments[paramIndex] = outNBElement;
						outNBElementIndex = paramIndex;
						break;
					case "outInteger":
						arguments[paramIndex] = outInteger;
						outIndex = paramIndex;
						bOutReal = false;
						hasKnownOut = true;
						break;
					case "outReal":
						hasKnownOut = true;
						arguments[paramIndex] = outReal;
						bOutReal = true;
						outIndex = paramIndex;
						break;
					case "outAroonUp":
						arguments[paramIndex] = new double[1000];
						break;
					case "outAroonDown":
						arguments[paramIndex] = new double[1000];
						break;
					case "outSlowD":
					case "outSlowK":
					case "outFastD":
					case "outFastK":
						arguments[paramIndex] = new double[1000];
						break;

					default:
						Tools.deb("nothing found for " + param.Name);
						break;
				}
				paramIndex++;
			}
			Tools.deb("arguments: " + arguments.Count());
			DumpArguments();
		}

		public object[] Get()
		{
			return arguments;
		}

		void DumpArguments()
		{
			int argIdx = 0;
			foreach (object o in arguments)
			{
				Tools.deb(" arg" + (argIdx++) + " " + o.GetType());
			}
		}

		void DumpParams(MethodInfo methodInfo)
		{
			foreach (ParameterInfo pi in methodInfo.GetParameters())
			{
				Tools.deb(" param " + pi.ParameterType + " " + pi.Name);
			}
		}
	}
}
