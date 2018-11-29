using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour {

	public Animator animator;
	public DialogueManager dialogueManager;
	public SpriteRenderer backgroundSprite;
	public SpriteRenderer characterSprite;
	public Camera mainCamera;
	public string currentBackgroundName;
	public string currentCharacterName;
	private int sceneToLoad;
	
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public void switchBackground(string backgroundName)
	{
		// If we notice a change in the background
		if (currentBackgroundName != backgroundName) {

			// Update the background and resize it to the screen
			backgroundSprite.sprite = Resources.Load<Sprite>("Backgrounds/Hospital/" + backgroundName);
			ResizeSpriteToScreen();

			// Reinitialize the character sprite
			characterSprite.sprite = null;
		}
	}

	public void switchCharacter(string characterName)
	{
		if(characterName != "Me" &&	 characterName != currentCharacterName)
		{
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + characterName);
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

	// Change the expression of a character
	public void changeCharacterFace(int faceType) {
		switch(faceType) 
		{
			case 1:
				Debug.Log("good");
				break;
			case 2:
				Debug.Log("bad.");
				break;
			default:
				Debug.Log("neutral");
				break;
		}

	}
 	
}
