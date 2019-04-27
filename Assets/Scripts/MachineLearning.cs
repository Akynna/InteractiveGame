using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MachineLearning
{

    private static string labelAnimated = "animated";

    private static string dataFile = "data.csv";

    private static string f0Key = "F0";
    private static string loudnessKey = "loudness";

    // W can be precomputed with, e.g, a python script, before the launch of the game
    private static string fileW = "w.csv";

    // Keeps the state of the w
    public static List<float> globalW;


    // Starts the Machine Learning processing with the current data set
    public static void readyW(/*string data*/)
    {
        if (!File.Exists(fileW))
        {
            List<List<float>> dataRead = readCSV(dataFile);

            List<float> y = dataRead[dataRead.Count - 1];

            dataRead.RemoveAt(dataRead.Count - 1);

            globalW = sgdSVM(y, dataRead);
            writeCSV(globalW);
        }
        else
        {
            globalW = readW();
        }
    }

    // Uses the given data and the w to make a prediction
    public static float predictWithData(string filename)
    {
        List<List<float>> dataToPredict = readCSV(filename);
        dataToPredict.RemoveAt(dataToPredict.Count-1);

        return prediction(dataToPredict, globalW)[0];
    }

    static List<int> populateColumns = new List<int>();

    // Function that reads the given csv and prepares the data to be used
    static List<List<float>> readCSV(string filename)
    {

        List<float> labels = new List<float>();
        List<List<float>> data = new List<List<float>>();
        List<float> dataRow = new List<float>();

        using (var reader = new StreamReader(filename))
        {
            // Use rows to know if you are reading the header of the data  itself
            var rows = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var columns = 0;
                
                if (rows != 0) // Ignore the header
                {
                    foreach (string cell in line.Split(';'))
                    {
                        // Take only the columns that are among the chosen ones
                        if (populateColumns.Contains(columns))
                        {
                            float cellAdd = (float)double.Parse(cell);
                            dataRow.Add(cellAdd);
                        }
                        else
                        {
                            if (columns == 0)
                            {
                                float label = parseLabel(cell);
                                labels.Add(label);
                            }
                        }
                        columns++;
                    }
                    data.Add(new List<float>(dataRow));
                    dataRow.Clear();
                }
                else
                {
                    int idx = 0;
                    foreach (string cell in line.Split(';'))
                    {
                        if (cell.Contains(f0Key) || cell.Contains(loudnessKey))
                        {
                            populateColumns.Add(idx);
                        }
                        idx++;
                    }
                }
                rows++;
            }
        }
        // Add the labels row as the last one, it will be separated later from the data itself
        data.Add(labels);

        return data;
    }

    static void writeCSV(List<float> data)
    {
        string[] toWrite = data.Select(x => x.ToString()).ToArray();

        using (var file = File.CreateText(fileW))
        {
            file.WriteLine(string.Join(",", toWrite));
        }
    }

    static List<float> readW()
    {
        List<float> w = new List<float>();

        using (var reader = new StreamReader(fileW))
        {
            var line = reader.ReadLine();

            foreach (string cell in line.Split(','))
            {
                // Take only the columns that are among the chosen ones
                float cellAdd = (float)double.Parse(cell);
                w.Add(cellAdd);
            }
        }
        return w;
    }

    // Gives a float to each possible label, for sake of simplicity we only consider 'animated' and 'not animated' for now
    static float parseLabel(string cell)
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

    static List<float> hingeLoss(List<float> y, List<List<float>> X, List<float> w)
    {
        List<float> XcrossW = crossProduct(X, w);

        List<float> YmultCross = y.Select((dValue, index) => 1 - dValue * XcrossW[index]).ToList();

        return clipList(YmultCross);
    }

    // Performs a cross product between a matrix/vector and a vector
    static List<float> crossProduct(List<List<float>> X, List<float> w)
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
    static List<float> clipList(List<float> list)
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

    static float calculatePrimalOjective(List<float> y, List<List<float>> X, List<float> w, float lambda)
    {
        List<float> v = hingeLoss(y, X, w);
        return v.Sum() + lambda / 2 * w.Sum(x => x * x);
    }

    static float accuracy(List<float> y1, List<float> y2)
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

    static List<float> prediction(List<List<float>> X, List<float> w)
    {
        List<float> XcrossW = crossProduct(X, w);

        List<float> boolPred = new List<float>();
        for(int i = 0; i < XcrossW.Count; i++)
        {
            if(XcrossW[i] > 0)
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

    static float calculateAccuracy(List<float> y, List<List<float>> X, List<float> w)
    {
        List<float> predictedY = prediction(X, w);
        return accuracy(predictedY, y);
    }

    static List<float> calculateStochasticGradient(List<float> y, List<List<float>> X, List<float> w, float lambda, int n, int numExamples)
    {
        List<float> tempGrad;
        List<float> xN = X[n];
        float yN = y[n];
        if(isSupport(yN, xN, w))
        {
            tempGrad = xN.Select(x => - yN * x).ToList();
        }
        else
        {
            tempGrad = Enumerable.Repeat((float) 0, xN.Count).ToList();
        }
        return tempGrad.Select((dValue, index) => (numExamples * dValue) + (lambda * w[index])).ToList();
    }

    static bool isSupport(float yN, List<float> xN, List<float> w)
    {
        float crossXNW = xN.Select((dValue, index) => dValue * w[index]).ToList().Sum();
        return (yN * crossXNW) < 1;
    }

    // Main function that performs the machine learning steps
    static List<float> sgdSVM(List<float> y, List<List<float>> X)
    {
        int maxIter = 100000;
        int gamma = 1;
        float lambda = 0.01f;

        int numExamples = X.Count;
        int num_features = X[0].Count;
        List<float> w = Enumerable.Repeat((float)0, num_features).ToList();

        for (int it = 0; it < maxIter; it++)
        {
            int n = randomNumber(0, numExamples - 1);

            List<float> grad = calculateStochasticGradient(y, X, w, lambda, n, numExamples);
            w = w.Select((dValue, index) => dValue - ((gamma / (it + 1)) * grad[index])).ToList();
        }

        return w;
    }

    // Generate a random number between two numbers
    public static int randomNumber(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
}
