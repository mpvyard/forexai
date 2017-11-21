/*     
				(<$$$$$$>#####<::::::>)
            _/~~~~~~~~~~~~~~~~~~~~~~~~~\
          /~                             ~\
        .~                                 ~

	()\/_____ _____\/()
   .-''      ~~~~~~~~~~~~~~~~~~~~~~~~~~~     ``-.
.-~__________________              ~-.
`~~/~~~~~~~~~~~~TTTTTTTTTTTTTTTTTTTT~~~~~~~~~~~~\~~'
| | | #### #### || | | | [] | | | || #### #### | | |
;__\|___________|++++++++++++++++++|___________|/__;
 (~~====___________________________________====~~~)
  \------_____________[CHIP 911] __________-------/
     |      ||         ~~~~~~~~       ||      |
      \_____/                          \_____/*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FANNCSharp.Double;
using FinancePermutator.Function;
using FinancePermutator.Generators;
using FinancePermutator.Networks;
using FinancePermutator.Prices;
using static FinancePermutator.Tools;

namespace FinancePermutator.Train
{
	class Train
	{
		static int InputDimension = Configuration.InputDimension;
		static double[][] inputSets = new double[1][];
		static double[][] outputSets = new double[1][];
		private static double[][] testSetOutput;
		private static double[][] testSetInput;
		private static double[][] trainSetOutput;
		private static double[][] trainSetInput;

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
		public Random random;
		private int randomSeed;
		private static LASTINPUTINFO lastInPutNfo;
		public static int ThreadSleepTime;

		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		private struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}

		public Train()
		{
			randomSeed = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;
			random = new Random(randomSeed);
			thread = new Thread(ProcessScan);
		}

		public void Stop()
		{
			thread.Abort();
		}

		public void Start()
		{
			ClearParameters();

			ThreadSleepTime = Configuration.SleepTime;

			RunScan = true;
			thread.Priority = ThreadPriority.Lowest;
			thread.Start();
		}

		public static int GetIdleTickCount()
		{
			return (Environment.TickCount - GetLastInputTime());
		}

		public static int GetLastInputTime()
		{
			lastInPutNfo.cbSize = (uint) Marshal.SizeOf(lastInPutNfo);
			if (!GetLastInputInfo(ref lastInPutNfo))
				debug($"ERROR: GetLastInputInfo: {Marshal.GetLastWin32Error()}");

			return (int) lastInPutNfo.dwTime;
		}

		public void EraseBigLabel()
		{
			Program.Form.debugView.Invoke((MethodInvoker) (() => { Data.chartBigLabel = string.Empty; }));
		}

		public void SetBigLabel(string text = "")
		{
			Program.Form.debugView.Invoke((MethodInvoker) (() =>
			{
				Data.chartBigLabel = text.Length > 0 ? text : $"[MUTATING DATA {Data.loadPercent,4:####}%]";
			}));
		}

		/*
	
				+------+.      +------+       +------+       +------+      .+------+
		|`.    | `.    |\     |\      |      |      /|     /|    .' |    .'|
		|  `+--+---+   | +----+-+     +------+     +-+----+ |   +---+--+'  |
		|   |  |   |   | |    | |     |      |     | |    | |   |   |  |   |
		+---+--+.  |   +-+----+ |     +------+     | +----+-+   |  .+--+---+
		 `. |    `.|    \|     \|     |      |     |/     |/    |.'    | .'
		   `+------+     +------+     +------+     +------+     +------+'
		*/

		public void ProcessScan()
		{
			if (!Data.TALibMethods.Any())
				return;

			do
			{
				Program.Form.ConfigurationClear();

				again:

				ThreadSleepTime = GetIdleTickCount() >= Configuration.SleepCheckTime ? 0 : Configuration.SleepTime;

				Program.Form.setStatus($"generating functions list, sleepTime={ThreadSleepTime}");

				class1 = class2 = class0 = 0;
				Data.FunctionsBase.Clear();
				Program.Form.debugView.Invoke((MethodInvoker) (() => { Program.Form.debugView.Items.Clear(); }));

				SetupFunctions(randomSeed);

				InputDimension = random.Next(8, 8 * (random.Next(8, 16)));
				Program.Form.AddConfiguration($"InputDimension: {InputDimension}\r\n");

				debug($"function setup done, generating data [inputDimension={InputDimension}] ...");

				for (int offset = 0; offset < Data.ForexPrices.Count && RunScan; offset += InputDimension)
				{
					Program.Form.setBigLabel($"Generating train/test data ...");
					if (offset % 155 == 0)
						Program.Form.setStatus($"Generating train&&test data [{offset} - {offset + InputDimension}] ...");

					combinedResult = new double[] { };

					foreach (var funct in Data.FunctionsBase)
					{
						var functionInfo = funct.Value;

						randomSeed = (int) functionInfo["randomseed"];
						FunctionParameters functionParameters = new FunctionParameters((MethodInfo) functionInfo["methodInfo"], InputDimension, offset);

						// execute function
						Function.Function function = new Function.Function((MethodInfo) functionInfo["methodInfo"]);
						result = function.Execute(functionParameters, out var code);
						if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
						    double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayAllZeros(result))
						{
							debug($"WARNING: skip {((MethodInfo) functionInfo["methodInfo"]).Name} due to bad output [len={result.Length}, code={code}]");
							Program.Form.setStatus($"ERROR: bad output for {((MethodInfo) functionInfo["methodInfo"]).Name}");
							goto again;
						}

						// copy new output to all data
						if (combinedResult != null)
							prevOffset = combinedResult.Length;

						Array.Resize(ref combinedResult, (combinedResult?.Length ?? 0) + result.Length);
						Array.Copy(result, 0, combinedResult, prevOffset, result.Length);
					}

					// generate train data set
					Array.Resize(ref inputSets, numRecord + 1);
					Array.Resize(ref outputSets, numRecord + 1);

					inputSets[numRecord] = new double[combinedResult.Length];
					outputSets[numRecord] = new double[2];

					if (numRecord % 145 == 0)
						debug(
							$"offset: {offset} numRecord:{numRecord} inputSets:{inputSets.Length} outputSets:{outputSets.Length} combinedResult:{combinedResult.Length}");

					Array.Copy(combinedResult, inputSets[numRecord], combinedResult.Length);

					SetOutputResult(InputDimension, offset, numRecord);

					numRecord++;
				}

				if (!RunScan)
					return;

				TrainNetwork(ref inputSets, ref outputSets);
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

		private void SetupFunctions(int randomSeedLocal)
		{
			int functionsCount = random.Next(Configuration.MinTaFunctionsCount, Configuration.MinTaFunctionsCount + 8);

			debug($"selecting functions Count={functionsCount}");
			Program.Form.ConfigurationClear();

			Program.Form.AddConfiguration("Functions:\r\n");

			Program.Form.EraseBigLabel();

			Program.Form.setBigLabel("[SETTING FUNCTIONS UP]");

			for (int i = 0; i < functionsCount && RunScan; i++)
			{
				Program.Form.setBigLabel($"[SETUP FUNCTION #{i}]");
				//ThreadSleepTime = GetIdleTickCount() >= Configuration.SleepCheckTime ? 0 : Configuration.SleepTime;
				//Thread.Yield();
				//Thread.Sleep(ThreadSleepTime);

				int unixTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;

				// get random method
				var methodInfo = Methods.GetRandomMethod(unixTimestamp);

				Program.Form.setStatus($"Setup function #{i} <{methodInfo.Name}> ...");

				debug($"Selected function #{i}: {methodInfo.Name} unixTimestamp: {unixTimestamp}");
				if (Data.FunctionsBase.ContainsKey(methodInfo.Name))
				{
					debug("this functions already exist");
					if (i > 0)
						i--;
					continue;
				}

				// generate parameters
				FunctionParameters functionParameters = new FunctionParameters(methodInfo, InputDimension, 0);

				// execute function
				var function = new Function.Function(methodInfo);
				result = function.Execute(functionParameters, out var code);

				if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
				    double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayAllZeros(result))
				{
					DumpValues(methodInfo, result);
					debug($"WARNING: skip {methodInfo.Name} due to bad output [len={result.Length}, code={code}], need {Configuration.MinTaFunctionsCount - i}");
					if (i > 0)
						i--;
					randomSeedLocal = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;
					continue;
				}

				Program.Form.AddConfiguration($" {methodInfo.Name} seed={randomSeedLocal}\r\n{functionParameters.parametersMap}");

				// record info
				Data.FunctionsBase[methodInfo.Name] = new Dictionary<string, object>();
				Data.FunctionsBase[methodInfo.Name]["parameters"] = functionParameters;
				Data.FunctionsBase[methodInfo.Name]["result"] = result;
				Data.FunctionsBase[methodInfo.Name]["randomseed"] = randomSeedLocal;
				Data.FunctionsBase[methodInfo.Name]["methodInfo"] = methodInfo;

				randomSeedLocal = unixTimestamp + random.Next(255);
			}

			var functions = new StringBuilder();

			foreach (var func in Data.FunctionsBase)
				functions.Append($"[{func.Key}] ");

			Program.Form.funcListLabel.Invoke((MethodInvoker) (() => { Program.Form.funcListLabel.Text = functions.ToString(); }));
		}

		/*		_______________________________________________________
						  |                                                      |
					 /    |                                                      |
					/---, |                                                      |
			   -----# ==| |                                                      |
			   | :) # ==| |                                                      |
		  -----'----#   | |______________________________________________________|
		  |)___()  '#   |______====____   \___________________________________|
		 [_/,-,\"--"------ //,-,  ,-,\\\   |/             //,-,  ,-,  ,-,\\ __#
		   ( 0 )|===******||( 0 )( 0 )||-  o              '( 0 )( 0 )( 0 )||
----'-'--------------'-'--'-'-----------------------'-'--'-'--'-'--------------*/

		public double CalculateHitRatio(Network net, double[][] inputs, double[][] desired_outputs)
		{
			int hits = 0, curX = 0;
			foreach (double[] input in inputs)
			{
				var output = net.Run(input);

				//var output0 = desired_outputs[curX][0] == -1 ? Math.Floor(output[0]) : Math.Ceiling(output[0]);
				//var output1 = desired_outputs[curX][1] == -1 ? Math.Floor(output[1]) : Math.Ceiling(output[1]);
				double output0, output1;
				if (output[0] >= 0.5)
					output0 = 1.0;
				else if (output[0] <= -0.5)
					output0 = -1.0;
				else
					output0 = 0;

				if (output[1] >= 0.5)
					output1 = 1.0;
				else if (output[1] <= -0.5)
					output1 = -1.0;
				else
					output1 = 0;

				if (output0 == desired_outputs[curX][0] && output1 == desired_outputs[curX][1])
				{
					hits++;
				}
				else
				{
					//debug($"output[0]({output[0]}/{Math.Ceiling(output[0])}) != desired[0]({desired_outputs[curX][0]})");
					//debug($"output[1]({output[1]}/{Math.Ceiling(output[1])}) != desired[1]({desired_outputs[curX][1]})");
				}
				curX++;
			}
			return ((double) hits / (double) inputs.Length) * 100.0;
		}

		private bool CheckInputDataIsCorrect(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
		{
			Program.Form.setStatus($"Checking training data: input {inputSetsLocal?.Length} output {outputSetsLocal?.Length} ...");

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
					if (double.IsPositiveInfinity(cell) || double.IsNegativeInfinity(cell) || double.IsInfinity(cell) || double.IsNaN(cell))
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
					if (double.IsPositiveInfinity(cell) || double.IsNegativeInfinity(cell) || double.IsInfinity(cell) || double.IsNaN(cell))
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
			double testMse = 0;
			double trainMse = 0;

			Program.Form.setBigLabel($"[CHECK {inputSetsLocal.Length} DATA ROWS]");
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
			debug($"SetTrainData: inpustSetsLocal.Length: {inputSetsLocal.Length} outputSetsLocal.Length: {outputSetsLocal.Length} ");

			TrainingData trainData;
			TrainingData testData;
			uint testDataOffset;
			try
			{
				trainData = new TrainingData();
				trainData.SetTrainData(inputSetsLocal, outputSetsLocal);
				trainData.ScaleTrainData(-1.0, 1.0);

				testData = new TrainingData(trainData);
				testDataOffset = trainData.TrainDataLength / Configuration.TestDataAmountPerc;
				testData.SubsetTrainData(0, testDataOffset);
				testData.SaveTrain(@"d:\temp\testdata.dat");
				testSetInput = testData.Input;
				testSetOutput = testData.Output;

				trainData.SubsetTrainData(testDataOffset, trainData.TrainDataLength - testDataOffset);
				trainData.SaveTrain(@"d:\temp\traindata.dat");
				trainSetInput = trainData.Input;
				trainSetOutput = trainData.Output;
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

			Program.Form.AddConfiguration(
				$"Info:\r\n inputSets: {inputSetsLocal.Length}\r\n Train: {trainData.TrainDataLength - testDataOffset} Test: {testDataOffset}\r\n");

			debug($"class1: {class1} class2: {class2} class0: {class0}");

			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				Program.Form.EraseBigLabel();
				Program.Form.chart.Series.Clear();
				Program.Form.chart.Series.Add("train");
				Program.Form.chart.Series.Add("test");
				Program.Form.chart.Series["test"].ChartType = SeriesChartType.Line;
				Program.Form.chart.Series["train"].ChartType = SeriesChartType.Line;

				Program.Form.chart.Series["train"].BorderWidth = 2;
				Program.Form.chart.Series["test"].BorderWidth = 2;

				Program.Form.chart.Series[0].Color = Color.Green;
				Program.Form.chart.Series[1].Color = Color.Blue;

				Program.Form.chart.ChartAreas[0].AxisX.LineColor = Color.White;
				Program.Form.chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
				Program.Form.chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;

				Program.Form.chart.ChartAreas[0].AxisY.LineColor = Color.White;
				Program.Form.chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
				Program.Form.chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

				/*Program.Form.chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
				Program.Form.chart.ChartAreas[0].AxisX.Interval = 1;
				Program.Form.chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
				Program.Form.chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
				Program.Form.chart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
				Program.Form.chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;*/
				Program.Form.chart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = 0.3;
			}));

			// create network to hold all input data
			var inputCount = trainData.InputCount;
			uint numNeurons = Configuration.DefaultHiddenNeurons > 0 ? Configuration.DefaultHiddenNeurons : inputCount / 2 - 1;
			debug($"new network: numinputs: {inputCount} neurons: {numNeurons}");

			Program.Form.AddConfiguration($"Network:\r\n inputs: {inputCount} neurons: {numNeurons}");

			network = new Network(inputCount, numNeurons, 2);

			network.TrainingAlgorithm = Configuration.TrainAlgo;

			network.InitWeights(trainData);
			network.SetupActivation();

			debug($"starting train on network {network.GetHashCode()}");

			double saveTestHitRatio = 0;
			
			for (var currentEpoch = 0; RunScan && inputSetsLocal != null && outputSetsLocal != null; currentEpoch++)
			{
				double TestHitRatio = 0.0, TrainHitRatio = 0.0;
				if (currentEpoch >= Configuration.TrainLimitEpochs)
				{
					debug("[AUTO-RESTART]");
					Program.Form.setStatus("AUTO-RESTARTING ...");
					ClearParameters();
					return -1;
				}

				if (Program.Form.nodelayCheckbox.Checked == false)
				{
					ThreadSleepTime = GetIdleTickCount() >= Configuration.SleepCheckTime ? 0 : Configuration.SleepTime;

					Thread.Sleep(ThreadSleepTime);
				}

				// DO TRAIN
				trainMse = network.Train(trainData);
				if (network.ErrNo > 0)
					debug($"error {network.ErrNo}: {network.ErrStr}");

				testMse = network.Test(testData);
				if (network.ErrNo > 0)
					debug($"error test {network.ErrNo}: {network.ErrStr}");

				TestHitRatio = CalculateHitRatio(network, testSetInput, testSetOutput);
				TrainHitRatio = CalculateHitRatio(network, trainSetInput, trainSetOutput);

				Program.Form.setStatus(
					$"[Training] TrainMSE {trainMse,-7:0.#####} {TrainHitRatio,-5:0.##}% TestMSE {testMse,-7:0.#####} {TestHitRatio,-5:0.##}% DELAY {ThreadSleepTime}  ");

				debug($"train: epoch #{currentEpoch} trainMse {trainMse,7:0.#####} {TrainHitRatio,3:0.##}% testmse {testMse,7:0.#####} {TestHitRatio,3:0.##}%");

				var mse = trainMse;
				var mse1 = testMse;
				var epoch1 = currentEpoch;

				if (trainMse <= 0.01 && currentEpoch > Configuration.MinSaveEpoch)
				{
					debug($"finished training, reached corner trainmse={trainMse} testmse={testMse}");
					break;
				}

				if ((testMse <= Configuration.MinSaveTestMSE || TestHitRatio >= Configuration.MinSaveHit) && currentEpoch > Configuration.MinSaveEpoch &&
				    saveTestHitRatio < TestHitRatio)
				{
					saveTestHitRatio = TestHitRatio;
					SaveNetwork();
				}

				Program.Form.chart.Invoke((MethodInvoker) (() =>
				{
					Program.Form.chart.Series["train"].Points.AddXY(epoch1, mse);
					Program.Form.chart.Series["test"].Points.AddXY(epoch1, mse1);
				}));
			}

			var output = network.Run(inputSetsLocal[0]);
			testMse = network.Test(testData);
			debug($"trained mse {trainMse} testmse {testMse} 0:should={outputSetsLocal[0][0]}");
			debug($"is={output[0]} should={outputSetsLocal[0][1]} is={output[1]} ");

			output = network.Run(inputSetsLocal[0]);
			testMse = network.Test(testData);
			Program.Form.setStatus($"[Done] Trainmse {trainMse,6:0.####} Testmse {testMse,6:0.####} . should={outputSetsLocal[0][1]} is={output[1]}.");

			return 0;
		}

/*
			    \\         //
				 \\     //
				   \\ //
					(O)
				   //#\\
				 // ### \\
			   //  #####  \\
				  #######
				  ### ###
			'' """  """"  "'"""""
*/

		private static void SaveNetwork()
		{
			string netDirectory = $"{network.GetHashCode():x}";

			if (!Directory.Exists($"d:\\temp\\forexAI\\{netDirectory}"))
				Directory.CreateDirectory($"d:\\temp\\forexAI\\{netDirectory}");

			network.Save($@"d:\temp\forexAI\{netDirectory}\FANN.net");

			File.Copy("d:\\temp\\traindata.dat", $@"d:\temp\forexAI\{netDirectory}\traindata.dat", true);
			File.Copy("d:\\temp\\testdata.dat", $@"d:\temp\forexAI\{netDirectory}\testdata.dat", true);

			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				Program.Form.chart.SaveImage($@"d:\temp\forexAI\{netDirectory}\chart.jpg", ChartImageFormat.Jpeg);

				using (var tw = new StreamWriter($@"d:\temp\forexAI\{netDirectory}\debug.log"))
				{
					foreach (var item in Program.Form.debugView.Items)
						tw.WriteLine(item.ToString());
				}

				using (var cf = new StreamWriter($@"d:\temp\forexAI\{netDirectory}\configuration.txt"))
				{
					cf.WriteLine(Program.Form.configurationTab.Text);
				}
			}));
		}

		private static void SetOutputResult(int inputDimension, int offset, int numRecordLocal)
		{
			double[] priceOpen = ForexPrices.GetClose(inputDimension, offset);

			if (priceOpen[inputDimension - 1] > priceOpen[inputDimension > 10 ? inputDimension - 10 : inputDimension - 1])
			{
				outputSets[numRecordLocal][0] = 1;
				outputSets[numRecordLocal][1] = -1;
				class1++;
			}
			else if (priceOpen[inputDimension - 1] <= priceOpen[inputDimension > 10 ? inputDimension - 10 : inputDimension - 1])
			{
				outputSets[numRecordLocal][0] = -1;
				outputSets[numRecordLocal][1] = 1;
				class2++;
			}
			else
			{
				outputSets[numRecordLocal][0] = 0;
				outputSets[numRecordLocal][1] = 0;
				class0++;
			}
		}
	}
}