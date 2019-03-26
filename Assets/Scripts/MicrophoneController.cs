using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class MicrophoneController : MonoBehaviour
{

    AudioSource myAudioClip;

    void Start()
    {
        myAudioClip = this.GetComponent<AudioSource>();
    }

    public void startRecord()
    {
        myAudioClip.clip = Microphone.Start(null, false, 50, 44100);
    }

    public void stopRecord()
    {
        EndRecording(myAudioClip, null);
        SavWav.Save("record", myAudioClip.clip);
        callOpenSmile();
    }

    void EndRecording(AudioSource audS, string deviceName)
    {
        //Capture the current clip data
        AudioClip recordedClip = audS.clip;
        var position = Microphone.GetPosition(deviceName);
        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);

        //Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];

        //Microphone.End (null);
        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        //One does not simply shorten an AudioClip,
        //    so we make a new one with the appropriate length
        var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
        newClip.SetData(newData, 0);        //Give it the data from the old clip

        //Replace the old clip
        AudioClip.Destroy(recordedClip);
        audS.clip = newClip;
    }

    void callOpenSmile()
    {
        try
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";

            var projectPath = "C:\\Users\\Alvaro\\Desktop\\Cours\\Master\\Semester4\\";
            //var backPath = "..\\..\\";
            var audioPath = projectPath + "SemesterProject\\InteractiveGame\\Assets\\record.wav";
            var config = projectPath + "opensmile\\opensmile-2.3.0\\config\\ComParE_2016.conf";
            var outputFile = "Output.csv";
            var osmilexe = projectPath + "opensmile\\opensmile-2.3.0\\bin\\Win32\\SMILExtract_Release.exe";
            string arg = osmilexe + " -C " + config + " -I " + audioPath + " -csvoutput " + outputFile;

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

    void inspectCSV()
    {

    }

}