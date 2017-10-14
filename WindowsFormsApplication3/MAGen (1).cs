using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication3
{
	 class MAGen
	 {
		  static Random random = new Random();

		  public static int GetRand()
		  {
				int[] periods = { 3, 5, 7, 10, 13, 16, 19, 23, 27, 30 };

				//return periods[random.nextInt(periods.length)];
				return 2 + random.Next(55);
		  }
	 }
}
