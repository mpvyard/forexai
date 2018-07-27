using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using static FinancePermutator.Tools;

namespace FinancePermutator.Prices
{
    class LoadPrices
    {
        public static void Exec() => new Thread(
            () =>
            {
                long length = new FileInfo(Configuration.PriceFileName).Length;

                debug($"ManagedThreadId: {Thread.CurrentThread.ManagedThreadId.ToString("00000").PadRight(10)}");
                debug($"Length: {length.ToString().PadLeft(10)}");

                int lineNum = 0;
                DateTime first = new DateTime();
                bool bFirst = false;
                var lines = File.ReadAllLines(Configuration.PriceFileName);

                foreach (string line in lines)
                {
                    string[] tokens = line.Split(',');

                    if (!bFirst)
                    {
                        bFirst = true;
                        first = DateTime.Parse(tokens[0] + " " + tokens[1]);
                    }

                    DateTime timeDate = DateTime.Parse(tokens[0] + " " + tokens[1]);

                    var priceEntry = new PriceEntry
                    {
                        Low = double.Parse(tokens[4], CultureInfo.InvariantCulture),
                        High = double.Parse(tokens[3], CultureInfo.InvariantCulture),
                        Close = double.Parse(tokens[5], CultureInfo.InvariantCulture),
                        Open = double.Parse(tokens[2], CultureInfo.InvariantCulture),
                        Vol = double.Parse(tokens[6], CultureInfo.InvariantCulture),
                        Date = DateTime.Parse(tokens[0] + " " + tokens[1])
                    };

                    Repository.Prices.Add(priceEntry);

                    if (lineNum % 1850 == 0)
                    {
                        debug($"load {lineNum} {timeDate}");
                        Program.Form.chart.Invoke(
                            (MethodInvoker) (() => Program.Form.chart.Series["Series1"].Points.AddXY(priceEntry.Date, priceEntry.Open)));
                    }

                    lineNum++;
                }

                debug($"lines: {lineNum}");
                debug($"first time: {first}");

                Program.Form.loadPricesButton.Invoke((MethodInvoker) (() => Program.Form.loadPricesButton.Text = lineNum + @" OK"));
            }).Start();
    }
}