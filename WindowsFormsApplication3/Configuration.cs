using FANNCSharp;

namespace FinancePermutator
{
	public static class Configuration
	{
		public static string PriceFileName = @"c:\forexAI\GBPUSD5.csv";
		public static string LogFileName = @"c:\forexAI\forexAI.log";
		public static string TempPath = "%TEMP%";
		public static int maxInputDimension = 64;
		public static double MinSaveTestMSE = 0.02;
		public static double MinSaveHit = 85;
		public static int MinSaveEpoch = 20;
		public static int MinTaFunctionsCount = 3;
		public static int TestDataAmountPerc = 6;
		public static int TrainLimitEpochs = 650;
		public static uint DefaultHiddenNeurons = 0;
		public static int SleepTime = 300;
		public static int SleepCheckTime = 10000;
		public static int OutputIndex = 15;
		public static TrainingAlgorithm TrainAlgo = TrainingAlgorithm.TRAIN_QUICKPROP;
	}
}