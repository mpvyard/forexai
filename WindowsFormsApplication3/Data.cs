﻿using FinancePermutator.Prices;

namespace FinancePermutator
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	internal static class Data
	{
		public static string chartBigLabel = "";
		public static readonly List<PriceEntry> ForexPrices = new List<PriceEntry>();
		public static readonly List<MethodInfo> TALibMethods = new List<MethodInfo>();
		public static int loadPercent = 0;
		public static Dictionary<string, Dictionary<string, object>> FunctionBase = new Dictionary<string, Dictionary<string, object>>();
	}
}