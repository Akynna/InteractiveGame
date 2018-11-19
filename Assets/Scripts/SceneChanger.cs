using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

	public Animator animator;
	public DialogueManager dialogueManager;
	public SpriteRenderer backgroundSprite;
	public string currentBackgroundName;
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
			backgroundSprite.sprite = Resources.Load<Sprite>("Environments/Backgrounds/" + backgroundName);

	}

	public void FadeToLevel(int levelIndex)
	{
		animator.SetTrigger("FadeOut");
	}

	public void OnFadeComplete()
	{
		SceneManager.LoadScene(sceneToLoad);
	}
}
