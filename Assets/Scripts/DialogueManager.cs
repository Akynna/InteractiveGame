using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour {

	// Variables that the DialogueManager need to access
	public DialoguesTable dialoguesTable;
	public DialogueTrigger dialogueTrigger;
	
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


	// Initialization
	void Start () {
		sentences = new Queue<string>();
		characterNames = new Queue<string>();
		
		// Initialize the first scene
		currentScene = "intro_0";
		currentSceneDialogues = dialoguesTable.FindAll_sceneID("intro_0");

		// Launch the first scene
		dialogueTrigger.triggerDialogue();

		// Set the background
		sceneChanger.currentBackgroundName = currentSceneDialogues[0].background;
		sceneChanger.switchBackground(sceneChanger.currentBackgroundName);
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
			nameText.text = characterNames.Dequeue();

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
			switchScene(lastRow.next_scene_neutral, 0);
		}
		 
	}

	// TODO : Make choices appear randomly on buttons
	private void showChoices(DialoguesTable.Row rowWithChoices) 
	{
		// Randomly assign a choice a button
		/*foreach (Button button in choicesPanel) {
			int randomIndex = Random.Range(0, choicesPanel.Count - 1);
			button.GetComponentsInChildren<UnityEngine.UI.Text>().Text = choices.good_answer;

     	}*/

		/*System.Random rnd = new System.Random();
		Debug.Log(choicesPanel.RandomItem());
		Debug.Log(choicesPanel[rnd.Next(0, choicesPanel.Count)]);
		Debug.Log(choicesPanel[rnd.Next(0, choicesPanel.Count)]);*/

		choicePanelAnimator.SetBool("isDisplayed", true);

		// TODO : Randomize the display order of the choices
		choicesPanel[0].GetComponentInChildren<Text>().text = rowWithChoices.good_answer;
		choicesPanel[1].GetComponentInChildren<Text>().text = rowWithChoices.bad_answer;
		choicesPanel[2].GetComponentInChildren<Text>().text = rowWithChoices.neutral_answer;

		choicesPanel[0].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_good, 1));
		choicesPanel[1].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_bad, 2));
		choicesPanel[2].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_neutral, 0));
	}

	public void switchScene(string sceneID, int answerType)
	{
		// If we previously displayed the choice panel, hide it
		choicePanelAnimator.SetBool("isDisplayed", false);

		// Load the dialogues of the next scene in the Dialogue Manager
		currentScene = sceneID;
		currentSceneDialogues =  dialoguesTable.FindAll_sceneID(sceneID);

		// Switch the background image if needed
		sceneChanger.switchBackground(currentSceneDialogues[0].background);

		// Trigger the dialogues of the next scene
		dialogueTrigger.triggerDialogue();

		randomFeedback(answerType);
	}
	
	// Choose randomly whether or not the character should give a feedback
	public void randomFeedback(int answerType) {

		// Generate a random number between 0 and 1
		System.Random r = new System.Random();
		int	giveFeedback = r.Next(0, 2);

		//Debug.Log("Feedback: " + giveFeedback);

		if(giveFeedback == 1)
		{
			switch(answerType) 
			{
				case 1:
					//Debug.Log("good");
					break;
				case 2:
					//Debug.Log("bad.");
					break;
				default:
					//Debug.Log("neutral");
					break;
			}
			// Make the character smile
			// Display the score increasing or decreasing
		}
	}
}
