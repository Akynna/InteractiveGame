﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour {

	public Animator animator;
	public StoryManager dialogueManager;
	public CharacterManager characterManager;
	public SpriteRenderer backgroundSprite;
	public Camera mainCamera;
	public string currentBackgroundName;
	private int sceneToLoad;
	
	void Start () {
		currentBackgroundName = "";
	}

	public void switchBackground(string backgroundName)
	{
		// If we notice a change in the background	
		if (currentBackgroundName != backgroundName) {

			// Update the background and resize it to the screen
			backgroundSprite.sprite = Resources.Load<Sprite>("Backgrounds/Hospital/" + backgroundName);
			currentBackgroundName = backgroundName;
			ResizeSpriteToScreen();

			// Reset the current character sprite
			characterManager.resetCharacterSprite();
		}


	}

	public void FadeToLevel(int levelIndex)
	{
		animator.SetTrigger("FadeOut");
		sceneToLoad = levelIndex;
	}

	public void OnFadeComplete()
	{
		SceneManager.LoadScene(sceneToLoad);
	}

	// Used to make the background image fit to the camera
	public void ResizeSpriteToScreen() {

		var width = backgroundSprite.sprite.bounds.size.x;
		var height = backgroundSprite.sprite.bounds.size.y;

		var worldScreenHeight = mainCamera.orthographicSize * 2.0;
		var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		Vector3 lTemp = transform.localScale;
 		lTemp.x = (float) worldScreenWidth / width;
		lTemp.y = (float) worldScreenHeight / height;

		backgroundSprite.transform.localScale = lTemp;
	} 	
}
