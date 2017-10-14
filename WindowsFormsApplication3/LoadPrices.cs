using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    class LoadPrices
    {
        public LoadPrices()
        {
            new Thread(
                () =>
                    {
                        const string FileName = @"d:\temp\GBPUSD.csv";
                        var fileInfo = new FileInfo(FileName);
                        var length = fileInfo.Length;

                        Tools.Debug(
                            $"ManagedThreadId: {Thread.CurrentThread.ManagedThreadId.ToString("00000").PadRight(10)}");
                        Tools.Debug($"Length: {length.ToString().PadLeft(10)}");

                        int lineNum = 0;
                        var lines = File.ReadLines(FileName);

                        foreach (string line in lines)
                        {
                            string[] tokens = line.Split(',');

                            Price priceEntry = new Price
                            {
                                Low = double.Parse(tokens[4], CultureInfo.InvariantCulture),
                                High = double.Parse(tokens[3], CultureInfo.InvariantCulture),
                                Close = double.Parse(tokens[5], CultureInfo.InvariantCulture),
                                Open = double.Parse(tokens[2], CultureInfo.InvariantCulture),
                                Vol = double.Parse(tokens[6], CultureInfo.InvariantCulture),
                                Date = DateTime.Parse(tokens[0] + " " + tokens[1])
                            };

                            Data.Prices.Add(priceEntry);

                            if (lineNum % 850 == 0)
                                Program.Form.chart1.Invoke(
                                    (MethodInvoker)(() => Program.Form.chart1.Series["Series1"].Points.AddXY(priceEntry.Date, priceEntry.Open)));

                            lineNum++;
                        }
                        Tools.Debug("lines: " + lineNum);

                        Program.Form.loadPricesButton.Invoke(
                            (MethodInvoker)(() => Program.Form.loadPricesButton.Text = lineNum + @" OK"));
                    }).Start();
        }
    }
}