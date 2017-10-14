using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public double TestData(TrainingData testData)
		{
			return this.network.TestData(testData);
		}

		public TrainingAlgorithm TrainingAlgorithm
		{
			get => network.TrainingAlgorithm;
			set => network.TrainingAlgorithm = value;
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
			this.network.SetActivationFunctionLayer(ActivationFunction.ELLIOT_SYMMETRIC, 1); //SIGMOID_SYMMETRIC
			this.network.SetActivationFunctionLayer(ActivationFunction.SIGMOID_SYMMETRIC_STEPWISE,
				2); //SIGMOID_SYMMETRIC_STEPWISE
			//LINEAR_PIECE_SYMMETRIC
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
			return this.network.TrainEpochSarpropParallel(trainData, 4); //TrainEpochIrpropmParallel
		}

		public double[] RunNetwork(double[] input)
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