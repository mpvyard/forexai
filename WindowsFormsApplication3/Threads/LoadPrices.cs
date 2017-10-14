using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    class LoadPrices
    {
        public LoadPrices()
        {
            Thread funcThread = new Thread(delegate ()
            {
                Tools.deb("ManagedThreadId: " + Thread.CurrentThread.ManagedThreadId.ToString("00000").PadRight(10));

                string fileName = "f:\\temp\\GBPUSD60.csv";
                FileInfo info = new FileInfo(fileName);
                long length = info.Length;
                Tools.deb("Length: " + length.ToString().PadLeft(10));

                int lineNum = 0;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    Price price = new Price();

                    price.Low = double.Parse(tokens[4], CultureInfo.InvariantCulture);
                    price.High = double.Parse(tokens[3], CultureInfo.InvariantCulture);
                    price.Close = double.Parse(tokens[5], CultureInfo.InvariantCulture);
                    price.Open = double.Parse(tokens[2], CultureInfo.InvariantCulture);
                    price.Vol = double.Parse(tokens[6], CultureInfo.InvariantCulture);
                    price.date = DateTime.Parse(tokens[0]);

                    Program.prices.Add(price);

                    if (lineNum % 1850 == 0)
                        Program.form.chart1.Invoke((MethodInvoker)(()
                           => Program.form.chart1.Series["Series1"].Points.AddXY(price.date, price.Open)));

                    lineNum++;
                }
                Tools.deb("lines: " + lineNum);

                Program.form.loadPricesButton.Invoke((MethodInvoker)(() =>
                   Program.form.loadPricesButton.Text = lineNum + " OK"));
            });
            funcThread.Start();
        }
    }
}