using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FANNCSharp.Double;
using FinancePermutator.Function;
using FinancePermutator.Generators;
using FinancePermutator.Networks;
using static FinancePermutator.Tools;

namespace FinancePermutator.Train
{
	class Train
	{
		static int InputDimension = Configuration.InputDimension;
		static double[][] inputSets = new double[1][];
		static double[][] outputSets = new double[1][];
		static double[] combinedResult;
		static double[] result;
		static int numRecord;
		static int prevOffset;
		public bool RunScan = true;
		public static int class1;
		public static int class2;
		public static int class0;
		private static Thread thread;
		private static Network network;

		public Train()
		{
			thread = new Thread(ProcessScan);
		}

		public void Stop()
		{
			thread.Abort();
		}

		public void Start()
		{
			ClearParameters();

			RunScan = true;
			thread.Priority = ThreadPriority.Lowest;
			thread.Start();
		}

		public void ProcessScan()
		{
			if (!Data.TALibMethods.Any())
				return;

			int ret;
			do
			{
				Program.Form.setStatus("generating functions list");

				again:

				Data.FunctionsBase.Clear();
				Program.Form.debugView.Items.Clear();

				int randomSeed = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;
				Random random = new Random(randomSeed);
				SetupFunctions(randomSeed);

				InputDimension = random.Next(8, 8 * (random.Next(8, 32)));

				debug($"function setup done, generating data [inputDimension={InputDimension}] ...");

				for (int offset = 0; offset < Data.ForexPrices.Count && RunScan; offset += InputDimension)
				{
					Program.Form.setStatus($"Generating train/test data {offset} - {offset + InputDimension} ...");
					//debug($"creating train data offset {offset} num {valuesCount} ...");

					combinedResult = new double[] { };

					foreach (var funct in Data.FunctionsBase)
					{
						// Thread.Yield();
						var functionInfo = funct.Value;
						//string funcName = funct.Key;

						randomSeed = (int) functionInfo["randomseed"];
						//debug($"load seed {randomSeed} for {funcName}");
						FunctionParameters functionParameters =
							new FunctionParameters((MethodInfo) functionInfo["methodInfo"], InputDimension, offset, randomSeed);

						// save seed

						// execute function
						Function.Function function = new Function.Function((MethodInfo) functionInfo["methodInfo"]);
						result = function.Execute(functionParameters, out var code);
						if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) ||
						    double.IsPositiveInfinity(result[0]) || double.IsNaN(result[0]) || double.IsInfinity(result[0]) ||
						    IsArrayAllZeros(result))
						{
							debug(
								$"WARNING: skip {((MethodInfo) functionInfo["methodInfo"]).Name} due to bad output [len={result.Length}, code={code}]");
							Program.Form.setStatus($"ERROR: bad output for {((MethodInfo) functionInfo["methodInfo"]).Name}");
							goto again;
						}

/*					if (numRecord % 3 == 0 && result != null && result.Length > 0)
						Form1.DrawResults(funcName, result, false);*/

						// copy new output to all data
						if (combinedResult != null)
							prevOffset = combinedResult.Length;

						Array.Resize(ref combinedResult, (combinedResult?.Length ?? 0) + result.Length);
						Array.Copy(result, 0, combinedResult, prevOffset, result.Length);

						//debug($"+result {result.Length} combined {combinedResult.Length} firstel {combinedResult[0]}");
					}

					// generate train data set
					Array.Resize(ref inputSets, numRecord + 1);
					Array.Resize(ref outputSets, numRecord + 1);
					//debug($"arrays resized to numRecord={numRecord}");

					inputSets[numRecord] = new double[combinedResult.Length];
					outputSets[numRecord] = new double[2];

					if (numRecord % 25 == 0)
						debug(
							$"offset: {offset} numRecord:{numRecord} inputSets:{inputSets.Length} outputSets:{outputSets.Length}\r\n combinedResult:{combinedResult.Length}");

					Array.Copy(combinedResult, inputSets[numRecord], combinedResult.Length);

					SetOutputResult(InputDimension, offset, numRecord);

					numRecord++;
					// hello 2

					if (offset > Configuration.MaxOffset)
						break;
				}

				if (!RunScan)
					return;

				/*if (-1 == TrainNetwork(ref inputSets, ref outputSets))
					debug($"ERROR: unsuccessfull train il {inputSets?.Length} ol {outputSets?.Length}");*/
				ret = TrainNetwork(ref inputSets, ref outputSets);
			} while (RunScan);

			// goto again;
			Program.Form.DoingSearch = false;
			debug($"done scan: numRecord={numRecord} i:{inputSets?.Length} o:{outputSets?.Length}");
		}

/*
		／ イ(((ヽ
		(ﾉ ￣Ｙ＼
		|　(＼　(. /) ｜ )
		ヽ ヽ` ( ͡° ͜ʖ ͡°) _ノ /
		＼ |　⌒Ｙ⌒　/ /
		｜ヽ　 ｜　 ﾉ ／
		＼トー仝ーイ
		｜ ミ土彡/
		)\ ° /
		( \ /
		/ / ѼΞΞΞΞΞΞΞD
		/ / / \ \ \ 
		(( ). ) ).)
		( ). ( | | 
		| / \ |*/

		private void SetupFunctions(int randomSeed)
		{
			Program.Form.ConfigurationClear();
			for (int i = 0; i < Configuration.TaFunctionsCount && RunScan; i++)
			{
				Thread.Yield();
				Thread.Sleep(10);

				int unixTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +
				                    DateTime.Now.Millisecond;

				// get random method
				var methodInfo = Methods.GetRandomMethod(unixTimestamp);

				Program.Form.setStatus($"Setup function #{i} <{methodInfo.Name}> ...");

				debug($"selected function #{i}: {methodInfo.Name} randomSeed: {randomSeed}");

				// generate parameters
				FunctionParameters functionParameters = new FunctionParameters(methodInfo, InputDimension, 0, randomSeed);

				// save seed
				randomSeed = functionParameters.RandomSeed;

				// execute function
				var function = new Function.Function(methodInfo);
				result = function.Execute(functionParameters, out var code);
				if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) ||
				    double.IsPositiveInfinity(result[0]) || double.IsNaN(result[0]) || double.IsInfinity(result[0]) ||
				    IsArrayAllZeros(result))
				{
					DumpValues(methodInfo, result);
					debug(
						$"WARNING: skip {methodInfo.Name} due to bad output [len={result.Length}, code={code}], need {Configuration.TaFunctionsCount - i}");
					if (i > 0)
						i--;
					randomSeed = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;
					continue;
				}

				// record info
				Data.FunctionsBase[methodInfo.Name] = new Dictionary<string, object>();
				Data.FunctionsBase[methodInfo.Name]["parameters"] = functionParameters;
				Data.FunctionsBase[methodInfo.Name]["result"] = result;
				Data.FunctionsBase[methodInfo.Name]["randomseed"] = randomSeed;
				Data.FunctionsBase[methodInfo.Name]["methodInfo"] = methodInfo;

				randomSeed = unixTimestamp;
			}
			//debug($"functions in input: {Data.FunctionsBase.Count}");

			var functions = new StringBuilder();

			foreach (var item in Data.FunctionsBase)
			{
				int seed = (int) Data.FunctionsBase[item.Key]["randomseed"];
				functions.Append($"{item.Key}[seed={seed}] ");
			}

			//debug($"setup functions : {functions}");

			Program.Form.funcListLabel.Invoke(
				(MethodInvoker) (() => { Program.Form.funcListLabel.Text = functions.ToString(); }));
			//Process.GetCurrentProcess().Kill();
		}

		private bool CheckInputDataIsCorrect(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
		{
			Program.Form.setStatus(
				$"Checking training data: input {inputSetsLocal?.Length} output {outputSetsLocal?.Length} ...");

			if (inputSetsLocal == null || outputSetsLocal == null || inputSetsLocal[0] == null || outputSetsLocal[0] == null)
			{
				debug("first null");
				return false;
			}

			double firstValue = 0;
			foreach (double[] set in inputSetsLocal)
			{
				int setLength = set.Length;
				if (firstValue == 0)
					firstValue = setLength;
				if (setLength != firstValue)
				{
					debug("ERROR: check same input size failed");
					return false;
				}
			}

			/*inputSetsLocal[inputSetsLocal.Length - 1] = null;
			outputSetsLocal[outputSetsLocal.Length - 1] = null;

			Array.Resize(ref inputSetsLocal, inputSetsLocal.Length - 1);
			Array.Resize(ref outputSetsLocal, outputSetsLocal.Length - 1);*/

			for (var i = 0; i < inputSetsLocal.Length; i++)
				if (inputSetsLocal[i] == null || inputSetsLocal[i].Length == 0)
					debug($"ERROR: input {i} is NULL! (Length:{inputSetsLocal.Length})");
			for (var i = 0; i < outputSetsLocal.Length; i++)
				if (outputSetsLocal[i] == null || outputSetsLocal[i].Length == 0)
					debug($"ERROR: output {i} is NULL! (Length:{outputSetsLocal.Length})");

			// check input
			int j = 0, k = 0;
			foreach (var set in inputSetsLocal)
			{
				if (set == null)
				{
					debug($"ERROR: set {j} IS NULL");
					return false;
				}
				foreach (var cell in set)
				{
					if (double.IsPositiveInfinity(cell) || double.IsNegativeInfinity(cell) || double.IsInfinity(cell) ||
					    double.IsNaN(cell))
					{
						debug($"inputSetsLocal: error in CELL {j}:{k}  {cell}");
						/*MessageBox.Show($"inputSetsLocal: error in CELL {j}:{k} {cell}", "input", MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Error);*/
						return false;
					}
					k++;
				}
				k = 0;
				j++;
			}

			// check output
			j = k = 0;
			foreach (var set in outputSetsLocal)
			{
				if (set == null)
				{
					debug($"ERROR: set {j} IS NULL");
					return false;
				}
				foreach (var cell in set)
				{
					if (double.IsPositiveInfinity(cell) || double.IsNegativeInfinity(cell) || double.IsInfinity(cell) ||
					    double.IsNaN(cell))
					{
						debug($"outputSetsLocal: error in CELL {j}:{k} {cell}");
						/*MessageBox.Show($"outputSetsLocal: error in CELL {j}:{k} {cell}", "output", MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Error);*/
						return false;
					}
					k++;
				}
				k = 0;
				j++;
			}

			return true;
		}

		private void ClearParameters()
		{
			outputSets = null;
			inputSets = null;
			numRecord = 0;
		}

		private int TrainNetwork(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
		{
			double testMse = 0.0;
			double trainMse = 0.0;

			if (!CheckInputDataIsCorrect(ref inputSetsLocal, ref outputSetsLocal))
			{
				debug($"ERROR: data check failed");
				ClearParameters();
				return -1;
			}

			/*			 *
			░░ ♡ ▄▀▀▀▄░░░ 
			▄███▀░◐░░░▌░░░░░░░ 
			░░░░▌░░░░░▐░░░░░░░ 
			░░░░▐░░░░░▐░░░░░░░ 
			░░░░▌░░░░░▐▄▄░░░░░ 
			░░░░▌░░░░▄▀▒▒▀▀▀▀▄ 
			░░░▐░░░░▐▒▒▒▒▒▒▒▒▀▀▄ 
			░░░▐░░░░▐▄▒▒▒▒▒▒▒▒▒▒▀▄ 
			░░░░▀▄░░░░▀▄▒▒▒▒▒▒▒▒▒▒▀▄ 
			░░░░░░▀▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▀▄ 
			░░░░░░░░░░░▌▌░▌▌░░░░░ 
			░░░░░░░░░░░▌▌░▌▌░░░░░ 
			░░░░░░░░░▄▄▌▌▄▌▌░░░░░
			-----------
			*/

			// load train data
			debug(
				$"SetTrainData: inpustSetsLocal.Length: {inputSetsLocal.Length} outputSetsLocal.Length: {outputSetsLocal.Length} ");

			TrainingData trainData;
			TrainingData testData;
			try
			{
				trainData = new TrainingData();
				trainData.SetTrainData(inputSetsLocal, outputSetsLocal);

				testData = new TrainingData(trainData);
				var testDataOffset = trainData.TrainDataLength / Configuration.TestDataAmountPerc;

				testData.SubsetTrainData(0, testDataOffset);
				testData.ScaleTrainData(-1.0, 1.0);
				testData.SaveTrain(@"d:\temp\testdata.dat");

				trainData.SubsetTrainData(testDataOffset, trainData.TrainDataLength - testDataOffset);
				trainData.ScaleTrainData(-1.0, 1.0);
				trainData.SaveTrain(@"d:\temp\traindata.dat");
			}
			catch (Exception e)
			{
				debug($"Exception '{e.Message}' while working with train data");
				Program.Form.setStatus($"Exception '{e.Message}' while working with train data");
				/*if (MessageBox.Show($"err: {e.Message} {e.StackTrace}", "error", MessageBoxButtons.RetryCancel,
					    MessageBoxIcon.Error) == DialogResult.Cancel)
					throw;*/
				ClearParameters();
				return -1;
			}

			debug($"class1: {class1} class2: {class2} class0: {class0}");

			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				Program.Form.chart.Series.Clear();
				Program.Form.chart.Series.Add("train");
				Program.Form.chart.Series.Add("test");
				Program.Form.chart.Series["test"].ChartType = SeriesChartType.Line;
				Program.Form.chart.Series["train"].ChartType = SeriesChartType.Line;

				Program.Form.chart.Series["train"].BorderWidth = 2;
				Program.Form.chart.Series["test"].BorderWidth = 2;

				Program.Form.chart.Series[0].Color = Color.Green;
				Program.Form.chart.Series[1].Color = Color.DarkViolet;

				/*Program.Form.chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

				Program.Form.chart.ChartAreas[0].AxisX.Interval = 1;

				Program.Form.chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

				Program.Form.chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

				Program.Form.chart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;

				Program.Form.chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;

				Program.Form.chart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = 0.6;*/
			}));

			// create network to hold all input data
			var inputCount = trainData.InputCount;
			uint numNeurons = Configuration.DefaultHiddenNeurons > 0 ? Configuration.DefaultHiddenNeurons : inputCount / 2 - 1;
			debug($"new network: numinputs: {inputCount} neurons: {numNeurons}");

			network = new Network(inputCount, numNeurons, 2);

			network.TrainingAlgorithm = Configuration.TrainAlgo;

			// Net.SarTemp = 15.0f;
			network.InitWeights(trainData);
			network.SetupActivation();

			double minTestMSE = 1.0;

			debug("starting train");
			

			for (var epoch = 0; RunScan && inputSetsLocal != null && outputSetsLocal != null; epoch++)
			{
				if (epoch >= Configuration.TrainLimitEpochs)
				{
					debug("[AUTO-RESTART]");
					Program.Form.setStatus("AUTO-RESTARTING ...");
					ClearParameters();
					return -1;
				}
				Program.Form.setStatus($"[Training] TrainMSE {trainMse,-7:0.#####}  TestMSE {testMse,-7:0.#####} ");

				Thread.Yield();
				Thread.Sleep(150);

				/*network.SarpropStepErrorShift -= 0.01f;
				debug($"SarpropStepErrorShift {network.SarpropStepErrorShift}");*/

				trainMse = network.Train(trainData);
				if (network.ErrNo > 0)
					debug($"error {network.ErrNo}: {network.ErrStr}");

				// Net.SarTemp -= 0.1f;
				testMse = network.TestData(testData);
				if (network.ErrNo > 0)
					debug($"error {network.ErrNo}: {network.ErrStr}");

				debug($"train: epoch #{epoch} trainMse {trainMse} testmse {testMse} bitfail {network.BitFail}");

				// debug(String.Format("temp {0:0.##}", Net.SarTemp));
				var mse = trainMse;
				var mse1 = testMse;
				var epoch1 = epoch;

				if (trainMse <= 0.01 && epoch > 10)
				{
					debug($"finished training, reached corner trainmse={trainMse} testmse={testMse}");
					//RunScan = false;
					break;
				}

				if (testMse <= 0.15 && epoch > 10)
					SaveNetwork();

				//if (epoch % 2 == 0)
				Program.Form.chart.Invoke((MethodInvoker) (() =>
				{
					Program.Form.chart.Series["train"].Points.AddXY(epoch1, mse);
					Program.Form.chart.Series["test"].Points.AddXY(epoch1, mse1);

					//Program.Form.chart.Series["train"].Points[a1-1].Color = Color.Green;
					//Program.Form.chart.Series["test"].Points[a1-1].Color = Color.RosyBrown;
				}));

				testMse = network.TestData(testData);
			}

			var output = network.RunNetwork(inputSetsLocal[0]);
			testMse = network.TestData(testData);
			debug($"trained mse {trainMse} testmse {testMse} 0:should={outputSetsLocal[0][0]}");
			debug($"is={output[0]} should={outputSetsLocal[0][1]} is={output[1]} ");

			//network.Save(@"d:\temp\net_sharp.net");
			output = network.RunNetwork(inputSetsLocal[0]);
			testMse = network.TestData(testData);
			Program.Form.setStatus(
				$"[Done] Trainmse {trainMse,6:0.####} Testmse {testMse,6:0.####} . should={outputSetsLocal[0][1]} is={output[1]}.");

			return 0;
		}

		private static void SaveNetwork()
		{
			if (!Directory.Exists($"d:\\temp\\forexAI\\{network.GetHashCode()}"))
				Directory.CreateDirectory($"d:\\temp\\forexAI\\{network.GetHashCode()}");
			;
			network.Save($"d:\\temp\\forexAI\\{network.GetHashCode()}\\{network.GetHashCode(),4:0.####}.net");
			File.Copy("d:\\temp\\traindata.dat", $"d:\\temp\\forexAI\\{network.GetHashCode()}\\traindata.dat", true);
			File.Copy("d:\\temp\\testdata.dat", $"d:\\temp\\forexAI\\{network.GetHashCode()}\\testdata.dat", true);
			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				Program.Form.chart.SaveImage($"d:\\temp\\forexAI\\{network.GetHashCode()}\\chart.jpg", ChartImageFormat.Jpeg);
			}));
		}

		private static void SetOutputResult(int valuesCountLocal, int offset, int numRecordLocal)
		{
			double[] priceOpen = ForexPrices.GetOpen(valuesCountLocal, offset);
			if (priceOpen[valuesCountLocal - 1] > priceOpen[valuesCountLocal - 5])
			{
				// if (numRecord > 3 && priceOpen.Length > 3)
				// debug($"price {priceOpen[numRecord - 3]} {priceOpen[numRecord - 2]}");
				outputSets[numRecordLocal][0] = 1;
				outputSets[numRecordLocal][1] = -1;
				class1++;
			}
			else if (priceOpen[valuesCountLocal - 1] == priceOpen[valuesCountLocal - 5])
			{
				outputSets[numRecordLocal][0] = -1;
				outputSets[numRecordLocal][1] = -1;
				class0++;
			}
			else
			{
				outputSets[numRecordLocal][0] = -1;
				outputSets[numRecordLocal][1] = 1;
				class2++;
			}

			/*if (numRecord % 500 == 0)
				debug($"#{numRecord} set output {outputSets[numRecord][0]} {outputSets[numRecord][1]}");*/
		}
	}
}