using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication3
{
   internal static class Tools
   {
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      public static extern void OutputDebugString(string message);

      public static void Debug(string s)
      {
         System.Diagnostics.Debug.WriteLine(s);

         using (var logFile = File.AppendText(@"d:\temp\winform3.log"))
         {
            logFile.WriteLine($"{DateTime.Now:HH:mm:ss.fff} {s}");
         }
         //OutputDebugString(s);
      }
   }
}