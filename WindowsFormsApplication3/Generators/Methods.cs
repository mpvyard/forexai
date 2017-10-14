using System;
using System.Reflection;

namespace FinancePermutator.Generators
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