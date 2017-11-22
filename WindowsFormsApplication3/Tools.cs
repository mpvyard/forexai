﻿using System.Reflection;
using System.Text;

namespace FinancePermutator
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text.RegularExpressions;
	using System.Windows.Forms;

	internal static class Tools
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern void OutputDebugString(string message);

		public static List<string> Messages = new List<string>();
		private static bool runningMessagePump;

		public static int LastLogTime { get; private set; }
		private static StreamWriter sw = null;
		public static object writeMessagesBlock = new object();

		public static void WriteMessages()
		{
			if (runningMessagePump)
				return;

			// hello
			lock (writeMessagesBlock)
			{
				if (Messages.Count == 0 || Program.Form.debugView == null)
					return;
				runningMessagePump = true;

				Program.Form.debugView.Invoke((MethodInvoker) (() =>
				{
					
					try
					{
						if (!File.Exists(Configuration.LogFileName))
							sw = File.CreateText(Configuration.LogFileName);
						else
							sw = File.AppendText(Configuration.LogFileName);
					}
					catch (Exception e)
					{
						OutputDebugString($"error log file open: {e}");
					}

					Program.Form.debugView.BeginUpdate();

					LastLogTime = DateTime.Now.Second;
					foreach (var msg in Messages)
					{
						Program.Form.debugView.Items.Add(msg);
						if (sw != null)
							sw.WriteLine(msg);
					}
					if (sw != null)
						sw.Close();
					Messages.Clear();

					int visibleItems = Program.Form.debugView.ClientSize.Height / Program.Form.debugView.ItemHeight;
					Program.Form.debugView.TopIndex = Math.Max(Program.Form.debugView.Items.Count - visibleItems + 1, 0);
					Program.Form.debugView.EndUpdate();
				}));
				runningMessagePump = false;
			}
		}

		public static void DumpValues(MethodInfo method, double[] returnData)
		{
			if (Program.Form.showValues.CheckState == CheckState.Checked)
			{
				// dump values
				var sb = new StringBuilder();
				var num = 0;

				foreach (var val in returnData)
				{
					sb.AppendFormat($"{val} ");
					if (num++ > 3)
					{
						num = 0;
						debug($"[{method.Name}] values {sb}");
						sb.Clear();
					}
				}
			}
		}

		public static bool IsArrayAllZeros(double[] input)
		{
			for (int i = 0; i < input.Length; i++)
				if (i != 0)
					return false;
			return true;
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
				// using (var logFile = File.AppendText(Configuration.LogFileName))
				// logFile.WriteLine($"{DateTime.Now:HH:mm:ss.fff} {s}");
				lock (writeMessagesBlock)
				{
					Messages.Add($"{DateTime.Now:HH:mm:ss.fff} {s}");
				}
			}
			catch
			{
				// ignored
				System.Diagnostics.Debug.WriteLine("error writing log file");
			}

			// OutputDebugString(s);
		}
	}
}