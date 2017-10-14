using System;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
	internal static class Program
	{
		public static Form1 Form;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form = new Form1();
			Application.Run(Form);
		}
	}
}
