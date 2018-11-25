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
		/*if(Input.GetMouseButtonDown(0))
		{
			FadeToLevel(1);
		}*/
	}

	public void switchBackground(string backgroundName)
	{
		if (currentBackgroundName != backgroundName)
			backgroundSprite.sprite = Resources.Load<Sprite>("Backgrounds/Hospital/" + backgroundName);
			ResizeSpriteToScreen();
	}

	public void switchCharacter(string characterName)
	{
		if(characterName != "Me" && characterName != currentCharacterName)
		{
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + characterName);
		}
	}

	public void FadeToLevel(int levelIndex)
	{
		animator.SetTrigger("FadeOut");
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
