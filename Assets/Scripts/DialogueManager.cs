using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour {

	// Variables that the DialogueManager need to access
	public DialoguesTable dialoguesTable;
	public DialogueTrigger dialogueTrigger;
	public ScoreManager scoreManager;
	
	// Variables that the DialogueManager will change
	public string currentScene = "intro_0";
	public List<DialoguesTable.Row> currentSceneDialogues;
	private Queue<string> characterNames;
	private Queue<string> sentences;
	

	// Element of one dialogue element
	public Text nameText;
	public Text dialogueText;
	public Animator dialogueBoxAnimator;
	public Animator choicePanelAnimator;
	public List<Button> choicesPanel;

	// Variables for the backgrounds
	public SceneChanger sceneChanger;
	public Boolean switchBackground = false;

	float[] y_positions = {-159.1475f, -189.86f, -220.5725f};


	// Initialization
	void Start () {
		sentences = new Queue<string>();
		characterNames = new Queue<string>();
		
		// Initialize the first scene
		currentScene = "intro_0";
		currentSceneDialogues = dialoguesTable.FindAll_sceneID("intro_0");

		// Launch the first scene's
		dialogueTrigger.triggerDialogue();
	}

	public void startDialogue(Dialogue dialogue) 
	{
		// Move up the dialogue box
		dialogueBoxAnimator.SetBool("isOpen", true);

		// Clear previous messages
		characterNames.Clear();
		sentences.Clear();

		// List the characters involed in the conversation
		foreach (string name in dialogue.names)
		{
			characterNames.Enqueue(name);
		}

		// Add the new next sentences in the queue
		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		// Display the first sentence of the dialogue
		displayNextSentence();
	}

	public void displayNextSentence()	
	{
		// Check if there is more sentences in the queue
		if(sentences.Count == 0)
		{
			endDialogue();
		} else
		{
			
			// Set the character's name
			string characterName = characterNames.Dequeue();
			sceneChanger.switchCharacter(characterName);
			nameText.text = characterName;

			// Find the current character talking
			scoreManager.currentCharacter = scoreManager.getCharacterByName(characterName);
			
			// Collect the next sentence
			string sentence = sentences.Dequeue();

			// Add animation to text
			if(dialogueBoxAnimator.GetBool("isOpen"))
			{
				StopAllCoroutines();
				StartCoroutine(typeSentence(sentence));
			}
		}
	}

	IEnumerator typeSentence(string sentence)
	{
		dialogueText.text = "";

		// Display the letters of the dialogue one by one
		foreach(char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	private void endDialogue()
	{
		Debug.Log("End of conversation.");
		dialogueBoxAnimator.SetBool("isOpen", false);

		// Get the last rows of the scene
		DialoguesTable.Row lastRow = currentSceneDialogues[currentSceneDialogues.Count - 1];

		// If there is a choice to make at the end of the dialogue, display the choices
		if(lastRow.neutral_answer != "NA")
		{
			showChoices(lastRow);
		} else 
		{	
			// Switch to the default neutral scene
			switchScene(lastRow.next_scene_neutral, 0, 0);
		}
		 
	}

	// TODO : Make choices appear randomly on buttons
	private void showChoices(DialoguesTable.Row rowWithChoices) 
	{
		// Randomly assign a choice to a button
		System.Random r = new System.Random();
		int randIndex = r.Next(0, 3);

		// Create a list of used number to avoid repetition
		List<int> usedNumbers = new List<int>();

		for(int i=0; i < 3; ++i)
		{
			while(usedNumbers.Contains(randIndex)) {
				randIndex = r.Next(0, 3);
			}
			choicesPanel[i].transform.position = new Vector3(choicesPanel[i].transform.position.x, y_positions[randIndex], choicesPanel[i].transform.position.z);
			choicesPanel[i].GetComponentsInChildren<Text>()[1].text = (randIndex + 1).ToString();
			usedNumbers.Add(randIndex);
		}

		usedNumbers.Clear();

		// TODO : Repair bug

		choicesPanel[0].GetComponentInChildren<Text>().text = rowWithChoices.good_answer;
		choicesPanel[1].GetComponentInChildren<Text>().text = rowWithChoices.bad_answer;
		choicesPanel[2].GetComponentInChildren<Text>().text = rowWithChoices.neutral_answer;

		int goodScore = 0;
		int badScore = 0;
		int neutralScore = 0;

		if(rowWithChoices.good_score != "NA") {
			goodScore = int.Parse(rowWithChoices.good_score);
			badScore = int.Parse(rowWithChoices.bad_score);
			neutralScore = int.Parse(rowWithChoices.neutral_score);
		}

		choicesPanel[0].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_good, 1, goodScore));
		choicesPanel[1].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_bad, 2, badScore));
		choicesPanel[2].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_neutral, 0, neutralScore));

		choicePanelAnimator.SetBool("isDisplayed", true);
	}

	public void switchScene(string sceneID, int answerType, int score)
	{
		// If we previously displayed the choice panel,
		if(choicePanelAnimator.GetBool("isDisplayed"))
		{
			// Hide the choice panel
			choicePanelAnimator.SetBool("isDisplayed", false);

			// Give a random feedback
			randomFeedback(answerType);

			if(score > 0) {
				new WaitForSeconds(10);
				scoreManager.upScore(score);
			}

			// Modify the relation score
			//scoreManager.updatePoints(score);
		}

		if(sceneID == "end") {
			sceneChanger.FadeToLevel(2);
			
		} else {
			// Load the dialogues of the next scene in the Dialogue Manager
			currentScene = sceneID;
			currentSceneDialogues =  dialoguesTable.FindAll_sceneID(sceneID);

			// Switch the background image if needed
			sceneChanger.switchBackground(currentSceneDialogues[0].background);

			// Trigger the dialogues of the next scene
			dialogueTrigger.triggerDialogue();
		}
	}
	
	// Choose randomly whether or not the character should give a feedback
	public void randomFeedback(int answerType) {

		// Generate a random number between 0 and 1
		System.Random r = new System.Random();
		int	giveFeedback = r.Next(0, 2);

		Debug.Log("Feedback: " + giveFeedback);

		if(giveFeedback == 1)
		{
			switch(answerType) 
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
			// Make the character smile
			// Display the score increasing or decreasing
		}
	}
}
