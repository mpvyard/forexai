using System.Reflection;
using System.Text;

namespace FinancePermutator
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	internal static class Tools
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern void OutputDebugStringA(string message);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetCurrentProcessId();
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetCurrentThreadId();

		public static List<string> Messages = new List<string>();
		private static bool runningMessagePump;

		public static int LastLogTime { get; private set; }
		private static StreamWriter sw = null;
		public static object writeMessagesBlock = new object();

		public static void WriteMessages()
		{
			if(runningMessagePump)
			{
				return;
			}

			lock(writeMessagesBlock)
			{
				if(Messages.Count == 0 || Program.Form.debugView == null)
				{
					return;
				}

				runningMessagePump = true;

				Program.Form.debugView.Invoke((MethodInvoker) (() =>
				 {
					 try
					 {
						 if(!File.Exists(Configuration.LogFileName))
							 sw = File.CreateText(Configuration.LogFileName);
						 else
							 sw = File.AppendText(Configuration.LogFileName);
					 }
					 catch(Exception e)
					 {
						 OutputDebugStringA($"error log file open: {e}");
					 }

					 LastLogTime = DateTime.Now.Second;
					 object[] tempItems = new object[Messages.Count];
					 int i = 0;

					 foreach(string msg in Messages)
					 {
						 tempItems[i++] = msg;
						 if(sw != null)
							 sw.WriteLine(msg);
					 }

					 Program.Form.debugView.Items.AddRange(tempItems);

					 if(sw != null)
						 sw.Close();

					 Messages.Clear();

					 int visibleItems = Program.Form.debugView.ClientSize.Height / Program.Form.debugView.ItemHeight;
					 Program.Form.debugView.TopIndex = Math.Max(Program.Form.debugView.Items.Count - visibleItems + 1, 0);
				 }));
				runningMessagePump = false;
			}
		}

		public static void DumpValues(MethodInfo method, double[] returnData)
		{
			if(Program.Form.showValues.CheckState == CheckState.Checked)
			{
				// dump values
				var sb = new StringBuilder();
				var num = 0;

				foreach(var val in returnData)
				{
					sb.AppendFormat($"{val} ");
					if(num++ > 3)
					{
						num = 0;
						debug($"[{method.Name}] values {sb}");
						sb.Clear();
					}
				}
			}
		}

		public static bool IsArrayRepeating(double[] input)
		{
			double value = input[0];
			for(int i = 1; i < input.Length; i++)
			{
				if(input[i] != value)
				{
					return false;
				}
			}

			return true;
		}

		public static string GetTempPath()
		{
			string tmp = Configuration.TempPath;
			return Environment.ExpandEnvironmentVariables(tmp);
		}

		/*
						░░ ♡ ▄▀▀▀▄░░░ 
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
		public static void debug(string s)
		{
			System.Diagnostics.Debug.WriteLine(s);
			try
			{
				lock(writeMessagesBlock)
				{
					var msg = $"{DateTime.Now:HH:mm:ss.fff} {GetCurrentProcessId():x04}|{GetCurrentThreadId():x04} {s}";
					using(var logFile = File.AppendText(Configuration.LogFileName))
						logFile.WriteLine(msg);

					Messages.Add(msg);
				}
			}
			catch
			{
				// ignor
				System.Diagnostics.Debug.WriteLine("error writing log file");
			}

			// OutputDebugString(s);
		}
	}
}