using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FANNCSharp.Double;
using Syncfusion.Styles;
using TicTacTec.TA.Library;
using DataType = System.Double;

namespace WindowsFormsApplication3
{
   /// <summary>
   /// The form 1.
   /// </summary>
   public partial class Form1 : Form
   {
      readonly Random random = new Random();
      int methodNum = 0;

      public Form1()
      {
         InitializeComponent();
      }

      private void LoadClick(object sender, EventArgs e)
      {
         loadPricesButton.Text = @"loading";
         var loadPrices = new LoadPrices();
      }

      private void Form1Resize(object sender, EventArgs e)
      {
         if (FormWindowState.Minimized == this.WindowState)
         {
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.BalloonTipTitle = @"SYSTEM";
            this.notifyIcon1.BalloonTipText = @"running in background";
            this.notifyIcon1.ShowBalloonTip(500);

            this.notifyIcon1.Text = @"[Message shown when hovering over tray icon]";
            this.Hide();
         }
         else if (FormWindowState.Normal == this.WindowState)
         {
            this.notifyIcon1.Visible = false;
         }
      }

      private void TrayClick(object sender, MouseEventArgs e)
      {
         Show();
         this.WindowState = FormWindowState.Normal;
         this.notifyIcon1.Visible = false;
      }

      private void CheckBox1CheckedChanged(object sender, EventArgs e)
      {
         Tools.Debug($"checkbox: {checkBox1.CheckState == CheckState.Checked}");
      }

      public static string GetParamName(MethodInfo method, int index)
      {
         string retVal = string.Empty;

         if (method != null && method.GetParameters().Length > index)
            retVal = method.GetParameters()[index].Name;

         return retVal;
      }

      private bool IsGoodMethod(MethodInfo method)
      {
         bool addMethod = false;
         foreach (var param in method.GetParameters())
         {
            if (param.Name.Contains("inReal") || param.Name.Contains("inOpen") ||
                param.Name.Contains("inClose") ||
                param.Name.Contains("inLow") || param.Name.Contains("inHigh"))
               addMethod = true;

            if (param.Name.Contains("outFast") || param.Name.Contains("outSine") ||
                param.Name.Contains("outMACD") || param.Name.Contains("outMAMA") ||
                param.Name.Contains("outSlowK") || param.Name.Contains("outAroonDown") ||
                param.Name.Contains("inPeriods") || param.Name.Contains("outMin") ||
                param.Name.Contains("outInPhase"))
               addMethod = false;
            Tools.Debug($" param: {param.Name} {param.ParameterType}");
         }
         return addMethod;
      }

      private void AddMethod(MethodInfo method)
      {
         //if (method.ToString().Contains("Cdl"))
         // return;

         if (method.ToString().Contains("Single[]"))
            return;

         if (method.ToString().Contains("Lookback"))
            return;

         if (IsGoodMethod(method))
         {
            Data.TaLibMethods.Add(method);
            Tools.Debug($"method #{methodNum++,-3:000}: {method}");
         }
      }

      private void LoadTaLib(object sender, EventArgs e)
      {
         Tools.Debug("Harvesing ta-lib");

         methodNum = 0;

         Program.Form.loadTAButton.Invoke((MethodInvoker)(()
            => Program.Form.loadTAButton.Text = "examining"));

         Data.TaLibMethods.Clear();

         var methodInfos = typeof(Core).GetMethods(BindingFlags.Static | BindingFlags.Public);

         foreach (var method in methodInfos)
            AddMethod(method);

         Program.Form.loadTAButton.Text = Data.TaLibMethods.Count + " OK";
      }

      private void TimeFastTick(object sender, EventArgs e)
      {
         // Tools.Debug("tick");
      }

      public void DrawResults(string name, double[] data)
      {
         if (Program.Form.chart1.Series.IndexOf(name) > 0)
            return;

         Program.Form.chart1.Series.Add(name);

         foreach (var v in data)
         {
            //	Tools.Debug("v: " + v);
            Program.Form.chart1.Invoke((MethodInvoker)(()
               =>
            { Program.Form.chart1.Series[name].Points.AddXY("xxx", v); }));
         }
      }

      private void GenerateTrainClick(object sender, EventArgs e)
      {
         if (!Data.TaLibMethods.Any())
            return;

         MethodInfo mi = Data.TaLibMethods[this.random.Next(Data.TaLibMethods.Count - 1)];

         Tools.Debug($"selected function: {mi.Name}");

         FunctionParameters parameters = new FunctionParameters(mi);

         Function func = new Function(mi, 255);

         var result = func.Execute(parameters);

         DrawResults(mi.Name, result);
      }

      private void Chart1Click(object sender, EventArgs e)
      {

      }

      private void CreateNetworkClick(object sender, EventArgs e)
      {
         chart1.Series.Clear();
      }

      private void ExecuteClick(object sender, EventArgs e)
      {
         foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
         {
            if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211
                && ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
               continue;
            Tools.Debug($"interface: {ni.Name}");
            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
               if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
               {
                  Tools.Debug($"ip: {ip.Address}");
               }
            }
         }
      }

      private void Button2Paint(object sender, PaintEventArgs e)
      {
         Graphics g = e.Graphics;
         g.DrawString("This is a diagonal line drawn on the control",
              new Font("Arial", 10), Brushes.Blue, new Point(30, 30));
         // Draw a line in the PictureBox.

      }

      private void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
      {

      }
   }
}
