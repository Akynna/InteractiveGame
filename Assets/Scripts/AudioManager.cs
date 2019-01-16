using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour {

	public Slider musicVolumeSlider;
	public AudioSource musicPlayer;
	public Slider effectsVolumeSlider;
	public AudioSource speechPlayer;
	public AudioSource effectsPlayer;

	// Use this for initialization
	void Start () {
		musicVolumeSlider.onValueChanged.AddListener(SetMusicListenerVolume);
		effectsVolumeSlider.onValueChanged.AddListener(SetEffetcsListenerVolume);

		updateEffectSound("");
	}

	private void SetMusicListenerVolume(float volume)
    {
        musicPlayer.volume = volume;
    }

	private void SetEffetcsListenerVolume(float volume)
    {
        effectsPlayer.volume = volume;
		speechPlayer.volume = volume;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playEffect() {
		speechPlayer.Play();
	}

	public void pauseEffect() {
		speechPlayer.Pause();
	}

	public void updateEffectSound(string audioName) {
		speechPlayer.clip = Resources.Load<AudioClip>("Soundtracks/Effects/" + audioName);
	}
}
