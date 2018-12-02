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
	public CharacterManager characterManager;
	
	// Variables that the DialogueManager will change
	public List<DialoguesTable.Row> currentSceneDialogues;
	private Queue<string> characterNames;
	private Queue<string> sentences;
	

	// Element of one dialogue element
	public Text nameText;
	public Text dialogueText;

	public GameObject choicesPanel;
	private Button[] buttonList;
	public Button continueButton;

	// Animators
	public Animator dialogueBoxAnimator;
	public Animator choicePanelAnimator;

	// Variables for the backgrounds
	public SceneChanger sceneChanger;

	private float[] y_positions = new float[3];
	private int isFeedBack = 0;

	// Initialization
	void Start () {

		characterNames = new Queue<string>();
		sentences = new Queue<string>();
		
		// Initialize the first scene
		currentSceneDialogues = dialoguesTable.FindAll_sceneID("intro_0");

		// Launch the first scene's
		dialogueTrigger.triggerDialogue();

		// Initialize the button positions
		buttonList = choicesPanel.GetComponentsInChildren<Button>();

		for(int i=0; i < 3; ++i) {
			y_positions[i] = buttonList[i].transform.position.y;
		}

		continueButton.onClick.AddListener(() => displayNextSentence());
	}

	public void startDialogue(Dialogue dialogue) 
	{

		// Clear previous messages
		characterNames.Clear();
		sentences.Clear();

		// Move up the dialogue box
		dialogueBoxAnimator.SetBool("isOpen", true);

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
			nameText.text = characterName;

			// Collect the next sentence
			string sentence = sentences.Dequeue();

			// Add animation to text
			if(dialogueBoxAnimator.GetBool("isOpen"))
			{
				StopAllCoroutines();
				StartCoroutine(typeSentence(sentence));
			}

			// Find the current character talking and set the appropriate sprite
			if(characterManager.isFeedBack == 0) {
				if(characterName != "Me" && characterManager.currentCharacter.name != characterName)
				{
					characterManager.currentCharacter = characterManager.getCharacterByName(characterName);
					characterManager.currentSpriteName = characterName;
				} 
			} else
			{
				characterManager.isFeedBack = 0;
				characterManager.currentCharacter.name = " ";
			}

			scoreManager.updatePoints(0);
			characterManager.updateCharacterSprite();
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
			buttonList[i].transform.position = new Vector3(buttonList[i].transform.position.x, y_positions[randIndex], buttonList[i].transform.position.z);
			buttonList[i].GetComponentsInChildren<Text>()[1].text = (randIndex + 1).ToString();
			usedNumbers.Add(randIndex);
		}
		usedNumbers.Clear();

		buttonList[0].GetComponentInChildren<Text>().text = rowWithChoices.good_answer;
		buttonList[1].GetComponentInChildren<Text>().text = rowWithChoices.bad_answer;
		buttonList[2].GetComponentInChildren<Text>().text = rowWithChoices.neutral_answer;

		int goodScore = 0;
		int badScore = 0;
		int neutralScore = 0;

		if(rowWithChoices.neutral_score != "NA") {
			goodScore = int.Parse(rowWithChoices.good_score);
			badScore = int.Parse(rowWithChoices.bad_score);
			neutralScore = int.Parse(rowWithChoices.neutral_score);
		}

		// Remove old listeners
		buttonList[0].onClick.RemoveAllListeners();
		buttonList[1].onClick.RemoveAllListeners();
		buttonList[2].onClick.RemoveAllListeners();

		// Add new listeners to buttons
		buttonList[0].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_good, 1, goodScore));
		buttonList[1].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_bad, 2, badScore));
		buttonList[2].onClick.AddListener(() => switchScene(rowWithChoices.next_scene_neutral, 0, neutralScore));

		choicePanelAnimator.SetBool("isDisplayed", true);
	}

	public void switchScene(string sceneID, int answerType, int score)
	{
		// If we previously displayed the choice panel,
		if(choicePanelAnimator.GetBool("isDisplayed"))
		{
			// Hide the choice panel
			choicePanelAnimator.SetBool("isDisplayed", false);

			if(score != 0) {

				// Give a random feedback
				characterManager.randomFeedback(answerType);

				// Modify the relation score
				scoreManager.updatePoints(score);
			}	
		}

		if(sceneID == "end") {
			sceneChanger.FadeToLevel(2);
			
		} else {
			// Load the dialogues of the next scene in the Dialogue Manager
			currentSceneDialogues =  dialoguesTable.FindAll_sceneID(sceneID);

			// Switch the character and background images if needed
			sceneChanger.switchBackground(currentSceneDialogues[0].background);

			// Trigger the dialogues of the next scene
			dialogueTrigger.triggerDialogue();
		}
	}
	
	
}
