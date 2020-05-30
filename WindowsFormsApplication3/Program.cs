namespace FinancePermutator
{
	using System;
	using System.Windows.Forms;
	using Forms;
	using static FinancePermutator.Tools;
	using System.Runtime.InteropServices;
	using Microsoft.AppCenter;
	using Microsoft.AppCenter.Analytics;
	using Microsoft.AppCenter.Crashes;

	static class Program
	{
		public static Form1 Form;

		[STAThread]
		private static void Main()
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

			debug($"starting application ... pid {GetCurrentProcessId()}");

			Application.ThreadException += (sender, args) =>
			{
				Crashes.TrackError(args.Exception);
			};

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				debug("connecting with appStats");

				AppCenter.Start("caac19a5-4dc0-4d17-a2c7-9c2dea745a8e", typeof(Analytics), typeof(Crashes));

				Crashes.GetErrorAttachments = (ErrorReport report) =>
				{
					return new ErrorAttachmentLog[]
					{
						ErrorAttachmentLog.AttachmentWithText(System.IO.File.ReadAllText($@"{Configuration.LogFileName}"), "debug"),
					};
				};

				Form = new Form1();
				Application.Run(Form);
			}
			catch(Exception e)
			{
				debug($"exception: {e.Message}");
			}
		}
	}
}