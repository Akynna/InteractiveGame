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

    string audioPlayersTag = "AudioPlayers";
    string audioButtonsTag = "AudioButton";
    string playBtn = "PlayButton";
    string validateButton = "ValidateButton";

    // TODO Make it to be the start date of the chapter you are in
    DateTime startDate;

    // Get all the data for the recordings
    string recordName = MicrophoneController.recordName;
    string outputName = MicrophoneController.output;
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
        }
        if (GameObject.FindGameObjectsWithTag(audioPlayersTag).Length == 0)
        {
            evaluating = false;
            move = false;
            EnableButtons();
        }

        if (move)
        {
            MoveObjects();
        }
    }

    // TODO Change the name of this function to a better one once the trigger happens in the scene change
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

            GameObject obj = Instantiate(audioPlayerTemplate);

            // Active fields
            obj.transform.Find(playBtn).GetComponent<Button>().onClick.AddListener(() => PlayChosenAudio(file, obj.GetComponentInChildren<AudioSource>()));
            obj.transform.Find(validateButton).GetComponent<Button>().onClick.AddListener(() => ValidateEvaluation(obj, file));
            //obj.GetComponentInChildren<InputField>.

            // Position of elements in screen space
            position = new Vector3(position.x, position.y - obj.GetComponent<RectTransform>().rect.height);
            obj.transform.position = position;
            obj.transform.SetParent(canvas.transform);
            obj.GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, obj.GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    /* Gets all the records that have been recorded since the start of the chapter scene */
    private List<string> GetRecords(DateTime d1)
    {
        DirectoryInfo d = new DirectoryInfo(Application.dataPath); // Directory at Assets
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
        FileManager.DeleteFile(Application.dataPath, filename);

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

        string dateT = ParseDate(filename).ToString(dateFormat);
        string datafilename = outputName + id + dateT + ".csv";
        List<List<float>> data = FileManager.ReadOpensmileData(datafilename);
        data.RemoveAt(data.Count - 1);
        //data[0].Insert(0, score); // Compute an appropriate score or type of data

        // Delete Associated CSV
        FileManager.AddToCSV(MachineLearning.dataFile, datafilename, label, ";");
        string path = Application.dataPath;
        int pos = path.IndexOf("Assets");
        path = path.Remove(pos);
        FileManager.DeleteFile(path, datafilename);

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
        string soundPath = "file://" + Application.dataPath;
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
                button.enabled = false;
            }
        }
    }

    void EnableButtons()
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            button.enabled = true;
        }
    }

}
