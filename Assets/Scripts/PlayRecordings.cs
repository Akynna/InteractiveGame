using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayRecordings : MonoBehaviour
{

    GameObject playButton;

    public GameObject audioPlayerTemplate;
    public GameObject canvas;

    public AudioClip audioClip;

    GameObject background;

    string audioPlayersTag = "AudioPlayers";
    string audioButtonsTag = "AudioButton";
    string playBtn = "PlayButton";
    string validateButton = "ValidateButton";
    string choiceText = "ChoiceName";

    // TODO Make it to be the start date of the chapter you are in
    DateTime startDate;

    // Get all the data for the recordings
    string recordName = MicrophoneController.recordName;
    string outputName = MicrophoneController.output;
    string textName = MicrophoneController.textName;
    string id = MicrophoneController.id;
    string dateFormat = MicrophoneController.dateFormat;
    public static string date = MicrophoneController.date;

    public static bool evaluating = false;

    private List<Vector3> targets = new List<Vector3>();
    private bool move = false;

    // Start is called before the first frame update
    void Start()
    {
        // TODO modify it to create multiple objects depending on the number of records DONE
        // TODO delete this part when the audio check is setup to work on end of scene
        playButton = GameObject.Find("PlayRecords");

        startDate = DateTime.Now;
    }

    void Update()
    {
        if (evaluating)
        {
            PlayRecords();
            evaluating = false;
        }

        if (GameObject.FindGameObjectsWithTag(audioPlayersTag).Length == 0)
        {
            move = false;
            EnableButtons();
            RevertBackground();
        }

        if (move)
        {
            MoveObjects();
        }
    }

    void OnApplicationQuit()
    {
        EnableButtons();
    }

    /* OnClick function to launch the play of the records */
    public void PlayRecords()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(audioPlayersTag))
        {
            Destroy(obj);
        }
        CreateTableWithAudios();
        DisableButtons();
    }

    private void CreateTableWithAudios()
    {
        List<string> recordFiles = GetRecords(startDate);
        Vector3 position = new Vector3(Screen.width/2f, Screen.height);

        foreach(string file in recordFiles)
        {
            CreateBackground();

            GameObject obj = Instantiate(audioPlayerTemplate);

            // Active fields
            obj.transform.Find(playBtn).GetComponent<Button>().onClick.AddListener(() => PlayChosenAudio(file, obj.GetComponentInChildren<AudioSource>()));
            obj.transform.Find(validateButton).GetComponent<Button>().onClick.AddListener(() => ValidateEvaluation(obj, file));

            // Position of elements in screen space
            obj.transform.SetParent(background.transform);
            position = new Vector3(position.x, position.y - obj.GetComponent<RectTransform>().rect.height);
            obj.transform.position = position;

            string answer = GetAnswerText(file);
            obj.transform.Find(choiceText).GetComponent<Text>().text = answer;

            float ratio = Convert.ToSingle(Screen.width / obj.GetComponent<RectTransform>().sizeDelta.x);
            obj.transform.localScale = new Vector3(ratio, ratio, 1);
        }
    }

    private string GetAnswerText(string filename)
    {
        string dateT = ParseDate(filename).ToString(dateFormat);
        string text = FileManager.ReadTextFile(FileManager.tempAnswersDataFolder, textName + dateT + ".txt");
        
        if(text.Length > 30)
        {
            text = text.Substring(0, 10) + "...";
        }

        return text;
    }

    private void CreateBackground()
    {
        background = new GameObject();
        background.AddComponent<Image>();
        background.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
        background.transform.SetParent(canvas.transform);
        background.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
        background.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width, Screen.height);
        
    }

    private void RevertBackground()
    {
        Destroy(background);
    }

    /* Gets all the records that have been recorded since the start of the chapter scene */
    private List<string> GetRecords(DateTime d1)
    {
        DirectoryInfo d = new DirectoryInfo(FileManager.tempDataFolder); // Directory at Assets
        FileInfo[] Files = d.GetFiles("*.wav"); // Getting all .wav files

        List<string> files = new List<string>();

        foreach (FileInfo file in Files)
        {
            string fileName = file.Name;
            DateTime date = ParseDate(fileName);
            // Take all records recorded after start of session
            if (DateTime.Compare(date, d1) >= 0)
            {
                files.Add(fileName);
            }
        }
        return files;
    }

    /* Get the DateTime from a record name */
    private DateTime ParseDate(string dateString)
    {
        int pos = dateString.IndexOf(recordName + id);
        string date = dateString.Remove(pos, (recordName + id).Length);
        date = date.Remove(date.IndexOf(".wav"));
        return DateTime.ParseExact(date, dateFormat, null);
    }
    
    void PlayChosenAudio(string filename, AudioSource audioSource)
    {
        StartCoroutine(LoadAudio(filename, audioSource));
    }

    void ValidateEvaluation(GameObject obj, string filename)
    {
        // Delete sound file
        FileManager.DeleteFile(FileManager.tempDataFolder, filename);

        float score = obj.GetComponentInChildren<Scrollbar>().value;
        string label;
        if(score < 0.5)
        {
            label = MachineLearning.labelNotAnimated;
        }
        else
        {
            label = MachineLearning.labelAnimated;
        }
        string path = FileManager.tempDataFolder;

        string dateT = ParseDate(filename).ToString(dateFormat);
        string datafilename = outputName + id + dateT + ".csv";
        List<List<float>> data = FileManager.ReadOpensmileData(path, datafilename);
        data.RemoveAt(data.Count - 1);
        //data[0].Insert(0, score); // Compute an appropriate score or type of data

        // Delete Associated CSV
        FileManager.AddToCSV(FileManager.dataFolder, MachineLearning.dataFile, FileManager.tempDataFolder, datafilename, label, ";");
        FileManager.DeleteFile(path, datafilename);
        FileManager.DeleteFile(FileManager.tempAnswersDataFolder, textName + id + dateT + ".txt");

        targets.Clear();
        targets.Add(obj.transform.position);
        foreach(GameObject trg in GameObject.FindGameObjectsWithTag(audioPlayersTag))
        {
            if (trg.transform.position.y < obj.transform.position.y)
                targets.Add(trg.transform.position);
        }
        move = true;

        Destroy(obj);
    } 

    /* Load the audio into the AudioClip */
    private IEnumerator LoadAudio(string filename, AudioSource audioSource)
    {
        string soundPath = "file://" + FileManager.tempDataFolder;
        WWW request = GetAudioFromFile(soundPath, filename);
        yield return request;

        audioClip = request.GetAudioClip();
        audioClip.name = filename;

        PlayAudioFiles(audioSource);
    }

    /* Launch a file request for the specified filename */
    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = Path.Combine(path, filename);
        WWW request = new WWW(audioToLoad);
        return request;
    }

    /* Put the AudioClip into the AudioSource and play it */
    private void PlayAudioFiles(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void MoveObjects()
    {
        int acc = 0;
        foreach (GameObject others in GameObject.FindGameObjectsWithTag(audioPlayersTag))
        {
            if (others.transform.position.y < targets[acc].y)
            {
                others.transform.position = Vector3.Lerp(others.transform.position, targets[acc], Time.deltaTime);
                if (others.transform.position.y >= targets[acc].y)
                {
                    targets.RemoveAt(0);
                    move = false;
                    return;
                }
                acc++;
            }
        }
        
    }

    void DisableButtons()
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            if(button.tag != audioButtonsTag)
            {
                button.interactable = false;
            }
        }
    }

    void EnableButtons()
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            button.interactable = true;
        }
    }

}
