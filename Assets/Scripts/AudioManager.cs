using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour {

	public Slider musicVolumeSlider;
	public AudioSource musicPlayer;
	public Slider effectsVolumeSlider;
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playEffect() {
		effectsPlayer.Play();
	}

	public void pauseEffect() {
		effectsPlayer.Pause();
	}

	public void updateEffectSound(string audioName) {
		effectsPlayer.clip = Resources.Load<AudioClip>("Soundtracks/Effects/" + audioName);
		// effectsPlayer.loop = true;
	}
}
