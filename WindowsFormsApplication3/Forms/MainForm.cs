﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FinancePermutator.Prices;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using FANNCSharp.Double;
using FinancePermutator.Generators;
using static FANNCSharp.TrainingAlgorithm;
using FinancePermutator.Networks;
using static FinancePermutator.Tools;
using TicTacTec.TA.Library;

namespace FinancePermutator.Forms
{
	/// <summary>
	/// Main form
	/// </summary>
	public partial class Form1 : Form
	{
		int methodNum;
		Train.Train threadProcessScan;
		internal bool DoingSearch;

		[DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
		static extern int GetCurrentThreadId();

		public void ConfigurationClear()
		{
			Program.Form.chart.Invoke((MethodInvoker) (() => { Program.Form.configurationTab.Clear(); }));
		}

		public void AddConfiguration(string text)
		{
			Program.Form.chart.Invoke((MethodInvoker) (() => { Program.Form.configurationTab.AppendText(text); }));
		}

		public Form1()
		{
			try
			{
				File.Delete(Configuration.LogFileName);
			}
			catch (Exception e)
			{
				debug($"exception while delete: {e}");
			}
			InitializeComponent();
			XRandom.init();
		}

		private void LoadPricesButtonClick(object sender, EventArgs e)
		{
			loadPricesButton.Text = @"loading";
			LoadPrices.Exec();
		}

		private void Form1Resize(object sender, EventArgs e)
		{
			switch (WindowState)
			{
				case FormWindowState.Minimized:
					notifyIcon1.Visible = true;
					notifyIcon1.BalloonTipTitle = "FINANCE PERMUTATOR";
					notifyIcon1.BalloonTipText = $"running in background, pid={Process.GetCurrentProcess().Id} tid={GetCurrentThreadId()}";
					notifyIcon1.ShowBalloonTip(1500);

					notifyIcon1.Text = @"[Message shown when hovering over tray icon]";
					Hide();
					break;
				case FormWindowState.Normal:
					notifyIcon1.Visible = false;
					break;
			}
		}

		private void TrayClick(object sender, MouseEventArgs e)
		{
			debug("tray click");
			Show();
			WindowState = FormWindowState.Normal;
			notifyIcon1.Visible = false;
		}

		private void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			debug($"checkbox: {showValues.CheckState == CheckState.Checked}");
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
		private static bool IsGoodMethod(MethodInfo method)
		{
			/*if (method.ToString().Contains("Cdl"))
				return false;*/
			if (method.ToString().Contains("Single[]"))
				return false;
			if (method.ToString().Contains("Lookback"))
				return false;

			bool bAddMethod = false;

			if (!CheckFunctionParams(method, ref bAddMethod))
				return false;

			return bAddMethod;
		}

		private static bool CheckFunctionParams(MethodInfo method, ref bool bAddMethod)
		{
			foreach (var param in method.GetParameters())
			{
				// debug($" param: {param.Name} {param.ParameterType}");
				if (param.Name.Contains("inReal") || param.Name.Contains("inOpen") || param.Name.Contains("inClose") || param.Name.Contains("inLow") ||
				    param.Name.Contains("inHigh"))
					bAddMethod = true;

				if (param.Name.Contains("outFast") || param.Name.Contains("outSine") || param.Name.Contains("outMACD") || param.Name.Contains("outMAMA") ||
				    param.Name.Contains("outSlowK") || param.Name.Contains("outAroonDown") || param.Name.Contains("inPeriods") || param.Name.Contains("outMin") ||
				    param.Name.Contains("outInPhase"))
					return false;
			}

			return true;
		}

		private void LoadTaLib(object sender, EventArgs e)
		{
			debug("Harvesing ta-lib");
			methodNum = 0;

			Data.TALibMethods.Clear();
			var methodInfos = typeof(Core).GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (var method in methodInfos)
				if (IsGoodMethod(method))
				{
					Data.TALibMethods.Add(method);
					debug($"-> method #{this.methodNum++,-3:000}: {method}");
					Program.Form.loadTAButton.Invoke((MethodInvoker) (() => Program.Form.loadTAButton.Text = $"loading {this.methodNum}"));
				}
			Program.Form.loadTAButton.Text = $"{Data.TALibMethods.Count} OK";

			if (Data.ForexPrices.Any())
				Program.Form.buttonExecute.Enabled = true;
		}

		private void TimeFastTick(object sender, EventArgs e)
		{
			lock (writeMessagesBlock)
			{
				if (Messages.Any())
					WriteMessages();
			}
		}

		internal static void DrawResults(string name, double[] data, bool clearCurrent = true)
		{
			// if (Program.Form.chart.Series.IndexOf(name) != -1)
			// return;
			//debug($"draw name '{name}'");

			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				if (clearCurrent)
					Program.Form.chart.Series.Clear();

				if (Program.Form.chart.Series.IndexOf(name) == -1)
					Program.Form.chart.Series.Add(name);

				Program.Form.chart.Series[name].ChartType = SeriesChartType.Line;

				foreach (var v in data)
					Program.Form.chart.Series[name].Points.AddXY("xxx", v);
			}));
		}

		public void setStatus(string status)
		{
			Program.Form.statusLabel.Invoke((MethodInvoker) (() => { Program.Form.statusLabel.Text = status; }));
		}

		private void GenerateTrainButtonClick(object sender, EventArgs e)
		{
			switch (DoingSearch)
			{
				case true:
					threadProcessScan.RunScan = false;

					// threadProcessScan.Stop();
					debug("stop scan");
					DoingSearch = false;
					break;
				case false:
					threadProcessScan = new Train.Train();
					threadProcessScan.Start();
					DoingSearch = true;
					break;
			}
		}

		private void ChartClick(object sender, EventArgs e)
		{ }

		private void CreateNetworkButtonClick(object sender, EventArgs e)
		{
			chart.Series.Clear();
			debugView.Items.Clear();
		}

		private void EnumerateNetworkInterfaces()
		{
			foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
					continue;
				debug($"interface: {ni.Name}");
				foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
					if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
						debug($"ip: {ip.Address}");
			}
		}

		private void ExecuteButtonClick(object sender, EventArgs e)
		{
			EnumerateNetworkInterfaces();
			debugView.Items.Clear();
		}

		private void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			debug("notify dbl click");
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

			// base.OnPaint(e);
		}

		private void DebugView_Draw(object sender, DrawItemEventArgs e)
		{
			if (e.Index <= 0)
				return;

			// Draw the background.
			e.DrawBackground();

			string line = this.debugView.Items[e.Index] as string;

			// See if the item is selected.
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
			{
				// Selected. Draw with the system highlight color.
				e.Graphics.DrawString(line, new Font("lucida console", 8, FontStyle.Regular), SystemBrushes.HighlightText, e.Bounds.Left, e.Bounds.Top + 1);
			}
			else
			{
				var brush = new SolidBrush(Color.DarkSlateBlue);
				if (line.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0 || line.IndexOf("Exception", StringComparison.OrdinalIgnoreCase) >= 0)
					brush = new SolidBrush(Color.Red);
				else if (line.IndexOf("values", StringComparison.OrdinalIgnoreCase) >= 0)
					brush = new SolidBrush(Color.MediumSeaGreen);
				else if (line.IndexOf("WARNING", StringComparison.OrdinalIgnoreCase) >= 0)
					brush = new SolidBrush(Color.DarkOrange);

				using (SolidBrush br = brush)
				{
					e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
					e.Graphics.DrawString(line, new Font("lucida console", 8, FontStyle.Regular), br, e.Bounds.Left, e.Bounds.Top);
				}
			}

			// Draw the focus rectangle if appropriate.
			e.DrawFocusRectangle();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			debug("Form1 LOAD");
		}

		public void EraseBigLabel()
		{
			Program.Form.chart.Invoke((MethodInvoker)(() =>
			{
				Program.Form.chart.Series.Clear();
			}));
			Program.Form.debugView.Invoke((MethodInvoker)(() => { Data.chartBigLabel = string.Empty; }));
		}

		public void setBigLabel(string text)
		{
			Program.Form.debugView.Invoke((MethodInvoker)(() =>
			{
				Data.chartBigLabel = text.Length > 0 ? text : $"[MUTATING DATA {Data.loadPercent,4:####}%]";
			}));
		}

		private void chart_PostPaint(object sender, ChartPaintEventArgs e)
		{
			if (Data.chartBigLabel.Length <= 1)
				return;

			//MessageBox.Show($"paint {Data.chartBigLabel}");

			Font drawFont = new Font("Consolas", 15);
			SolidBrush drawBrush = new SolidBrush(Color.BlueViolet);

			// Create rectangle for drawing.
			float x = 100.0F;
			float y = 100.0F;
			float width = 300.0F;
			float height = 250.0F;
			RectangleF drawRect = new RectangleF(x, y, width, height);
		
			// Draw rectangle to screen.
			Pen blackPen = new Pen(Color.Black);
			e.ChartGraphics.Graphics.DrawRectangle(blackPen, x, y, width, height);

			// Set format of string.
			StringFormat drawFormat = new StringFormat();
			drawFormat.Alignment = StringAlignment.Center;
			e.ChartGraphics.Graphics.DrawString(Data.chartBigLabel, drawFont, drawBrush, drawRect, drawFormat);
		}
	}
}