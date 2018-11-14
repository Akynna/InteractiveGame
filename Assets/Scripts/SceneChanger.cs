using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

	public Animator animator;
	public DialogueManager dialogueManager;
	private int sceneToLoad;
	
	public List<Background> backgroundsList;

	void Start () {
		backgroundsList = new List<Background>();

		initializeBackgrounds();
	}

	// Update is called once per frame
	void Update () {
		/*if(Input.GetMouseButtonDown(0))
		{
			FadeToLevel(1);
		}*/
	}

	public void initializeBackgrounds()
	{
		backgroundsList.Add(new Background("Lockers", 15, -10, 0));
		backgroundsList.Add(new Background("Classroom", 0, -10, 0));
		backgroundsList.Add(new Background("Behind", -15, -10, 0));
		backgroundsList.Add(new Background("Corridor", 0, 0, 0));
	}

	public float getBackgroundX(string name)
	{
		foreach(Background background in backgroundsList)
		{
			if(name == background.name)
			{
				return background.xCoords;
			}
		}
		return 0.0f;
	}

	public float getBackgroundY(string name)
	{
		foreach(Background background in backgroundsList)
		{
			if(name == background.name)
			{
				return background.yCoords;
			}
		}
		return 0.0f;
	}

	public float getBackgroundZ(string name)
	{
		foreach(Background background in backgroundsList)
		{
			if(name == background.name)
			{
				return background.zCoords;
			}
		}
		return 0.0f;
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
