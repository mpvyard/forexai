using System;
using System.Linq;
using System.Reflection;

namespace WindowsFormsApplication3
{
    class FunctionParameters
    {
        private Random random = new Random(DateTime.Now.Millisecond);
        object[] arguments;
        public bool BOutReal;
        public bool HasKnownOut;
        int paramIndex;
        int numData = 255;
        int outIndex;
        int outBegIdx = 0;
        int outNbElement = 0;
        int outNbElementIndex;
        int[] outInteger = new int[1000];
        double[] outReal = new double[1000];

        public object GetoutReal()
        {
            return arguments[outIndex];
        }

        public int GetoutNbElementIndex()
        {
            return outNbElementIndex;
        }

        public int GetOutBeginIndex()
        {
            return outBegIdx;
        }

        public int GetNbElement()
        {
            return outNbElement;
        }

        public int GetOutIndex()
        {
            return outIndex;
        }

        public FunctionParameters(MethodInfo mi)
        {
            HasKnownOut = false;
            var unused = new Prices();

            DumpParams(mi);

            arguments = new object[mi.GetParameters().Length];

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
                        arguments[paramIndex] = MaTypeGen.GetRandom();
                        Tools.Debug("optInSignalPeriod=" + arguments[paramIndex]);
                        break;
                    case "optInMAType":
                        arguments[paramIndex] = MaTypeGen.GetRandom();
                        Tools.Debug("optInMAType=" + arguments[paramIndex]);
                        break;
                    case "optInNbDev":
                        arguments[paramIndex] = (double)0.0;
                        break;
                    case "inReal0":
                    case "inReal1":
                    case "inReal":
                        var index = random.Next(3);
                        switch (index)
                        {
                            case 0:
                                arguments[paramIndex] = Prices.GetOpen(numData);
                                break;
                            case 1:
                                arguments[paramIndex] = Prices.GetClose(numData);
                                break;
                            case 2:
                                arguments[paramIndex] = Prices.GetHigh(numData);
                                break;
                            case 3:
                                arguments[paramIndex] = Prices.GetLow(numData);
                                break;
                        }
                        Tools.Debug("inReal: " + index);

                        break;
                    case "optInMaximum":
                        arguments[paramIndex] = 0.0;
                        break;
                    case "optInSlowD_MAType":
                    case "optInFastD_MAType":
                    case "optInSlowK_MAType":
                        arguments[paramIndex] = MaTypeGen.GetRandom();//MAType.Ema;
                        Tools.Debug("optMAtype=" + arguments[paramIndex]);
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
                        arguments[paramIndex] = (int)MaGen.GetRand();
                        Tools.Debug("optDKPeriod=" + arguments[paramIndex]);
                        break;
                    case "optInSlowPeriod":
                    case "optInFastPeriod":
                        arguments[paramIndex] = (int)MaGen.GetRand();
                        Tools.Debug("optSlowFastPeriod=" + arguments[paramIndex]);
                        break;

                    case "optInTimePeriod1":
                    case "optInTimePeriod3":
                    case "optInTimePeriod2":
                    case "optInTimePeriod":
                        arguments[paramIndex] = (int)MaGen.GetRand();
                        Tools.Debug("optInTimePeriod=" + arguments[paramIndex]);
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
                        arguments[paramIndex] = Prices.GetOpen(numData);
                        break;
                    case "inHigh":
                        arguments[paramIndex] = Prices.GetHigh(numData);
                        break;
                    case "inLow":
                        arguments[paramIndex] = Prices.GetLow(numData);
                        break;
                    case "inClose":
                        arguments[paramIndex] = Prices.GetClose(numData);
                        break;
                    case "inVolume":
                        arguments[paramIndex] = Prices.GetVolume(numData);
                        break;
                    case "outBegIdx":
                        arguments[paramIndex] = outBegIdx;
                        break;
                    case "outNBElement":
                        arguments[paramIndex] = outNbElement;
                        outNbElementIndex = paramIndex;
                        break;
                    case "outInteger":
                        arguments[paramIndex] = outInteger;
                        outIndex = paramIndex;
                        BOutReal = false;
                        HasKnownOut = true;
                        break;
                    case "outReal":
                        HasKnownOut = true;
                        arguments[paramIndex] = outReal;
                        BOutReal = true;
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
                        Tools.Debug($"nothing found for {param.Name}");
                        break;
                }
                paramIndex++;
            }
            DumpArguments();
        }

        public object[] Get => arguments;

        private void DumpArguments()
        {
            Tools.Debug($"arguments: {arguments.Length}");
            int argIdx = 0;
            foreach (object o in arguments)
                if (o != null)
                    Tools.Debug($" +arg{argIdx++,-2:00} {o.GetType()} {o}");
        }

        private static void DumpParams(MethodInfo methodInfo)
        {
            int idx = 0;
            foreach (var pi in methodInfo.GetParameters())
                Tools.Debug($" prm{idx++,2:00} {pi.ParameterType} {pi.Name}");
        }
    }
}
