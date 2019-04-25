using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System;
using System.IO;
using System.Text.RegularExpressions;

/* This scripts takes care of the control of the microphone. It saves the audio as a .wav file
and then analyzes it with opensmile to retrieve all the interesting features in a .csv file*/

public class MicrophoneController : MonoBehaviour
{

    // Need to use an AudioSource to reduce the clip time
    AudioSource myAudioClip;

    GameObject startButton;
    GameObject stopButton;

    // Values for the Fundamental Frequency analysis
    //private string outputFundamental = "outputF0.csv";
    //private string outputFundamental = "animated3.csv";
    //private string configFundamental = "ComParE_2016.conf";

    // Values for the Loudness analysis
    private string output = "outputData.csv";
    private string config = "IS10_paraling.conf";

    void Awake()
    {
        MachineLearning.readyW();   
    }

    void Start()
    {
        myAudioClip = this.GetComponent<AudioSource>();

        startButton = GameObject.Find("StartRecordButton");
        stopButton = GameObject.Find("StopRecordButton");

        stopButton.SetActive(false);
        startButton.SetActive(true);
    }

    public void startRecord()
    {
        stopButton.SetActive(true);
        startButton.SetActive(false);

        // Default microphone
        myAudioClip.clip = Microphone.Start(null, false, 50, 44100);
    }

    public void stopRecord()
    {

        stopButton.SetActive(false);
        startButton.SetActive(true);

        // Cuts the recording when the stop button is pressed
        EndRecording(myAudioClip, null);

        // Saves the audio clip as a .wav
        SavWav.Save("record", myAudioClip.clip);

        // Analyzes the clip with opensmile
        callOpenSmile();

        // Get all the information from CSVs
        var prediction = MachineLearning.predictWithData(output);
        UnityEngine.Debug.Log(prediction);

        // Destroy the CSVs
        destroyCSV();

    }

    void EndRecording(AudioSource audS, string deviceName)
    {
        // Capture the current clip data
        AudioClip recordedClip = audS.clip;
        var position = Microphone.GetPosition(deviceName);
        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);

        // Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];

        // Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        // One does not simply shorten an AudioClip,
        // so we make a new one with the appropriate length
        var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
        // Give it the data from the old clip
        newClip.SetData(newData, 0);

        // Replace the old clip
        AudioClip.Destroy(recordedClip);
        audS.clip = newClip;
    }

    void callOpenSmile()
    {
        // opensmile mode = 0
        commandPromptCalls(0, output, config);
        //commandPromptCalls(0, outputLoudness, configLoudness);
    }

    void destroyCSV()
    {
        // delete mode = 1
        commandPromptCalls(1, output);
        //commandPromptCalls(1, outputLoudness);
    }

    void commandPromptCalls(int mode, string filename, string configMode = "")
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

            string projectPath = Application.dataPath;
            int pos = projectPath.IndexOf("Assets");
            var prPath = projectPath.Remove(pos);

            string pattern = @"/";

            string replacement = "\\";

            projectPath = Regex.Replace(prPath, pattern, replacement,
                                              RegexOptions.IgnoreCase);

            if (mode == 0)
            {
                var audioPath = projectPath + "Assets\\record.wav";
                var config = projectPath + "opensmile\\opensmile-2.3.0\\config\\" + configMode;
                var osmilexe = projectPath + "opensmile\\opensmile-2.3.0\\bin\\Win32\\SMILExtract_Release.exe";

                // Call the whole argument
                arg = osmilexe + " -C " + config + " -I " + audioPath + " -csvoutput " + filename;
            }
            else if(mode == 1)
            {
                // Creation of the paths
                // Need to update those paths to relative ones
                var path = projectPath;

                // Call the whole argument
                arg = "del /f " + path + filename;
            }

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

}