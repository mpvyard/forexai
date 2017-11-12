using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TicTacTec.TA.Library;
using static FinancePermutator.Tools;

namespace FinancePermutator.Function
{
	internal class Function
	{
		private MethodInfo method;
		private int resultLen;
		private double[] results = new double[1000];
		private int[] resultsInt = new int[1000];
		private double[] returnData;

		public Function(MethodInfo mi)
		{
			method = mi;
		}

		public double[] Execute(FunctionParameters parameters, out Core.RetCode code)
		{
			Core.SetCompatibility(Core.Compatibility.Metastock);
			//Core.SetUnstablePeriod(Core.FuncUnstId.FuncUnstAll, 255);

			var retCode = Core.RetCode.UnknownErr;
			try
			{
				retCode = (Core.RetCode) method.Invoke(null, parameters.Arguments);
			}
			catch (Exception e)
			{
				debug($"exception: {e.Message}");
			}

			resultLen = (int) parameters.Arguments[parameters.OutNbElement];
			//debug($"retCode: {retCode} resultLen: {resultLen}");
			code = retCode;
			returnData = new double[resultLen];

			if (parameters.Arguments[parameters.OutIndex].GetType().ToString() == "System.Int32[]")
			{
				resultsInt = (int[]) parameters.Arguments[parameters.OutIndex];

				for (var i = 0; i < resultLen; i++)
					results[i] = resultsInt[i];
			}
			else
				results = parameters.Arguments[parameters.OutIndex] as double[];

			if (results != null)
				Array.Copy(results, returnData, resultLen);

			DumpValues(method, returnData);

			return returnData;
		}
	}
}