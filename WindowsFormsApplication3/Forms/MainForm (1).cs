using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacTec.TA.Library;
using WindowsFormsApplication3.Forms;

namespace WindowsFormsApplication3
{
	public partial class Form1 : Form
	{
		Random random = new Random();

		public Form1()
		{
			InitializeComponent();
		}

		private void loadClick(object sender, EventArgs e)
		{
			loadPricesButton.Text = "loading";
			LoadPrices tc = new LoadPrices();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if(FormWindowState.Minimized == this.WindowState)
			{
				this.notifyIcon1.Visible = true;
				//this.notifyIcon1.ShowBalloonTip(500);
				this.notifyIcon1.BalloonTipText = "sdf";
				this.notifyIcon1.Text = "[Message shown when hovering over tray icon]";
				this.Hide();
			}
			else if(FormWindowState.Normal == this.WindowState)
			{
				this.notifyIcon1.Visible = false;
			}
		}

		private void trayClick(object sender, MouseEventArgs e)
		{
			Show();
			this.WindowState = FormWindowState.Normal;
			this.notifyIcon1.Visible = false;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			Tools.deb("checkbox: " + (checkBox1.CheckState == CheckState.Checked));
		}

		public static string GetParamName(MethodInfo method, int index)
		{
			string retVal = string.Empty;

			if(method != null && method.GetParameters().Length > index)
				retVal = method.GetParameters()[index].Name;

			return retVal;
		}

		private void LoadTALib(object sender, EventArgs e)
		{
			Boolean addMethod = false;
			object[] one = new object[1000];
			int methodNum = 0;
			MethodInfo[] methodInfos;

			Tools.deb("Harvesing ta-lib");

			Program.form.loadTAButton.Invoke((MethodInvoker) (()
				 => Program.form.loadTAButton.Text = "examining"));

			Program.taMethods.Clear();

			methodInfos = typeof(Core).GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach(var method in methodInfos)
			{
				addMethod = false;

				if(method.ToString().Contains("Cdl"))
					continue;
				if(method.ToString().Contains("Single[]"))
					continue;
				if(method.ToString().Contains("Lookback"))
					continue;

				foreach(var param in method.GetParameters())
				{
					if(param.Name.Contains("inReal") || param.Name.Contains("inOpen") ||
						 param.Name.Contains("inClose") ||
						 param.Name.Contains("inLow") || param.Name.Contains("inHigh"))
						addMethod = true;


					if(param.Name.Contains("outFast") || param.Name.Contains("outSine") ||
						param.Name.Contains("outMACD") || param.Name.Contains("outMAMA") ||
						param.Name.Contains("outSlowK") || param.Name.Contains("outAroonDown") ||
						param.Name.Contains("inPeriods") || param.Name.Contains("outMin") ||
						param.Name.Contains("outInPhase"))
						addMethod = false;
					Tools.deb(" param: " + param.Name + " " + param.ParameterType);
				}

				if(addMethod)
				{
					methodNum++;
					Program.taMethods.Add(method);
					Tools.deb("method #" + methodNum + ": " + method);
				}
			}
			Program.form.loadTAButton.Text = Program.taMethods.Count + " OK";
		}

		private void timeFast_Tick(object sender, EventArgs e)
		{


		}

		public void DrawResults(string name, double[] data)
		{
            if (Program.form.chart1.Series.IndexOf(name) > 0)
                return;

			Program.form.chart1.Series.Add(name);

			foreach(var v in data)
			{
				Tools.deb("v: " + v);
				Program.form.chart1.Invoke((MethodInvoker) (()
					=> { Program.form.chart1.Series[name].Points.AddXY("xxx", v); }));
			}
		}

		private void GenTrainClick(object sender, EventArgs e)
		{
			if(Program.taMethods.Count() == 0)
				return;

			MethodInfo mi = Program.taMethods[random.Next(Program.taMethods.Count - 1)];
			Tools.deb("selected function: " + mi.Name);
			FunctionParameters parameters = new FunctionParameters(mi);
			Function func = new Function(mi, 255);
			double[] result;
			result = func.Execute(parameters);
			DrawResults(mi.Name, result);
		}

		private void chart1_Click(object sender, EventArgs e)
		{

		}

		private void CreateNetworkClick(object sender, EventArgs e)
		{
			chart1.Series.Clear();
		}

		private void ExecuteClick(object sender, EventArgs e)
		{

		}

		private void button2_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.DrawString("This is a diagonal line drawn on the control",
				  new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(30, 30));
			// Draw a line in the PictureBox.

		}
	}
}
