using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*
 *	This Manager is one of the most important one. It handles the order
 *  of the dialogues and also how they are displayed on the screen.
 *
 */
public class DialogueManager : MonoBehaviour {

	// Managers with whom the Dialogue Manager communicates
	public StoryManager StoryManager;
	public AudioManager AudioManager;
	public CharacterManager CharacterManager;

	public Dialogue dialogue;

	// Ordered elements of a dialogue
	private Queue<string> characterNames;
	private Queue<string> characterSpritesNames;
	private Queue<string> sentences;
	private Queue<string> audioNames;
	
	// Elements that the Manager will keep track of
	public Text nameText;
	public Text dialogueText;


	//====================================
	//		VISUAL INTERFACE ELEMENTS
	//====================================

	// Buttons
	public Button continueButton;
	private Button[] buttonList;
	private float[] y_positions = new float[3];
	public GameObject choicesPanel;

	// Animators of the dialogue box
	public Animator dialogueBoxAnimator;
	public Animator choicePanelAnimator;
	
	
	public void Start()
	{	
		// Initialize the dialogue content
		dialogue = new Dialogue();

		dialogue.names = new List<string>();
		dialogue.spritesNames = new List<string>();
		dialogue.sentences = new List<string>();
		dialogue.audioFileNames = new List<string>();
	}

	public void Initialize() {

		// Initialize the stack of elements of a dialogue
		characterNames = new Queue<string>();
		characterSpritesNames = new Queue<string>();
		sentences = new Queue<string>();
		audioNames = new Queue<string>();

		// Initialize the button positions
		buttonList = choicesPanel.GetComponentsInChildren<Button>();

		for(int i=0; i < 3; ++i) {
			y_positions[i] = buttonList[i].transform.position.y;
		}

		continueButton.onClick.AddListener(() => DisplayNextSentence());
	}

	public void TriggerDialogue()
	{
		// Clear the list at the beginning
		dialogue.names.Clear();
		dialogue.spritesNames.Clear();
		dialogue.sentences.Clear();
		dialogue.audioFileNames.Clear();

		// Get the dialogues of the current scene
		List<DialoguesTable.Row> sceneDialogues = StoryManager.currentSceneDialogues;

		foreach(DialoguesTable.Row row in sceneDialogues)
		{
			dialogue.names.Add(row.character);
			dialogue.spritesNames.Add(row.character_image);
			dialogue.sentences.Add(row.dialogue);
			dialogue.audioFileNames.Add(row.dialogue_audio);
		}
		
		StartDialogue(dialogue);
	}

	public void StartDialogue(Dialogue dialogue) 
	{

		// Clear previous messages
		characterNames.Clear();
		characterSpritesNames.Clear();
		sentences.Clear();
		audioNames.Clear();

		// Move up the dialogue box
		dialogueBoxAnimator.SetBool("isOpen", true);

		// List the characters involed in the conversation
		foreach (string name in dialogue.names)
		{
			characterNames.Enqueue(name);
		}

		// List the characters sprites that will be displayed
		foreach (string name in dialogue.spritesNames)
		{
			characterSpritesNames.Enqueue(name);
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
		DisplayNextSentence();
	}

	public void DisplayNextSentence()	
	{
		// Check if there is more sentences in the queue
		if(sentences.Count == 0)
		{
			EndDialogue();
		} else
		{
			// Collect the next dialogue informations
			string characterName = characterNames.Dequeue();
			string spriteName = characterSpritesNames.Dequeue();
			string sentence = sentences.Dequeue();
			string audioName = audioNames.Dequeue();

			// Update the character's name and dialogue's sound
			nameText.text = characterName;

			if(characterName!= "Me") {
				CharacterManager.currentCharacter = CharacterManager.GetCharacterByName(characterName);
			}
			
			AudioManager.UpdateEffectSound(audioName);

			// Add animation to text
			if(dialogueBoxAnimator.GetBool("isOpen"))
			{
				StopAllCoroutines();
				StartCoroutine(TypeSentence(sentence));
			}

			// Update character sprite
			CharacterManager.UpdateCharacterSprite(spriteName);
		}
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		AudioManager.PlayEffect();

		// Display the letters of the dialogue one by one
		foreach(char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}

		AudioManager.PauseEffect();
	}

	private void EndDialogue()
	{
		dialogueBoxAnimator.SetBool("isOpen", false);

		// Get the last rows of the scene
		DialoguesTable.Row lastRow = StoryManager.currentSceneDialogues[StoryManager.currentSceneDialogues.Count - 1];

		// If there is a choice to make at the end of the dialogue, display the choices
		if(lastRow.answer1 != "NA")
		{
			showChoices(lastRow);
		} else 
		{	
			// Switch to the default neutral scene
			StoryManager.SwitchScene(lastRow.next_scene1, "", "", 0, "");
		}
	}

	public void showChoices(DialoguesTable.Row rowWithChoices) 
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

		// Find the current skill and subskill that are evaluated
		string skillName = rowWithChoices.main_skill;
		string subskillName = rowWithChoices.sceneID;

		// Initialize both empathy and emp score
		int goodEmpathyScore = 0;
		int badEmpathyScore = 0;
		int neutralEmpathyScore = 0;

		/*int goodSkillScore = 0;
		int badSkillScore = 0;
		int neutralSkillScore = 0;*/

		// Parse the scores if they exist
		if(rowWithChoices.score1 != "NA") {
			goodEmpathyScore = int.Parse(rowWithChoices.score1);
			badEmpathyScore = int.Parse(rowWithChoices.score2);
			neutralEmpathyScore = int.Parse(rowWithChoices.score3);

			/*goodSkillScore = int.Parse(rowWithChoices.score1);
			badSkillScore = int.Parse(rowWithChoices.score2);
			neutralSkillScore = int.Parse(rowWithChoices.score3);*/
		}
		
		// Remove old listeners
		buttonList[0].onClick.RemoveAllListeners();
		buttonList[1].onClick.RemoveAllListeners();
		buttonList[2].onClick.RemoveAllListeners();

		// Add new listeners to buttons
		buttonList[0].onClick.AddListener(() => StoryManager.SwitchScene(rowWithChoices.next_scene1, skillName, subskillName, goodEmpathyScore, rowWithChoices.answer1));
		buttonList[1].onClick.AddListener(() => StoryManager.SwitchScene(rowWithChoices.next_scene2, skillName, subskillName, badEmpathyScore, rowWithChoices.answer2));
		buttonList[2].onClick.AddListener(() => StoryManager.SwitchScene(rowWithChoices.next_scene3, skillName, subskillName, neutralEmpathyScore, rowWithChoices.answer3));

		choicePanelAnimator.SetBool("isDisplayed", true);
	}

	public Boolean WasAChoice() {

		// If we previously displayed the choice panel,
		if(choicePanelAnimator.GetBool("isDisplayed")) {

			// Hide the choice panel
			choicePanelAnimator.SetBool("isDisplayed", false);

			return true;
		}
		return false;
		
	}
}
