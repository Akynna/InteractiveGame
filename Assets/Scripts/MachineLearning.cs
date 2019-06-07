using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MachineLearning
{
    // Labels
    public static string labelAnimated = "animated";
    public static string labelNotAnimated = "not animated";

    // Files
    public static string dataFile = "data.csv";
    private static string fileW = "w.csv";

    public static List<float> globalW;

    // Starts the Machine Learning processing with the current data set
    public static void ReadyW(/*string data*/)
    {
        if (!File.Exists(Path.Combine(FileManager.dataFolder, fileW)))
        {
            List<List<float>> dataRead = FileManager.ReadOpensmileData(FileManager.dataFolder, dataFile);

            List<float> y = dataRead[dataRead.Count - 1];

            dataRead.RemoveAt(dataRead.Count - 1);

            globalW = SgdSVM(y, dataRead);

            FileManager.WriteCSV(globalW, FileManager.dataFolder, fileW, ",");
        }
        else
        {
            globalW = FileManager.ReadCSV(FileManager.dataFolder, fileW, ',', false, false)[0];
        }
    }

    // Uses the given data and the w to make a prediction
    public static float PredictWithData(string filename)
    {
        List<List<float>> dataToPredict = FileManager.ReadOpensmileData(FileManager.tempDataFolder, filename);
        dataToPredict.RemoveAt(dataToPredict.Count-1);

        return Prediction(dataToPredict, globalW)[0];
    }

    // Gives a float to each possible label, for sake of simplicity we only consider 'animated' and 'not animated' for now
    public static float ParseLabel(string cell)
    {
        if (cell.Equals(labelAnimated))
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    static List<float> HingeLoss(List<float> y, List<List<float>> X, List<float> w)
    {
        List<float> XxW = ScalarProduct(X, w);
        List<float> Ymult = y.Select((dValue, index) => 1 - dValue * XxW[index]).ToList();

        return ClipList(Ymult);
    }

    static List<float> ScalarProduct(List<List<float>> X, List<float> w)
    {
        //List<float> XcrossW = new List<float>();

        /*for(int i = 0; i < X.Count; i++)
        {
            List<float> cross = X[i].Select((dValue, index) => dValue * w[index]).ToList();
            XcrossW.Add(cross.Sum());
        }*/

        return X.Select(list => list.Select((dValue, index) => dValue * w[index]).ToList().Sum()).ToList();
    }
    
    // Clips to positive values all the content from the given list
    static List<float> ClipList(List<float> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i] < 0)
            {
                list[i] = 0;
            }
        }
        return list;
    }

    static float CalculatePrimalOjective(List<float> y, List<List<float>> X, List<float> w, float lambda)
    {
        List<float> v = HingeLoss(y, X, w);
        return v.Sum() + lambda / 2 * w.Sum(x => x * x);
    }

    static float Accuracy(List<float> y1, List<float> y2)
    {
        if(y1.Count != y2.Count) // Check if both lists have the same length, if not exit
        {
            Debug.Log("Lists in accuracy function are not of the same size!");
            return 0;
        }
        float mean = 0;
        for(int i = 0; i < y1.Count; i++)
        {
            if(y1[i] == y2[i])
            {
                mean++;
            }
        }
        return mean / y1.Count;
    }

    static List<float> Prediction(List<List<float>> X, List<float> w)
    {
        List<float> XxW = ScalarProduct(X, w);
        List<float> boolPred = new List<float>();

        for(int i = 0; i < XxW.Count; i++)
        {
            if(XxW[i] > 0)
            {
                boolPred.Add(1);
            }
            else
            {
                boolPred.Add(0);
            }
        }

        return boolPred.Select(x => x * 2 - 1).ToList();
    }

    static float CalculateAccuracy(List<float> y, List<List<float>> X, List<float> w)
    {
        List<float> predictedY = Prediction(X, w);
        return Accuracy(predictedY, y);
    }

    static List<float> CalculateStochasticGradient(List<float> y, List<List<float>> X, List<float> w, float lambda, int n, int numExamples)
    {
        List<float> tempGrad;
        List<float> xN = X[n];
        float yN = y[n];
        if(IsSupport(yN, xN, w))
        {
            tempGrad = xN.Select(x => - yN * x).ToList();
        }
        else
        {
            tempGrad = Enumerable.Repeat((float) 0, xN.Count).ToList();
        }
        return tempGrad.Select((dValue, index) => (numExamples * dValue) + (lambda * w[index])).ToList();
    }

    static bool IsSupport(float yN, List<float> xN, List<float> w)
    {
        float XNW = xN.Select((dValue, index) => dValue * w[index]).ToList().Sum();
        return (yN * XNW) < 1;
    }

    // Main machine learning function
    static List<float> SgdSVM(List<float> y, List<List<float>> X)
    {
        int maxIter = 100000;
        int gamma = 1;
        float lambda = 0.01f;

        int numExamples = X.Count;
        int num_features = X[0].Count;
        List<float> w = Enumerable.Repeat((float)0, num_features).ToList();

        for (int it = 0; it < maxIter; it++)
        {
            int n = RandomNumber(0, numExamples - 1);

            List<float> grad = CalculateStochasticGradient(y, X, w, lambda, n, numExamples);
            w = w.Select((dValue, index) => dValue - ((gamma / (it + 1)) * grad[index])).ToList();
        }

        return w;
    }

    public static int RandomNumber(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
}
