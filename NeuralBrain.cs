using System;
using System.Collections.Generic;
using NeuralNetwork.NetworkModels;
using UnityEngine;
using System.IO;
using System.Threading;

public class NeuralBrain : MonoBehaviour
{
	public int numSamples;
	public int numInputs, numOutputs;
	public int[] hiddenLayer;
	public TrainingType trainType;
	public int epochs;
	public double minimumError;
	public double learningRate, momentum;
	public int testValueIndex;

	private NeuralNetwork.NetworkModels.Network network;
	private int[] hiddenLayers;
	private List<DataSet> dataSets;
	private DataSet tempDataSet;

	private StreamReader reader;
	private String trainingSetPath = "C:\\Users\\Naman Gupta\\Downloads\\Compressed\\minor project\\testProject0_5.4.2\\Assets\\CAS_trainingData\\TrainingData.txt";
	private String testSetPath = "C:\\Users\\Naman Gupta\\Downloads\\Compressed\\minor project\\testProject0_5.4.2\\Assets\\CAS_trainingData\\TestData.txt";
	private String readPath;
	private double[] values, targets;
	private int lineCount;

	[HideInInspector]public bool trained;

	private Thread trainingThread;

	public void PredictRandomFromTrainingSet ()
	{
		if (trained) {
			double[] testInput = dataSets [testValueIndex].Values;

			double[] results = network.Compute (testInput);

			double maxPrediction = -1;
			int loc = -1;

			for (int i = 0; i < results.Length; i++) {
				if (maxPrediction < results [i] && (i - (numOutputs / 2)) != 0) {
					maxPrediction = results [i];
					loc = i;
				}
			}
			Debug.Log ((loc - (numOutputs / 2)));
		}
	}

	public void PredictFromTrainingSet ()
	{
		if (trained) {
			Initiallize (0);

			for (int i = 0; i < numSamples; i++) {
				double[] results = network.Compute (dataSets [i].Values);

				Debug.Log (results [0] * 60 - 30);
			}

			/*double trainingSetAccuracy = 0;
			float errorSum = 0;

			for (int i = 0; i < numSamples; i++) {
				double[] testInput = dataSets [i].Values;
				double[] results = network.Compute (testInput);

				double maxPrediction = -1;
				int loc = -1;

				for (int x = 0; x < results.Length; x++) {
					if (maxPrediction < results [x] && (x - (numOutputs / 2)) != 0) {
						maxPrediction = results [x];
						loc = x;
					}
				}

				double prediction = loc - (numOutputs / 2);
				double targetPrediction = Array.FindIndex (dataSets [i].Targets, t => t == 1) - (numOutputs / 2);

				errorSum += Mathf.Abs ((float)(targetPrediction - prediction)) / Mathf.Abs ((float)targetPrediction);

				Debug.Log ((i + 1) + ":" + prediction);
			}
			Debug.Log (errorSum / numSamples);*/
		}
	}

	public void PredictFromTestSet ()
	{
		if (trained) {
			Initiallize (1);

			for (int i = 0; i < numSamples; i++) {
				double[] results = network.Compute (dataSets [i].Values);

				Debug.Log (results [0] * 60 - 30);
			}

			/*double trainingSetAccuracy = 0;
			float errorSum = 0;

			for (int i = 0; i < numSamples; i++) {
				double[] testInput = dataSets [i].Values;
				double[] results = network.Compute (testInput);

				double maxPrediction = -1;
				int loc = -1;

				for (int x = 0; x < results.Length; x++) {
					if (maxPrediction < results [x] && (x - (numOutputs / 2)) != 0) {
						maxPrediction = results [x];
						loc = x;
					}
				}

				double prediction = loc - (numOutputs / 2);
				double targetPrediction = Array.FindIndex (dataSets [i].Targets, t => t == 1) - (numOutputs / 2);

				errorSum += Mathf.Abs ((float)(targetPrediction - prediction)) / Mathf.Abs ((float)targetPrediction);

				Debug.Log ((i + 1) + ":" + prediction);
			
			}
			Debug.Log (errorSum / numSamples);*/
		}
	}

	public double Predict (double[] values)
	{
		if (trained) {
			double[] results = network.Compute (values);

			/*double maxPrediction = -1;
			int loc = -1;

			for (int x = 0; x < results.Length; x++) {
				if (maxPrediction < results [x] && (x - (numOutputs / 2)) != 0) {
					maxPrediction = results [x];
					loc = x;
				}
			}
			Debug.Log (loc - (numOutputs / 2));

			return (loc - (numOutputs / 2));*/

			return (results [0] * 60 - 30);
		} else
			return 0;
	}

	void TrainNetwork ()
	{
		while (true) {
			if (network == null) {
				network = new NeuralNetwork.NetworkModels.Network (numInputs, hiddenLayer, numOutputs);
				network.LearnRate = learningRate;
				network.Momentum = momentum;
			}

			Debug.Log ("Wait, network is training, this can take a while...");

			if (trainType == TrainingType.Epoch)
				network.Train (dataSets, epochs);
			else
				network.Train (dataSets, minimumError);

			Debug.Log ("Training complete!" + " Time elapsed = ");
			trained = true;
			break;
		}
	}

	public bool Initiallize (int dataSetType)
	{
		if (dataSetType == 0)
			readPath = trainingSetPath;
		else
			readPath = testSetPath;

		reader = new StreamReader (readPath);

		int index = 0;
		lineCount = 0;

		dataSets = new List<DataSet> ();

		while (lineCount < numSamples) {

			String line = reader.ReadLine ();
			String[] readValues = line.Split (',');

			targets = new double[numOutputs];

			/*index = int.Parse (readValues [0]) + (numOutputs / 2);
			index = index > numOutputs - 1 ? numOutputs - 1 : index;

			
			for (int i = 0; i < numOutputs; i++) {
				targets [i] = 0;
			}
			targets [index] = 1;*/

			targets [0] = (double.Parse (readValues [0]) + 30) / 60;

			values = new double[numInputs];
			for (int i = 0; i < numInputs; i++) {
				values [i] = double.Parse (readValues [i + 1]) / 16;
			}
				
			dataSets.Add (new DataSet (values, targets));

			lineCount++;
		}


		trainingThread = new Thread (TrainNetwork);

		if (dataSetType == 0)
			Debug.Log ("Training set has been loaded.");
		else
			Debug.Log ("Test set has been loaded.");

		reader.Close ();

		return true;
	}

	public void StartTrainingThread ()
	{
		trainingThread.Start ();
	}

	public void Reset ()
	{
		network = null;
		trained = false;

		if (dataSets != null)
			dataSets.Clear ();

		lineCount = 0;

		if (reader != null)
			reader.Close ();

		if (trainingThread != null && trainingThread.IsAlive) {
			trainingThread.Abort ();
			trainingThread = null;
		}
	}
}