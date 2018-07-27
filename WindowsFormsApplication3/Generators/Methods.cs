using System;
using System.Reflection;

namespace FinancePermutator.Generators
{
    static class Methods
    {
        public static MethodInfo GetRandomMethod(int randomSeed)
        {
            return Repository.TALibMethods[XRandom.next(Repository.TALibMethods.Count - 1)];
        }
    }
}