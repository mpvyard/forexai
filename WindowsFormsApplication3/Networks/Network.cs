using System;
using System.IO;
using FANNCSharp;
using FANNCSharp.Double;
using FinancePermutator.Generators;
using Microsoft.AppCenter;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using static FinancePermutator.Tools;
using System.Windows.Forms;

/*
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
		░░░░░░░░░▄▄▌▌▄▌▌░░░░░*/

namespace FinancePermutator.Networks
{
	class Network
	{
		private NeuralNet network;
		public float MSE => network.MSE;
		public uint ErrNo => network.ErrNo;
		public string ErrStr => network.ErrStr;
		public uint BitFail => network.BitFail;
		public bool newNetwork = false;

		public double Test(TrainingData testData)
		{
			return network.TestDataParallel(testData, 4);
		}

		public TrainingAlgorithm TrainingAlgorithm
		{
			get => network.TrainingAlgorithm;
			set => network.TrainingAlgorithm = value;
		}

		public float SarpropStepErrorShift
		{
			get => network.SarpropStepErrorShift;
			set => network.SarpropStepErrorShift = value;
		}

		public float SarTemp
		{
			get { return network.SarpropTemperature; }

			set { network.SarpropTemperature = value; }
		}

		public void SetupActivation()
		{
			var activationFunc = ActivationFunction.SIGMOID_SYMMETRIC;
			Program.Form.AddConfiguration($"\r\n InputActFunc i: {activationFunc}");
			network.SetActivationFunctionLayer(activationFunc, 0);
			activationFunc = ActivationFunctionGenerator.GetRandomFunction();
			Program.Form.AddConfiguration($" LayerActFunc m: {activationFunc}");
			network.SetActivationFunctionLayer(activationFunc, 1);
			activationFunc = ActivationFunctionGenerator.GetRandomFunction();
			Program.Form.AddConfiguration($" LayerActFunc o: {activationFunc}");
			network.SetActivationFunctionLayer(activationFunc, 2);
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
		public void SaveNetwork()
		{
			string netDirectory = $"NET_{network.GetHashCode():X}";

			if(!Directory.Exists($"c:\\forexAI\\{netDirectory}"))
				Directory.CreateDirectory($"c:\\forexAI\\{netDirectory}");

			network.Save($@"c:\forexAI\{netDirectory}\FANN.net");

			File.Copy($@"{GetTempPath()}\traindata.dat", $@"c:\forexAI\{netDirectory}\traindata.dat", true);
			File.Copy($@"{GetTempPath()}\testdata.dat", $@"c:\forexAI\{netDirectory}\testdata.dat", true);

			Program.Form.chart.Invoke((MethodInvoker) (() =>
			{
				Program.Form.chart.SaveImage($@"c:\forexAI\{netDirectory}\chart.jpg", ChartImageFormat.Jpeg);

				using(var tw = new StreamWriter($@"c:\forexAI\{netDirectory}\debug.log"))
				{
					foreach(var item in Program.Form.debugView.Items)
						tw.WriteLine(item.ToString());
				}

				using(var cf = new StreamWriter($@"c:\forexAI\{netDirectory}\configuration.txt"))
				{
					cf.WriteLine(Program.Form.configurationTab.Text);
				}

				using(var cf = new StreamWriter($@"c:\forexAI\{netDirectory}\functions.json"))
				{
					cf.WriteLine(JsonConvert.SerializeObject(Data.FunctionConfiguration, Formatting.Indented));
				}

			}));
		}

		public void InitWeights(TrainingData trainData)
		{
			network.InitWeights(trainData);
		}

		public Network(NetworkType layer, uint numInput, uint numHidden, uint numOutput)
		{
			network = new NeuralNet(layer, 3, numInput, numHidden, numOutput);
			debug($"network {network} input: {numInput} numHidden: {numHidden} output: {numOutput}");
		}

		public double Train(TrainingData trainData)
		{
			network.SetScalingParams(trainData, -1.0f, 1.0f, -1.0f, 1.0f);
			return network.TrainEpochIrpropmParallel(trainData, 4);
		}

		public double[] Run(double[] input)
		{
			double[] outputData = network.Run(input);
			return outputData;
		}

		public void Save(string name)
		{
			debug($"saving network 0x{network.GetHashCode()} as {name}");
			network.Save(name);

			if(newNetwork)
			{
				Analytics.TrackEvent($"New network mined [0x{network.GetHashCode():x}]");
				newNetwork = false;
			}
		}

		internal void SetupScaling(TrainingData trainData)
		{
			network.SetScalingParams(trainData, -1.0f, 1.0f, -1.0f, 1.0f);
		}
	}
}