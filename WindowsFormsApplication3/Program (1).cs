using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
	static class Program
	{
		public static List<MethodInfo> taMethods = new List<MethodInfo>();
		public static List<Price> prices = new List<Price>();
		public static Form1 form ;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			form = new Form1();
			Application.Run(form);
		}
	}
}
