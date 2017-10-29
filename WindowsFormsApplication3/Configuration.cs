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
		public const int InputDimension = 128;
		public const double MinSaveTestMSE = 0.2;
		public const int MaxOffset = 140000;
		public const int TaFunctionsCount = 8;
		public const int TestDataAmountPerc = 8;
		public const int TrainLimitEpochs = 200;
		public const uint DefaultHiddenNeurons = 0;

		public const TrainingAlgorithm TrainAlgo = TrainingAlgorithm.TRAIN_RPROP; //TRAIN_QUICKPROP;//TRAIN_RPROP;
		//TRAIN_BATCH;//TRAIN_INCREMENTAL;//TRAIN_SARPROP;
	}
}