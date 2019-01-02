using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour {

	public Slider volumeSlider;
	public AudioSource musicPlayer;

	// Use this for initialization
	void Start () {
		volumeSlider.onValueChanged.AddListener(SetMusicListenerVolume);
	}

	private void SetMusicListenerVolume(float volume)
    {
        musicPlayer.volume = volume;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
