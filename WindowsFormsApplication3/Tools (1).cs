using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication3
{

	internal class Tools
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern void OutputDebugString(string message);

		public static void deb(String s)
		{
			Debug.WriteLine(s);
			//OutputDebugString(s);
		}
	}
}