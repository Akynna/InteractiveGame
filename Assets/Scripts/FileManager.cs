using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileManager : MonoBehaviour
{


    private static string f0Key = "F0";
    private static string loudnessKey = "loudness";

    static Type t = typeof(float);

    public static List<List<float>> ReadCSV(string filename, char separator, bool skipHeader = false, bool skipLabel = false)
    {
        List<List<float>> data = new List<List<float>>();

        using (var reader = new StreamReader(filename))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<float> lineData = new List<float>();
                var skipLbl = skipLabel;
                if (!skipHeader)
                {
                    foreach (string cell in line.Split(separator))
                    {
                        if (!skipLbl)
                        {
                            float cellAdd = (float)double.Parse(cell);
                            lineData.Add(cellAdd);
                        }
                        skipLbl = false;
                    }
                }
                skipHeader = false;
                data.Add(lineData);
            }
        }
        return data;
    }

    public static List<List<string>> ReadCSV(string filename, char separator, bool skipHeader = false)
    {
        List<List<string>> data = new List<List<string>>();

        using (var reader = new StreamReader(filename))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<string> lineData = new List<string>();
                if (!skipHeader)
                {
                    foreach (string cell in line.Split(separator))
                    {
                        lineData.Add(cell);
                    }
                }
                skipHeader = false;
                data.Add(lineData);
            }
        }
        return data;
    }

    static List<int> populateColumns = new List<int>();

    // Function that reads the given csv and prepares the data to be used
    public static List<List<float>> ReadOpensmileData(string filename)
    {

        List<float> labels = new List<float>();
        List<List<float>> data = new List<List<float>>();
        List<float> dataRow = new List<float>();

        int headerLength = 0;

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
                                float label = MachineLearning.ParseLabel(cell);
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
                    headerLength = line.Split(';').Length;
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

    public static void WriteCSV(List<float> data, string filename, string separator)
    {
        string[] toWrite = data.Select(x => x.ToString()).ToArray();

        using (var file = File.CreateText(filename))
        {
            file.WriteLine(string.Join(separator, toWrite));
        }
    }

    public static void AddToCSV(string fileToWrite, string label, List<List<float>> data, string separator)
    {
        foreach(List<float> line in data)
        {
            string[] toWrite = line.Select(x => x.ToString()).ToArray();

            File.AppendAllText(fileToWrite, label + separator + string.Join(separator, toWrite) + Environment.NewLine);
        }
    }

    public static void AddToCSV(string fileToWrite, string fileToRead, string label, string separator)
    {
        List<List<float>> data = ReadCSV(fileToRead, ';', true, true);
        AddToCSV(fileToWrite, label, data, separator);
    }

    public static void DeleteFile(string path, string filename)
    {
        string os = SystemInfo.operatingSystem;
        if (os.Contains("Windows"))
        {
            try
            {
                // Start process
                Process myProcess = new Process();
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // Hide the window
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "cmd.exe"; // Open a Command prompt at windows

                string arg = "";

                string projectPath = Path.Combine(path, filename);

                string pattern = @"/";

                string replacement = "\\";

                string finalPath = Regex.Replace(projectPath, pattern, replacement,
                                                  RegexOptions.IgnoreCase);

                // Call the whole argument
                arg = "del /f " + finalPath;

                // Call the necessary functions to execute the command
                myProcess.StartInfo.Arguments = "/c" + arg;
                myProcess.EnableRaisingEvents = true;
                myProcess.Start();
                myProcess.WaitForExit();
                int ExitCode = myProcess.ExitCode;
                //print(ExitCode);
            }
            catch (Exception e)
            {
                print(e);
            }
        }
        else if(os.Contains("Mac"))
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "rm";
            proc.WorkingDirectory = path;
            proc.Arguments = filename;
            proc.WindowStyle = ProcessWindowStyle.Minimized;
            proc.CreateNoWindow = true;
            Process.Start(proc);

        } else if (os.Contains("Linux"))
        {

        }
        
    }
}
