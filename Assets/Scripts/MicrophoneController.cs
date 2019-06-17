using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

/* This scripts takes care of the control of the microphone. It saves the audio as a .wav file
and then analyzes it with opensmile to retrieve all the interesting features in a .csv file*/

public class MicrophoneController : MonoBehaviour
{

    // Need to use an AudioSource to reduce the clip time
    AudioSource myAudioClip;

    // Buttons for the recording
    GameObject startButton;
    GameObject stopButton;
    GameObject playButton;
    GameObject ans;

    public GameObject startTemplate;
    public GameObject stopTemplate;
    public GameObject playTemplate;
    public GameObject choice;
    public GameObject canvas;
    public GameObject dialoguePanel;
    public Animator dialogueAnimator;
    public CharacterManager characterManager;
    public AudioManager AudioManager;

    // Opensmile files configuration
    public static string output = "outputData";
    private string config = "IS10_paraling.conf";

    // Naming and formating of the record
    public static string recordName = "record";
    public static string textName = "text";
    public static string characterName = "character";
    public static string id = ""; // Hold the id of the user TODO : UPDATE WHEN CREATED
    public static string dateFormat = "yyyyMMddHHmmss";
    public static string date;

    public static bool recording = false;
    public static string answer = "";
    public static string audioName = "";

    // On Awake, get ready the W vector for Machine Learning
    void Awake()
    {
        MachineLearning.ReadyW();
        id = PlayerPrefs.GetString("ID");
    }

    void Start()
    {
        // Get the AudioSource for the microphone
        myAudioClip = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (recording)
        {
            RecordChoice();
            recording = false;
        }
    }

    public void RecordChoice()
    {
        AudioManager.StopEffect();
        dialoguePanel.SetActive(false);
        ans = Instantiate(choice, canvas.transform);
        //ans.transform.SetParent(canvas.transform);
        //ans.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);
        //ans.transform.position = dialoguePanel.transform.position;
        ans.GetComponent<Animator>().SetBool("isOpen", true);

        Text answerText = ans.GetComponentInChildren<Text>();
        answerText.text = answer;

        AudioManager.UpdateEffectSound(audioName);

        CreatePlayButton();
        CreateStartButton();
    }

    void CreatePlayButton() {
        playButton = Instantiate(playTemplate);
        playButton.GetComponentInChildren<Button>().onClick.AddListener(() => PlayLastQuestion());
        playButton.transform.SetParent(ans.transform);
        playButton.transform.position = new Vector3(ans.transform.position.x - 50, ans.transform.position.y - 15);
    }



    public void PlayLastQuestion() {
        AudioManager.PlayEffect();
    }

    void CreateStartButton()
    {
        startButton = Instantiate(startTemplate);
        startButton.GetComponentInChildren<Button>().onClick.AddListener(() => StartRecord());
        startButton.transform.SetParent(ans.transform);
        startButton.transform.position = new Vector3(ans.transform.position.x + 40, ans.transform.position.y - 55);
    }

    void CreateStopButton()
    {
        stopButton = Instantiate(stopTemplate);
        stopButton.GetComponentInChildren<Button>().onClick.AddListener(() => StopRecord());
        stopButton.transform.SetParent(ans.transform);
        stopButton.transform.position = new Vector3(ans.transform.position.x + 40, ans.transform.position.y - 55);
    }

    public void StartRecord()
    {
        Destroy(startButton);
        CreateStopButton();

        // Default microphone
        myAudioClip.clip = Microphone.Start(null, false, 50, 44100);
    }

    public void StopRecord()
    {
        dialoguePanel.SetActive(true);
        dialogueAnimator.SetBool("isOpen", true);

        Destroy(stopButton);
        Destroy(ans);

        // Cuts the recording when the stop button is pressed
        EndRecording(myAudioClip, null);

        date = DateTime.Now.ToString(dateFormat);
        // Saves the audio clip as a .wav
        SavWav.Save("Data/TempData/" + recordName + id + date, myAudioClip.clip);
        SavWav.Save("Data/Records/" + recordName + id + date, myAudioClip.clip);
        FileManager.WriteTextFile(FileManager.tempAnswersDataFolder, textName + id + date + ".txt", answer);
        FileManager.WriteTextFile(FileManager.tempAnswersDataFolder, characterName + id + date + ".txt", characterManager.currentCharacter.name);
        // Analyzes the clip with opensmile
        CallOpenSmile(output + id + date + ".csv", config);

        /*bool timeout = false;
        DateTime startCheck = DateTime.Now;
        while (!File.Exists(Path.Combine(FileManager.tempDataFolder, output + id + date + ".csv")) && !timeout)
        {
            DateTime check = DateTime.Now;
            if((check - startCheck).TotalSeconds >= 2)
            {
                timeout = true;
            }
        }*/

        if (SystemInfo.operatingSystem.Contains("Mac"))
        {
            Thread.Sleep(2000);
        }

        // Get all the information from CSVs
        var prediction = MachineLearning.PredictWithData(output + id + date + ".csv");
        characterManager.currentCharacter.empathyScore += (int)prediction * 2 - 1; // Gives 1 or -1 to the empathy score depending on the empathy level

        recording = false;
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

    void CallOpenSmile(string filename, string configMode)
    {
        string os = SystemInfo.operatingSystem;
        if (os.Contains("Windows"))
        {
            try
            {
                // Start process
                Process myProcess = new Process();
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "cmd.exe";

                string arg = "";

                string projectPath = Application.dataPath;
                int pos = projectPath.IndexOf("Assets");
                string prPath = projectPath.Remove(pos);

                string pattern = @"/";

                string replacement = "\\";

                projectPath = Regex.Replace(prPath, pattern, replacement,
                                                  RegexOptions.IgnoreCase);


                var audioPath = projectPath + "Assets\\Data\\TempData\\" + recordName + id + date + ".wav";
                var config = projectPath + "opensmile\\opensmile-2.3.0\\config\\" + configMode;
                var osmilexe = projectPath + "opensmile\\opensmile-2.3.0\\bin\\Win32\\SMILExtract_Release.exe";

                // Call the whole argument
                arg = osmilexe + " -C " + config + " -I " + audioPath + " -csvoutput " + "Assets\\Data\\TempData\\" + filename;


                // Call the necessary functions to execute the command
                myProcess.StartInfo.Arguments = "/c" + arg;
                myProcess.EnableRaisingEvents = true;
                myProcess.Start();
                myProcess.WaitForExit();
                int ExitCode = myProcess.ExitCode;

            }
            catch (Exception e)
            {
                print(e);
            }
        }
        else if (os.Contains("Mac"))
        {

            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "bash";

            string projectPath = Application.dataPath;
            int pos = projectPath.IndexOf("Assets");
            string prPath = projectPath.Remove(pos);

            proc.WorkingDirectory = prPath;


            var audioPath = "Assets/Data/TempData/" + recordName + id + date + ".wav";
            var config = "opensmile/opensmile-2.3.0/config/" + configMode;
            var osmilexe = "opensmile/opensmile-2.3.0/SMILExtract";

            // Call the whole argument
            string arg = osmilexe + " -C " + config + " -I " + audioPath + " -csvoutput " + "Assets/Data/TempData/" + filename;

            proc.Arguments = arg;
            proc.WindowStyle = ProcessWindowStyle.Minimized;
            proc.CreateNoWindow = true;
            Process.Start(proc);
        }
        else if (os.Contains("Linux"))
        {

        }
    }

}