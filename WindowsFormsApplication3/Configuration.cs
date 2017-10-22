using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FANNCSharp;
using FANNCSharp.Double;

namespace FinancePermutator
{
	public static class Configuration
	{
		public const string PriceFileName = @"d:\temp\GBPUSD.csv";
		public const string LogFileName = @"d:\temp\winform3.log";
		public const int ValuesCount = 64;
		public const int MaxOffset = 140000;
		public const int TaFunctionsCount = 5;
		public const int TestDataAmountPerc = 5;

		public const TrainingAlgorithm TrainAlgo = TrainingAlgorithm.TRAIN_RPROP;//TRAIN_QUICKPROP;//TRAIN_RPROP;

		//TRAIN_BATCH;//TRAIN_INCREMENTAL;//TRAIN_RPROP;
		public const int TrainEpochs = 130;

		public const uint DefaultHiddenNeurons = 0;
	}
}