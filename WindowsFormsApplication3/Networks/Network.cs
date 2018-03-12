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

        public void SetupActivation()
        {
            var activationFunc = ActivationFunction.SIGMOID_SYMMETRIC;
            Program.Form.AddConfiguration($"\r\n InputActFunc: {activationFunc}");
            this.network.SetActivationFunctionLayer(activationFunc, 1);
            activationFunc = ActivationFunctionGenerator.GetRandomFunction();
            Program.Form.AddConfiguration($" LayerActFunc: {activationFunc}");
            this.network.SetActivationFunctionLayer(activationFunc, 2);
        }

        public void InitWeights(TrainingData trainData)
        {
            this.network.InitWeights(trainData);
        }

        public Network(NetworkType layer, uint numInput, uint numHidden, uint numOutput)
        {
            network = new NeuralNet(layer, 3, numInput, numHidden, numOutput);
            debug($"network {this.network} input: {numInput} numHidden: {numHidden} output: {numOutput}");
        }

        public double Train(TrainingData trainData)
        {
            this.network.SetScalingParams(trainData, -1.0f, 1.0f, -1.0f, 1.0f);
            return this.network.TrainEpochIrpropmParallel(trainData, 4);
        }

        public double[] Run(double[] input)
        {
            double[] outputData = this.network.Run(input);
            return outputData;
        }

        public void Save(string name)
        {
            debug($"saving network 0x{this.network.GetHashCode()} as {name}");
            this.network.Save(name);
        }

        internal void SetupScaling(TrainingData trainData)
        {
            this.network.SetScalingParams(trainData, -1.0f, 1.0f, -1.0f, 1.0f);
        }
    }
}