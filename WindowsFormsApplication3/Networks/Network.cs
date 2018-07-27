using System;
using FANNCSharp;
using FANNCSharp.Double;
using FinancePermutator.Generators;
using static FinancePermutator.Tools;

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
			Program.Form.AddConfiguration($"\r\n InputActFunc: {activationFunc}");
			network.SetActivationFunctionLayer(activationFunc, 1);
			activationFunc = ActivationFunctionGenerator.GetRandomFunction();
			Program.Form.AddConfiguration($" LayerActFunc: {activationFunc}");
			network.SetActivationFunctionLayer(activationFunc, 2);
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
		}

		internal void SetupScaling(TrainingData trainData)
		{
			network.SetScalingParams(trainData, -1.0f, 1.0f, -1.0f, 1.0f);
		}
	}
}