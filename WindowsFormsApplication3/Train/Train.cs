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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FANNCSharp;
using FANNCSharp.Double;
using FinancePermutator.Generators;
using FinancePermutator.Networks;
using FinancePermutator.Prices;
using Newtonsoft.Json;
using static FinancePermutator.Tools;

namespace FinancePermutator.Train
{
    class Train
    {
        public static int networksProcessed = 0;
        public static int networksBad = 0;
        public static int numberOfFailedFunctions = 0;
        double saveTestHitRatio = 0;
        public static int numberOfBrokenData = 0;
        static int inputDimension = Configuration.InputDimension;
        static double[][] inputSets = new double[1][];
        static double[][] outputSets = new double[1][];
        private static double[][] testSetOutput;
        private static double[][] testSetInput;
        private static double[][] trainSetOutput;
        private static double[][] trainSetInput;
        public static int networksSuccess = 0;
        TrainingData trainData;
        TrainingData testData;
        uint testDataOffset;
        static double[] combinedResult;
        static double[] result;
        static int numRecord;
        static int prevOffset;
        public bool runScan = true;
        public static int class1;
        public static int class2;
        public static int class0;
        private static Thread generateFunctionsThread;
        private static Network network;
        private int randomSeed;
        private static LASTINPUTINFO lastInPutNfo;
        public static int threadSleepTime;
        double testHitRatio = 0.0, trainHitRatio = 0.0;
        double testMse = 0;
        double trainMse = 0;
        bool noDelayEnabled;

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public Train()
        {
            randomSeed = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;
            generateFunctionsThread = new Thread(LoopGenerate);
        }

        public void Stop()
        {
            generateFunctionsThread.Abort();
        }

        public void Start()
        {
            ClearAllParameters();

            threadSleepTime = Configuration.SleepTime;
            runScan = true;
            generateFunctionsThread.Priority = ThreadPriority.Lowest;
            generateFunctionsThread.Start();
        }

        public static int GetIdleTickCount()
        {
            return Environment.TickCount - GetLastInputTime();
        }

        public static int GetLastInputTime()
        {
            lastInPutNfo.cbSize = (uint)Marshal.SizeOf(lastInPutNfo);
            if (!GetLastInputInfo(ref lastInPutNfo))
                debug($"ERROR: GetLastInputInfo: {Marshal.GetLastWin32Error()}");

            return (int)lastInPutNfo.dwTime;
        }

        public void EraseBigLabel()
        {
            Program.Form.debugView.Invoke((MethodInvoker)(() =>
           {
               Program.Form.debugView.Invoke((MethodInvoker)(() => { Data.chartBigLabel = string.Empty; }));
           }));
        }

        public void SetBigLabel(string text = "")
        {
            Program.Form.debugView.Invoke((MethodInvoker)(() =>
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

        public void LoopGenerate()
        {
            if (!Data.TALibMethods.Any())
                return;

            do
            {
                again:
                Program.Form.ConfigurationClear();

                Program.Form.debugView.Invoke((MethodInvoker)(() =>
                {
                    noDelayEnabled = Program.Form.nodelayCheckbox.Checked;
                }));

                threadSleepTime = GetIdleTickCount() >= Configuration.SleepCheckTime ? 0 : Configuration.SleepTime;

                Program.Form.SetStatus($"generating functions list, sleepTime={threadSleepTime}");

                class1 = class2 = class0 = 0;
                Data.FunctionBase.Clear();
                Program.Form.debugView.Invoke((MethodInvoker)(() => { Program.Form.debugView.Items.Clear(); }));

                SetupFunctions(randomSeed);

                inputDimension = XRandom.next(8, (XRandom.next(9, Configuration.InputDimension)));

                Program.Form.AddConfiguration($"\r\nInputDimension: {inputDimension}\r\n");

                debug($"function setup done, generating data [inputDimension={inputDimension}] ...");

                for (int offset = 0; offset < Data.ForexPrices.Count && runScan; offset += inputDimension)
                {
                    Program.Form.SetBigLabel($"Generating train/test data ...");

                    if (offset % 55 == 0)
                        Program.Form.SetStatus(
                            $"Generating train && test data [{offset} - {offset + inputDimension}] {(double)offset / Data.ForexPrices.Count * 100.0,2:0.##}% ...");

                    combinedResult = new double[] { };

                    foreach (var funct in Data.FunctionBase)
                    {
                        if (noDelayEnabled == false)
                            Thread.Sleep(1);

                        var functionInfo = funct.Value;

                        FunctionParameters functionParameters = new FunctionParameters((MethodInfo)functionInfo["methodInfo"],
                            inputDimension, offset);

                        // execute function
                        Function function = new Function((MethodInfo)functionInfo["methodInfo"]);
                        result = function.Execute(functionParameters, out var code);

                        // check function output
                        if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
                            double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayRepeating(result))
                        {
                            debug($"WARNING: skip {((MethodInfo)functionInfo["methodInfo"]).Name} due to bad output [len={result.Length}, code={code}]");
                            Program.Form.SetStatus($"ERROR: bad output for {((MethodInfo)functionInfo["methodInfo"]).Name}");
                            numberOfBrokenData++;
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

                    if (numRecord % 245 == 0)
                        debug(
                            $"offset: {offset} numRecord:{numRecord} inputSets:{inputSets.Length} outputSets:{outputSets.Length} combinedResult:{combinedResult.Length}");

                    Array.Copy(combinedResult, inputSets[numRecord], combinedResult.Length);

                    SetOutputResult(inputDimension, offset, numRecord);

                    numRecord++;
                }

                TrainNetwork(ref inputSets, ref outputSets);
            } while (runScan);

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
            SetStats();
            int functionsCount = XRandom.next(Configuration.MinTaFunctionsCount, Configuration.MinTaFunctionsCount + 8);

            debug($"selecting functions Count={functionsCount}");
            Program.Form.ConfigurationClear();
            //Program.Form.AddConfiguration("Functions:\r\n");

            Program.Form.EraseBigLabel();
            Program.Form.SetBigLabel("[SETTING FUNCTIONS UP]");

            for (int i = 0; i < functionsCount && runScan; i++)
            {
                SetStats();
                Program.Form.SetBigLabel($"[SETUP FUNCTION #{i}]");

                int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + DateTime.Now.Millisecond;

                // get random method
                var methodInfo = Methods.GetRandomMethod(unixTimestamp);

                Program.Form.SetStatus($"Setup function #{i} <{methodInfo.Name}> ...");
                debug($"Selected function #{i}: {methodInfo.Name} unixTimestamp: {unixTimestamp}");

                if (Data.FunctionBase.ContainsKey(methodInfo.Name))
                {
                    debug($"function {methodInfo.Name} already exist");
                    if (i > 0)
                        i--;
                    numberOfFailedFunctions++;
                    continue;
                }

                // generate parameters
                FunctionParameters functionParameters = new FunctionParameters(methodInfo, inputDimension, 0);

                // execute function 
                var function = new FinancePermutator.Function(methodInfo);
                result = function.Execute(functionParameters, out var code);

                if (result == null || result.Length <= 1 || double.IsNegativeInfinity(result[0]) || double.IsPositiveInfinity(result[0]) ||
                    double.IsNaN(result[0]) || double.IsInfinity(result[0]) || IsArrayRepeating(result))
                {
                    DumpValues(methodInfo, result);
                    debug(
                        $"WARNING: skip {methodInfo.Name} due to bad output [len={result.Length}, code={code} InputDimension={inputDimension}], need {Configuration.MinTaFunctionsCount - i}");
                    if (i > 0)
                        i--;
                    numberOfFailedFunctions++;
                    continue;
                }

                //Program.Form.AddConfiguration($" [{methodInfo.Name} \r\n{functionParameters.parametersMap}] \r\n =====================\r\n");

                // record info
                Data.FunctionBase[methodInfo.Name] = new Dictionary<string, object>();
                Data.FunctionBase[methodInfo.Name]["parameters"] = functionParameters;
                Data.FunctionBase[methodInfo.Name]["methodInfo"] = methodInfo;

                randomSeedLocal = unixTimestamp + XRandom.next(255);
            }

            var functions = new StringBuilder();

            foreach (var func in Data.FunctionBase)
                functions.Append($"[{func.Key}] ");

            Program.Form.funcListLabel.Invoke((MethodInvoker)(() => { Program.Form.funcListLabel.Text = functions.ToString(); }));

            //debug($"{JsonConvert.SerializeObject(Data.FunctionBase, Formatting.Indented)}");
            //Thread.Sleep(500000);
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

        public double CalculateHitRatio(Network net, double[][] inputs, double[][] desiredOutputs)
        {
            int hits = 0, curX = 0;
            foreach (double[] input in inputs)
            {
                var output = net.Run(input);

                double output0 = 0;
                if (output[0] >= 0.0)
                    output0 = 1.0;
                else if (output[0] < -0.0)
                    output0 = -1.0;

                double output1 = 0;
                if (output[1] >= 0.0)
                    output1 = 1.0;
                else if (output[1] < -0.0)
                    output1 = -1.0;

                if (output0 == desiredOutputs[curX][0] && output1 == desiredOutputs[curX][1])
                    hits++;
                curX++;
            }
            return ((double)hits / (double)inputs.Length) * 100.0d;
        }

        private bool AssertInputDataIsCorrect(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
        {
            Program.Form.SetStatus($"Checking training data: input {inputSetsLocal?.Length} output {outputSetsLocal?.Length} ...");

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
                {
                    debug($"ERROR: input {i} is NULL! (Length:{inputSetsLocal.Length})");
                    return false;
                }

            for (var i = 0; i < outputSetsLocal.Length; i++)
                if (outputSetsLocal[i] == null || outputSetsLocal[i].Length == 0)
                {
                    debug($"ERROR: output {i} is NULL! (Length:{outputSetsLocal.Length})");
                    return false;
                }

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
                        return false;
                    }
                    k++;
                }
                k = 0;
                j++;
            }

            return true;
        }

        private void ClearAllParameters()
        {
            outputSets = null;
            inputSets = null;
            numRecord = 0;
        }

        private void CreateTrainAndTestData(double[][] inputSetsLocal, double[][] outputSetsLocal)
        {
            try
            {
                trainData = new TrainingData();
                trainData.SetTrainData(inputSetsLocal, outputSetsLocal);
                trainData.ScaleTrainData(-1.0, 1.0);

                testData = new TrainingData(trainData);
                testDataOffset = trainData.TrainDataLength / (uint)Configuration.TestDataAmountPerc;
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
                Program.Form.SetStatus($"Exception '{e.Message}' while working with train data");
                ClearAllParameters();
            }
        }

        private void InitChart()
        {
            Program.Form.chart.Invoke((MethodInvoker)(() =>
           {
               Program.Form.EraseBigLabel();

               Program.Form.chart.Series.Clear();
               Program.Form.chart.Series.Add("train");
               Program.Form.chart.Series.Add("test");

               Program.Form.chart.Series["train"].ChartType = SeriesChartType.Line;
               Program.Form.chart.Series["test"].ChartType = SeriesChartType.FastLine;

               Program.Form.chart.Series["train"].BorderWidth = 2;
               Program.Form.chart.Series["test"].BorderWidth = 2;

               Program.Form.chart.Series["train"].Color = Color.Green;
               Program.Form.chart.Series["test"].Color = Color.Blue;

               Program.Form.chart.ChartAreas[0].AxisX.LineColor = Color.White;
               Program.Form.chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
               Program.Form.chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;

               Program.Form.chart.ChartAreas[0].AxisY.LineColor = Color.White;
               Program.Form.chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
               Program.Form.chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

               Program.Form.chart.ChartAreas[0].AxisY.Interval = 0.1;
               /*Program.Form.chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
               Program.Form.chart.ChartAreas[0].AxisX.Interval = 1;
               Program.Form.chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
               Program.Form.chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
               Program.Form.chart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
               Program.Form.chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;*/
               Program.Form.chart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = 0.3;
           }));
        }

        private void CreateNetwork()
        {
            if (trainData == null)
                return;

            // create network to hold all input data
            var inputCount = trainData.InputCount;
            // TODO: это бред, надо исправить
            uint numNeurons = Configuration.DefaultHiddenNeurons > 0 ? Configuration.DefaultHiddenNeurons : inputCount / 2 - 1;
            debug($"new network: numinputs: {inputCount} neurons: {numNeurons}");

            Program.Form.AddConfiguration($"\r\nConfig hash: {XRandom.randomString()}\r\n\r\nNetwork:\r\n inputs: {inputCount} neurons: {numNeurons}");

            NetworkType layer = NetworkType.LAYER;
            network = new Network(layer, inputCount, numNeurons, 2);
            network.TrainingAlgorithm = Configuration.TrainAlgo;
            network.InitWeights(trainData);
            network.SetupActivation();
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

        public static void SetStats()
        {
            Program.Form.SetStats($"{Train.networksProcessed,6:0} networks processed \r\n{Train.networksBad,6:0} bad networks \r\n" +
                                  $"{Train.networksSuccess,6:0} successful networks " + $"\r\n{Train.numberOfBrokenData,6:0} broken data " +
                                  $"\r\n{Train.numberOfFailedFunctions,6:0} failed functions");
        }

        private int PrepareNetwork(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
        {
            Program.Form.SetBigLabel($"[CHECK {inputSetsLocal.Length} DATA ROWS]");
            if (!AssertInputDataIsCorrect(ref inputSetsLocal, ref outputSetsLocal))
            {
                debug($"ERROR: data check failed");
                ClearAllParameters();
                numberOfBrokenData++;
                return -1;
            }

            // load train data
            debug($"SetTrainData: inpustSetsLocal.Length: {inputSetsLocal.Length} outputSetsLocal.Length: {outputSetsLocal.Length} ");
            CreateTrainAndTestData(inputSetsLocal, outputSetsLocal);

            if (inputSetsLocal != null && outputSetsLocal != null && trainData != null)
                Program.Form.AddConfiguration(
                    $"\r\nInfo:\r\n inputSets: {inputSetsLocal.Length}\r\n Train: {trainData.TrainDataLength - testDataOffset} Test: {testDataOffset}\r\n" +
                    $" class1: {class1} class2: {class2} class0: {class0}");

            debug($"class1: {class1} class2: {class2} class0: {class0}");
            InitChart();
            CreateNetwork();
            networksProcessed++;

            return 0;
        }

        private int TrainNetwork(ref double[][] inputSetsLocal, ref double[][] outputSetsLocal)
        {
            if (PrepareNetwork(ref inputSetsLocal, ref outputSetsLocal) == -1)
                return -1;

            if (network == null)
                return -1;

            debug($"starting train on network #{networksProcessed} id: 0x{network.GetHashCode():X}");
            saveTestHitRatio = 0;

            for (var currentEpoch = 0; runScan && inputSetsLocal != null && outputSetsLocal != null; currentEpoch++)
            {
                // stop if max epoch reached
                if (currentEpoch >= Configuration.TrainLimitEpochs)
                {
                    debug("[AUTO-RESTART]");
                    Program.Form.SetStatus("AUTO-RESTARTING ...");
                    ClearAllParameters();
                    networksBad++;
                    return -1;
                }

                // stop if no progress
                if (currentEpoch >= 95 && (testHitRatio <= 3 && trainHitRatio <= 3))
                {
                    debug("fail to train, bad network");
                    networksBad++;
                    break;
                }

                // stop if reached low train mse
                if (trainMse <= 0.01 && currentEpoch > Configuration.MinSaveEpoch)
                {
                    debug($"finished training, reached corner trainmse={trainMse} testmse={testMse}");
                    networksBad++;
                    break;
                }

                // sleep for delay seconds (if enabled)
                if (Program.Form.nodelayCheckbox.Checked == false)
                {
                    threadSleepTime = GetIdleTickCount() >= Configuration.SleepCheckTime ? 0 : Configuration.SleepTime;
                    Thread.Sleep(threadSleepTime);
                }

                // DO TRAIN
                trainMse = network.Train(trainData);
                if (network.ErrNo > 0)
                    debug($"error {network.ErrNo}: {network.ErrStr}");

                // DO TEST
                testMse = network.Test(testData);
                if (network.ErrNo > 0)
                    debug($"error test {network.ErrNo}: {network.ErrStr}");

                // Calcuate hit ratio in percentage
                testHitRatio = CalculateHitRatio(network, testSetInput, testSetOutput);
                trainHitRatio = CalculateHitRatio(network, trainSetInput, trainSetOutput);

                // save network if hit ratio reached
                if ((testMse <= Configuration.MinSaveTestMSE || testHitRatio >= Configuration.MinSaveHit) && currentEpoch > Configuration.MinSaveEpoch &&
                    saveTestHitRatio < testHitRatio)
                {
                    saveTestHitRatio = testHitRatio;
                    SaveNetwork();
                    networksSuccess++;
                }

                // draw graphics
                var epoch = currentEpoch;
                Program.Form.chart.Invoke((MethodInvoker)(() =>
               {
                   Program.Form.chart.Series["train"].Points.AddXY(epoch, trainMse);
                   Program.Form.chart.Series["test"].Points.AddXY(epoch, testMse);

                   Program.Form.chart.Series["train"].LegendText = $"Train {trainHitRatio,2:0.##}%";
                   Program.Form.chart.Series["test"].LegendText = $"Test {testHitRatio,2:0.##}%";
               }));

                // set various statuses
                string training = epoch % 2 == 0 ? "TRAINING" : "        ";
                Program.Form.SetStatus($"[{training}] TrainMSE {trainMse,7:0.#####} {trainHitRatio,4:0.##}% TestMSE {testMse,7:0.#####} {testHitRatio,4:0.##}% ");
                debug(
                    $"train: epoch #{currentEpoch,-4:0} trainMse {trainMse,8:0.#####} {trainHitRatio,4:0.##}% testmse {testMse,8:0.#####} {testHitRatio,4:0.##}%");
            }

            var output = network.Run(inputSetsLocal[0]);
            testMse = network.Test(testData);
            debug($"trained mse {trainMse} testmse {testMse} 0:should={outputSetsLocal[0][0]}");
            debug($"is={output[0]} should={outputSetsLocal[0][1]} is={output[1]} ");

            output = network.Run(inputSetsLocal[0]);
            testMse = network.Test(testData);
            Program.Form.SetStatus($"[Done] Trainmse {trainMse,6:0.####} Testmse {testMse,6:0.####} . should={outputSetsLocal[0][1]} is={output[1]}.");

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
            string netDirectory = $"NET_{network.GetHashCode():X}";

            if (!Directory.Exists($"d:\\temp\\forexAI\\{netDirectory}"))
                Directory.CreateDirectory($"d:\\temp\\forexAI\\{netDirectory}");

            network.Save($@"d:\temp\forexAI\{netDirectory}\FANN.net");

            File.Copy("d:\\temp\\traindata.dat", $@"d:\temp\forexAI\{netDirectory}\traindata.dat", true);
            File.Copy("d:\\temp\\testdata.dat", $@"d:\temp\forexAI\{netDirectory}\testdata.dat", true);

            Program.Form.chart.Invoke((MethodInvoker)(() =>
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

               using (var cf = new StreamWriter($@"d:\temp\forexAI\{netDirectory}\functions.json"))
               {
                   cf.WriteLine(JsonConvert.SerializeObject(Data.FunctionBase, Formatting.Indented));
               }

           }));
        }

        private static void SetOutputResult(int inputDimensionLocal, int offset, int numRecordLocal)
        {
            double[] priceOpen = ForexPrices.GetOpen(inputDimensionLocal, offset);

            if (priceOpen[inputDimensionLocal - Configuration.OutputIndex] >
                priceOpen[inputDimensionLocal - 1])
            {
                outputSets[numRecordLocal][0] = 1;
                outputSets[numRecordLocal][1] = -1;
                class1++;
            }
            else if (priceOpen[inputDimensionLocal - Configuration.OutputIndex] <=
                     priceOpen[inputDimensionLocal - 1])
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