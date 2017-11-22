using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotless.Core.Parser.Tree;
using FANNCSharp;
using FANNCSharp.Double;
using static FinancePermutator.Tools;

namespace FinancePermutator.Networks
{
	class Network
	{
		private NeuralNet network;

		public float MSE => this.network.MSE;

		public uint ErrNo => this.network.ErrNo;

		public string ErrStr => this.network.ErrStr;

		public uint BitFail => this.network.BitFail;

		public double Test(TrainingData testData)
		{
			return this.network.TestDataParallel(testData, 4);
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
			get { return this.network.SarpropTemperature; }

			set { this.network.SarpropTemperature = value; }
		}

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
		public void SetupActivation()
		{
			var activationFunc = ActivationFunctionGeneration.GetRandomActivationFunction();
			Program.Form.AddConfiguration($"\r\n  InputActivationFunc: {activationFunc}");
			this.network.SetActivationFunctionLayer(activationFunc, 1); 
			activationFunc = ActivationFunctionGeneration.GetRandomActivationFunction();
			Program.Form.AddConfiguration($" LayerActivationFunc: {activationFunc}");
			this.network.SetActivationFunctionLayer(activationFunc, 2);
		}

		public void InitWeights(TrainingData td)
		{
			this.network.InitWeights(td);
		}

		public Network(uint numInput, uint numHidden, uint numOutput)
		{
			network = new NeuralNet(NetworkType.LAYER, 3, numInput, numHidden, numOutput);
			debug($"network {this.network} input: {numInput} numHidden: {numHidden} output: {numOutput}");
		}

		public double Train(TrainingData trainData)
		{
			return this.network.TrainEpochIrpropmParallel(trainData, 4);
			//TrainEpochSarpropParallel
			//TrainEpochIrpropmParallel
			// TrainEpochBatchParallel
			//TrainEpochQuickpropParallel
			// TrainEpochIncrementalMod
		}

		public double[] Run(double[] input)
		{
			var outputData = this.network.Run(input);
			return outputData;
		}

		public void Save(string name)
		{
			debug($"saving network {this.network.GetHashCode()} as {name}");
			this.network.Save(name);
		}
	}
}