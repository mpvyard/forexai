using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
//using FANNCSharp.TrainingAlgorithm;

using FinancePermutator.Generators;
using FinancePermutator.Prices;
using Newtonsoft.Json;
using TicTacTec.TA.Library;
using static FinancePermutator.Tools;

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

namespace FinancePermutator.Forms
{
    using FinancePermutator.Train;

    /// <summary>
    /// Main form
    /// </summary>
    public partial class Form1 : Form
    {
        private int methodNum;
        private Train threadProcessScan;
        internal bool DoingSearch;

        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern int GetCurrentThreadId();

        public void ConfigurationClear()
        {
            Program.Form.chart.Invoke((MethodInvoker) (() =>
            {
                Program.Form.configurationTab.Clear();
            }));
        }

        public void AddConfiguration(string text)
        {
            Program.Form.chart.Invoke((MethodInvoker) (() =>
            {
                Program.Form.configurationTab.AppendText(text);
            }));
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

        private static bool IsGoodMethod(MethodInfo method)
        {
            /*if (method.ToString().Contains("Cdl"))
                return false;*/
            if (method.ToString().Contains("Single[]"))
                return false;
            if (method.ToString().Contains("Lookback"))
                return false;
            if (method.ToString().Contains("UnstablePeriod"))
                return false;
            if (method.ToString().Contains("Compatibility"))
                return false;
            if (method.ToString().Contains("Candle"))
                return false;
            if (method.ToString().Contains("Sub"))
                return false;

            bool bAddMethod = false;

            if (!CheckFunctionParams(method, ref bAddMethod))
                return false;

            if (!CheckUsageFunction(method))
                return false;

            return bAddMethod;
        }

        private static bool CheckUsageFunction(MethodInfo methodInfo)
        {
            //int inputDimension = XRandom.next(8, (XRandom.next(9, Configuration.InputDimension)));
            int inputDimension = Configuration.maxInputDimension;

            // generate parameters
            FunctionParameters functionParameters = new FunctionParameters(methodInfo, inputDimension, 0);

            debug($"Method : {methodInfo.Name}");
            string json = JsonConvert.SerializeObject(functionParameters);
            debug($"Params: {json}");

            // execute function
            var function = new Function(methodInfo);
            double[] result = function.Execute(functionParameters, out var code);

            debug($"Code: {code}");
            string resultJson = JsonConvert.SerializeObject(result);
            debug($"Result : {resultJson}");

            // check function output
            if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
                double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayRepeating(result))
            {
                return false;
            }

            return true;
        }

        private static bool CheckFunctionParams(MethodInfo method, ref bool bAddMethod)
        {
            foreach (var param in method.GetParameters())
            {
                // debug($" param: {param.Name} {param.ParameterType}");
                if (param.Name.Contains("inReal") || param.Name.Contains("inOpen") || param.Name.Contains("inClose") || param.Name.Contains("inLow")
                    || param.Name.Contains("inHigh"))
                    bAddMethod = true;

                if (param.Name.Contains("outFast") || param.Name.Contains("outSine") || param.Name.Contains("outMACD")
                    || param.Name.Contains("outMAMA") || param.Name.Contains("outSlowK") || param.Name.Contains("outAroonDown")
                    || param.Name.Contains("inPeriods") || param.Name.Contains("outMin") || param.Name.Contains("outInPhase"))
                    return false;
            }

            return true;
        }

        private void CheckOneFunction()
        {
            double[] inHigh = { 1.22127, 1.22192, 1.22171, 1.22171, 1.22159, 1.22161, 1.22168, 1.22164, 1.22165, 1.22151, 1.22133, 1.2214, 1.22172, 1.22181, 1.22169, 1.22185, 1.22154, 1.22173, 1.22179, 1.22192, 1.22187, 1.22234, 1.22206, 1.22231, 1.22227, 1.22257, 1.22239, 1.22187, 1.22233, 1.22218, 1.2218, 1.22172, 1.22173, 1.22177, 1.22144, 1.2213, 1.22139, 1.22094, 1.22065, 1.22141, 1.22139, 1.22107, 1.22106, 1.22107, 1.22113, 1.22055, 1.22068, 1.221, 1.22144, 1.22271, 1.22281, 1.22288, 1.22453, 1.22713, 1.22618, 1.22565, 1.22422, 1.22463, 1.224, 1.2227, 1.22311, 1.22344, 1.22349, 1.22286};
            double[] inLow = { 1.22109, 1.22116, 1.22091, 1.22077, 1.22142, 1.2214, 1.22136, 1.22143, 1.2214, 1.22112, 1.22098, 1.22097, 1.22128, 1.22133, 1.2213, 1.2213, 1.22126, 1.22133, 1.22141, 1.22142, 1.22145, 1.22167, 1.22165, 1.22176, 1.22185, 1.2214, 1.22166, 1.22149, 1.22147, 1.22154, 1.22115, 1.22128, 1.22141, 1.22121, 1.22082, 1.22085, 1.22069, 1.21998, 1.22032, 1.22056, 1.22088, 1.22035, 1.22039, 1.22055, 1.22013, 1.22009, 1.22027, 1.22027, 1.22051, 1.22144, 1.22193, 1.222, 1.22285, 1.22412, 1.22494, 1.22327, 1.22312, 1.22374, 1.2209, 1.22175, 1.22223, 1.22286, 1.22277, 1.2221 };
			double[] result = new double[64];

			Core.Apo(0, 63, inHigh, 5, 40, Core.MAType.T3, out int outBegin, out int outEnd, result);

            // check function output
            if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
                double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayRepeating(result))
            {
                debug($"Bad Result");
            }
            else {
                debug($"Good Result");
            }

            string json = JsonConvert.SerializeObject(result);
            debug($"outReal: {json}");

            debug($"outEnd: {outEnd}");

        }

        private void LoadTaLib(object sender, EventArgs e)
        {
            debug("Harvesing ta-lib");
            methodNum = 0;

            //CheckOneFunction();
            //return;

            Repository.TALibMethods.Clear();
            var methodInfos = typeof(Core).GetMethods(BindingFlags.Static | BindingFlags.Public);


            foreach (var method in methodInfos)
            {       
                if (IsGoodMethod(method))
                {
                    Repository.TALibMethods.Add(method);
                    debug($"Good method #{this.methodNum++,-3:000}: {method}");
                    Program.Form.loadTAButton.Invoke((MethodInvoker)(() => Program.Form.loadTAButton.Text = $"loading {this.methodNum}"));
                }
                else
                {
                    debug($"Wrong method #{this.methodNum++,-3:000}: {method}");
                }
            }

            Program.Form.loadTAButton.Text = $"{Repository.TALibMethods.Count} OK";

            if (Repository.Prices.Any())
                Program.Form.buttonExecute.Enabled = true;
        }

        private void TimeFastTick(object sender, EventArgs e)
        {
            lock (writeMessagesBlock)
            {
                if (Messages.Any())
                    WriteMessages();
            }

            FinancePermutator.Train.Train.SetStats();
        }

        internal static void DrawResults(string name, double[] data, bool clearCurrent = true)
        {
            // if (Program.Form.chart.Series.IndexOf(name) != -1)
            // return;
            // debug($"draw name '{name}'");
            Program.Form.chart.Invoke(
                (MethodInvoker) (() =>
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

        public void SetStatus(string status)
        {
            Program.Form.statusLabel.Invoke((MethodInvoker) (() =>
            {
                Program.Form.statusLabel.Text = status;
            }));
        }

        private void GenerateTrainButtonClick(object sender, EventArgs e)
        {
            switch (DoingSearch)
            {
                case true:
                    threadProcessScan.runScan = false;

                    // threadProcessScan.Stop();
                    debug("stop scan");
                    DoingSearch = false;
                    break;

                case false:
                    threadProcessScan = new Train();
                    threadProcessScan.Start();
                    DoingSearch = true;
                    break;
            }
        }

        private void ChartClick(object sender, EventArgs e)
        {
        }

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
                e.Graphics.DrawString(
                    line,
                    new Font("lucida console", 8, FontStyle.Regular),
                    SystemBrushes.HighlightText,
                    e.Bounds.Left,
                    e.Bounds.Top + 1);
            }
            else
            {
                var brush = new SolidBrush(Color.DarkSlateBlue);
                if (line.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0
                    || line.IndexOf("Exception", StringComparison.OrdinalIgnoreCase) >= 0)
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

        public void SetStats(string text)
        {
            Program.Form.statsBox.Invoke((MethodInvoker) (() =>
            {
                statsBox.Text = text;
            }));
        }

        public void EraseBigLabel()
        {
            Program.Form.chart.Invoke((MethodInvoker) (() =>
            {
                Program.Form.chart.Series.Clear();
            }));
            Program.Form.debugView.Invoke((MethodInvoker) (() =>
            {
                Repository.chartBigLabel = string.Empty;
            }));
        }

        public void SetBigLabel(string text)
        {
            Program.Form.debugView.Invoke(
                (MethodInvoker) (() =>
                {
                    Repository.chartBigLabel = text.Length > 0 ? text : $"[MUTATING DATA {Repository.loadPercent,4:####}%]";
                }));
        }

        private void Chart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (Repository.chartBigLabel.Length <= 1)
                return;

            Font drawFont = new Font("Consolas", 12);
            SolidBrush drawBrush = new SolidBrush(Color.BlueViolet);

            // Create rectangle for drawing.
            float x = 100.0F;
            float y = 100.0F;
            float width = 300.0F;
            float height = 250.0F;
            RectangleF drawRect = new RectangleF(x, y, width, height);

            // Draw rectangle to screen.
            Pen blackPen = new Pen(Color.DeepSkyBlue);
            e.ChartGraphics.Graphics.DrawRectangle(blackPen, x, y, width, height);

			// Set format of string.
			StringFormat drawFormat = new StringFormat(StringFormatFlags.FitBlackBox)
			{
				Alignment = StringAlignment.Center
			};
			e.ChartGraphics.Graphics.DrawString(Repository.chartBigLabel, drawFont, drawBrush, drawRect, drawFormat);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
			int.TryParse(Program.Form.minSavePercTextBox.Text, out int savePerc);
			debug($"minSavePerc changed => {savePerc}%");
            Configuration.MinSaveHit = savePerc;
        }
    }
}