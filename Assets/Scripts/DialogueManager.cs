using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour {

	// Variables that the DialogueManager need to access
	public DialoguesTable dialoguesTable;
	public DialogueTrigger dialogueTrigger;

	// Managers with whom the Dialogue Manager communicates
	public ScoreManager scoreManager;
	public CharacterManager characterManager;
	public AudioManager audioManager;
	
	// Variables that the DialogueManager will change
	public List<DialoguesTable.Row> currentSceneDialogues;


	private Queue<string> characterNames;
	private Queue<string> sentences;
	private Queue<string> audioNames;
	

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

	// Used to save the logs
	private List<string> listAnswers;

	// Used for the sound of dialogue
	public AudioSource effectsSource;


	// Initialization
	void Start () {

		characterNames = new Queue<string>();
		sentences = new Queue<string>();
		audioNames = new Queue<string>();

		listAnswers = new List<string>();
		
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

		sceneChanger.switchBackground(currentSceneDialogues[0].background);
	}

	public void startDialogue(Dialogue dialogue) 
	{

		// Clear previous messages
		characterNames.Clear();
		sentences.Clear();
		audioNames.Clear();

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

		// Add the new next audio sentences in the queue
		foreach (string audioFile in dialogue.audioFileNames)
		{
			audioNames.Enqueue(audioFile);
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

			// Update the dialogue audio file
			string audioName = audioNames.Dequeue();
			audioManager.updateEffectSound(audioName);

			// Add animation to text
			if(dialogueBoxAnimator.GetBool("isOpen"))
			{
				StopAllCoroutines();
				StartCoroutine(typeSentence(sentence));
			}

			// Update character sprite
			characterManager.updateCharacterSprite(characterName);
		}
	}

	IEnumerator typeSentence(string sentence)
	{
		dialogueText.text = "";
		audioManager.playEffect();

		// Display the letters of the dialogue one by one
		foreach(char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}

		audioManager.pauseEffect();
	}

	private void endDialogue()
	{
		dialogueBoxAnimator.SetBool("isOpen", false);

		// Get the last rows of the scene
		DialoguesTable.Row lastRow = currentSceneDialogues[currentSceneDialogues.Count - 1];

		// If there is a choice to make at the end of the dialogue, display the choices
		if(lastRow.answer1 != "NA")
		{
			showChoices(lastRow);
		} else 
		{	
			// Switch to the default neutral scene
			switchScene(lastRow.next_scene1, 0, 0, 0, "");
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

		buttonList[0].GetComponentInChildren<Text>().text = rowWithChoices.answer1;
		buttonList[1].GetComponentInChildren<Text>().text = rowWithChoices.answer2;
		buttonList[2].GetComponentInChildren<Text>().text = rowWithChoices.answer3;

		// Initialize both empathy and emp score
		int goodEmpathyScore = 0;
		int badEmpathyScore = 0;
		int neutralEmpathyScore = 0;

		int goodSkillScore = 0;
		int badSkillScore = 0;
		int neutralSkillScore = 0;

		// Parse the scores if they exist
		if(rowWithChoices.score1 != "NA") {
			goodEmpathyScore = int.Parse(rowWithChoices.score1);
			badEmpathyScore = int.Parse(rowWithChoices.score2);
			neutralEmpathyScore = int.Parse(rowWithChoices.score3);

			goodSkillScore = int.Parse(rowWithChoices.score1);
			badSkillScore = int.Parse(rowWithChoices.score2);
			neutralSkillScore = int.Parse(rowWithChoices.score3);
		}
		
		// Remove old listeners
		buttonList[0].onClick.RemoveAllListeners();
		buttonList[1].onClick.RemoveAllListeners();
		buttonList[2].onClick.RemoveAllListeners();

		// Add new listeners to buttons
		buttonList[0].onClick.AddListener(() => switchScene(rowWithChoices.next_scene1, 1, goodEmpathyScore, goodSkillScore, rowWithChoices.answer1));
		buttonList[1].onClick.AddListener(() => switchScene(rowWithChoices.next_scene2, 2, badEmpathyScore, badSkillScore, rowWithChoices.answer2));
		buttonList[2].onClick.AddListener(() => switchScene(rowWithChoices.next_scene3, 0, neutralEmpathyScore, neutralSkillScore, rowWithChoices.answer3));

		choicePanelAnimator.SetBool("isDisplayed", true);
	}

	public void switchScene(string sceneID, int answerType, int empathyScore, int skillScore, string answerText)
	{
		// If we previously displayed the choice panel,
		if(choicePanelAnimator.GetBool("isDisplayed"))
		{
			// Hide the choice panel
			choicePanelAnimator.SetBool("isDisplayed", false);

			if(empathyScore != 0) {

				// Give a random feedback
				characterManager.randomFeedback(answerType);

				// Modify the relation score
				scoreManager.updatePoints(empathyScore, skillScore);
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
			dialogueTrigger.triggerDialogue();
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
