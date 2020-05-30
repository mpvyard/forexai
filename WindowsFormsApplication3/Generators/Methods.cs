using System;
using System.Reflection;

namespace FinancePermutator.Generators
{
	static class Methods
	{
		public static MethodInfo GetRandomMethod(int randomSeed)
		{
			return Data.TALibMethods[XRandom.next(Data.TALibMethods.Count - 1)];
		}
	}
}