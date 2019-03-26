using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*
 *	This is the main Manager. The Story Manager is the only one that have
 *  access to the Dialogues Table. This Manager communicates with all other
 *	Managers to to build the story.
 */
public class StoryManager : MonoBehaviour {


	public DialoguesTable dialoguesTable;

	// Managers with whom the Story Manager communicates
	public AudioManager AudioManager;
	public CharacterManager CharacterManager;
	public DialogueManager DialogueManager;
	public ScoreManager ScoreManager;
	
	
	public List<DialoguesTable.Row> currentSceneDialogues;

	// Variables for the backgrounds
	public SceneChanger sceneChanger;

	// Used to save the logs
	private List<string> listAnswers;

	// Used for the sound of dialogue
	public AudioSource effectsSource;


	// Initialization
	void Start () {

		// Initialize all the Managers
		CharacterManager.Initialize();
		DialogueManager.Initialize();
		
		// Used to save the logs
		listAnswers = new List<string>();
		
		// Initialize the first scene
		currentSceneDialogues = dialoguesTable.FindAll_sceneID("intro_0");
		
		// Initialize the background
		sceneChanger.switchBackground(currentSceneDialogues[0].background);

		// Launch the first scene !
		DialogueManager.TriggerDialogue();
	}

	// Used to give access to characters' names to other Managers
	public HashSet<string> getCharacterNames() {
		return new HashSet<string>(dialoguesTable.getCharacterNames());
	}

	public void SwitchScene(string sceneID, int answerType, int empathyScore, int skillScore, string answerText)
	{
		// If we previously displayed the choice panel
		if(DialogueManager.WasAChoice())
		{	
			if(empathyScore != 0) {

				// Give a random feedback
				// characterManager.randomFeedback(answerType);
				//Debug.Log("coucou");
				// Modify the relation score
				ScoreManager.updatePoints(empathyScore, skillScore);
			}	
		}

		listAnswers.Add(answerText);

		if(sceneID == "end") {
			sceneChanger.FadeToLevel(2);
			saveAnswers();

		} else {
			// Load the dialogues of the next scene in the Dialogue Manager
			currentSceneDialogues =  dialoguesTable.FindAll_sceneID(sceneID);

			// Switch the background image if needed
			sceneChanger.switchBackground(currentSceneDialogues[0].background);
			
			// Trigger the dialogues of the next scene
			DialogueManager.TriggerDialogue();

		}
	}

	private void saveAnswers() {

		using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter("testfile.txt", false))

		foreach(string answer in listAnswers)
		{
			file.WriteLine(answer);
		}
	}
	
	
}
