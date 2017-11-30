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
		public const string PriceFileName = @"d:\temp\forexAI\GBPUSD.csv";
		public const string LogFileName = @"d:\temp\forexAI\forexAI.log";
		public const int InputDimension = 64;
		public const double MinSaveTestMSE = 0.02;
		public const double MinSaveHit = 80;
		public const int MinSaveEpoch = 20;
		public const int MinTaFunctionsCount = 3;
		public const int TestDataAmountPerc = 6;
		public const int TrainLimitEpochs = 200;
		public static uint DefaultHiddenNeurons = 0;
		public const int SleepTime = 300;
		public const int SleepCheckTime = 10000;
		public const int OutputIndex = 5;
		public const TrainingAlgorithm TrainAlgo = TrainingAlgorithm.TRAIN_RPROP; //TRAIN_QUICKPROP;//TRAIN_RPROP;
		//TRAIN_BATCH;//TRAIN_INCREMENTAL;//TRAIN_SARPROP;
	}
}