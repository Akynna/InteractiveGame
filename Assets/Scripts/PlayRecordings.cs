using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayRecordings : MonoBehaviour
{

    GameObject playButton;

    public AudioClip test;

    public AudioSource records;
    public AudioClip audioClip;

    DateTime startDate;

    int fileNr = 0;

    string recordName = MicrophoneController.recordName;
    string id = MicrophoneController.id;
    string dateFormat = MicrophoneController.dateFormat;
    public static string date = MicrophoneController.date;

    // Start is called before the first frame update
    void Start()
    {
        /* Start the objects for the play move it to a new script */
        playButton = GameObject.Find("PlayRecords");

        records = gameObject.GetComponent<AudioSource>();
        //records.clip = test;
        records.playOnAwake = false;

        startDate = DateTime.Now;
    }

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

    private DateTime ParseDate(string dateString)
    {
        int pos = dateString.IndexOf(recordName + id);
        string date = dateString.Remove(pos, (recordName + id).Length);
        date = date.Remove(date.IndexOf(".wav"));
        return DateTime.ParseExact(date, "yyyyMMddHHmmss", null);
    }

    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = Path.Combine(path, filename);
        Debug.Log(audioToLoad);
        WWW request = new WWW(audioToLoad);
        return request;
    }

    public void PlayRecords()
    {
        //records.PlayOneShot(test);
        StartCoroutine(LoadAudio());
        //PlayAudioFiles();
    }

    private IEnumerator LoadAudio()
    {
        List<string> recordFiles = GetRecords(startDate);
        string soundPath = "file://" + Application.dataPath;
        WWW request = GetAudioFromFile(soundPath, recordFiles[fileNr]);
        yield return request;

        audioClip = request.GetAudioClip();
        audioClip.name = recordFiles[fileNr];

        fileNr++;
        PlayAudioFiles();
    }

    private void PlayAudioFiles()
    {
        //Load(recordFiles[fileNr], records);
        //Debug.Log(records.clip.name);
        records.clip = audioClip;
        records.Play();
        Debug.Log(records.isPlaying);
    }

    public static IEnumerator<WWW> Load(string filename, AudioSource audioSource)
    {
        if (!String.IsNullOrEmpty(filename) && audioSource != null)
        {
            string path = GetPath(filename);

            if (File.Exists(path))
            {
                WWW www = new WWW("file:" + path);
                yield return www;
                audioSource.clip = www.GetAudioClip(false, false, AudioType.WAV);
                audioSource.clip.name = filename;
            }
        }
        yield break;
    }

    // Returns the used path with filename and its ending
    private static string GetPath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename.EndsWith(".wav") ? filename : filename + ".wav");
    }
}
