using System;
using System.Reflection;
using System.Windows.Forms;
using WindowsFormsApplication3.Forms;
using TicTacTec.TA.Library;

namespace WindowsFormsApplication3
{
    class Function
    {
        public readonly MethodInfo method;
        public int numData = 255;
        public int resultLen;
        public double[] Results = new double[1000];
        public int[] ResultsInt = new int[1000];
        private double[] ReturnData;

        public Function(MethodInfo mi, int num)
        {
            method = mi;
            numData = num;
        }

        public double[] Execute(FunctionParameters parameters)
        {
            var i = 0;
            var retCode =
                Core.RetCode.UnknownErr;

            try
            {
                retCode = (Core.RetCode)
                    method.Invoke(null, parameters.Get());
            }
            catch (Exception e)
            {
                Tools.deb("exception: " + e.Message);
            }

            Tools.deb("retCode: " + retCode);
            resultLen = (int)parameters.Get()[parameters.getoutNBElementIndex()];
            Tools.deb("resultLen: " + resultLen);
            ReturnData = new double[resultLen];

            if (parameters.Get()[parameters.getOutIndex()].GetType().ToString()
                == "System.Int32[]")
            {
                ResultsInt = (int[])parameters.Get()[parameters.getOutIndex()];
                for (i = 0; i < resultLen; i++)
                    Results[i] = ResultsInt[i];
            }
            else
            {
                Results = (double[])parameters.Get()[parameters.getOutIndex()];
            }

            Array.Copy(Results, ReturnData, resultLen);

            return ReturnData;
        }
    }
}