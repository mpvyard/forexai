﻿namespace FinancePermutator
{
	using System;
	using System.Windows.Forms;
	using Forms;
	using static FinancePermutator.Tools;

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

			try
			{
				Application.Run(Form);
			}
			catch(Exception e)
			{
				debug($"exception: {e.Message}");
			}
		}
	}
}