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

	public static float currentMusicVolume;
	public static float currentEffectsVolume;

	// Use this for initialization
	public void Start () {
		musicVolumeSlider.onValueChanged.AddListener(SetMusicListenerVolume);
		effectsVolumeSlider.onValueChanged.AddListener(SetEffectsListenerVolume);

		UpdateEffectSound("");
	}

	public void Initialize() {

		// Set the volumes to the ones from the previous scene
		musicPlayer.volume = PlayerPrefs.GetFloat("MusicSliderVolumeLevel", musicPlayer.volume);
		effectsPlayer.volume = PlayerPrefs.GetFloat("EffectsSliderVolumeLevel", effectsPlayer.volume);

		// Set the slider values to the ones from the previous scene
		musicVolumeSlider.value = musicPlayer.volume;
		effectsVolumeSlider.value = effectsPlayer.volume;

	}

	private void SetMusicListenerVolume(float volume)
    {
        musicPlayer.volume = volume;
		currentMusicVolume = volume;
    }

	private void SetEffectsListenerVolume(float volume)
    {
        effectsPlayer.volume = volume;
		speechPlayer.volume = volume;

		currentEffectsVolume = volume;
    }

	public void PlayEffect() {
		speechPlayer.Play();
	}

	public void PauseEffect() {
		speechPlayer.Pause();
	}

	public void UpdateEffectSound(string audioName) {
		speechPlayer.clip = Resources.Load<AudioClip>("Soundtracks/Effects/" + audioName);
	}

	// Used to save the volume when switching scene
	public void SaveSliderValues()
 	{
     	PlayerPrefs.SetFloat("MusicSliderVolumeLevel", currentMusicVolume);
		PlayerPrefs.SetFloat("EffectsSliderVolumeLevel", currentEffectsVolume);
 	}
}
