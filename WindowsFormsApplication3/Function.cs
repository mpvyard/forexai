using System;
using System.Reflection;
using TicTacTec.TA.Library;

namespace WindowsFormsApplication3
{
    internal class Function
    {
        public MethodInfo Method;
        public int NumData;
        public int ResultLen;
        public double[] Results = new double[1000];
        public int[] ResultsInt = new int[1000];
        private double[] returnData;

        public Function(MethodInfo mi, int num)
        {
            Method = mi;
            NumData = num;
        }

        public double[] Execute(FunctionParameters parameters)
        {
            Core.RetCode retCode = Core.RetCode.UnknownErr;
            try
            {
                retCode = (Core.RetCode)
                   Method.Invoke(null, parameters.Get);
            }
            catch (Exception e)
            {
                Tools.Debug($"exception: {e.Message}");
            }

            Tools.Debug($"retCode: {retCode}");

            ResultLen = (int)parameters.Get[parameters.GetoutNbElementIndex()];

            Tools.Debug($"resultLen: {ResultLen}");

            this.returnData = new double[ResultLen];

            if (parameters.Get[parameters.GetOutIndex()].GetType().ToString() == "System.Int32[]")
            {
                ResultsInt = (int[])parameters.Get[parameters.GetOutIndex()];

                for (var i = 0; i < ResultLen; i++)
                    Results[i] = ResultsInt[i];
            }
            else
            {
                Results = parameters.Get[parameters.GetOutIndex()] as double[];
            }

            if (Results != null) Array.Copy(Results, this.returnData, ResultLen);

            return this.returnData;
        }
    }
}