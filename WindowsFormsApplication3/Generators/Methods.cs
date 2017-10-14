using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinancePermutator
{
    static class Methods
    {
        private static Random random;

        public static MethodInfo GetRandomMethod(int randomSeed)
        {
            random = new Random(randomSeed);//DateTime.Now.Millisecond
            return Data.TALibMethods[random.Next(Data.TALibMethods.Count - 1)];
        }
    }
}