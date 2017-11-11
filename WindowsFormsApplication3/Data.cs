using FinancePermutator.Prices;

namespace FinancePermutator
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	internal static class Data
	{
		public static readonly List<PriceEntry> ForexPrices = new List<PriceEntry>();
		public static readonly List<MethodInfo> TALibMethods = new List<MethodInfo>();

		public static Dictionary<string, Dictionary<string, object>> FunctionsBase = new Dictionary<string, Dictionary<string, object>>();
	}
}