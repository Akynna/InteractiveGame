using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class ArrayExtensions
{
	// This is an extension method. RandomItem() will now exist on all arrays.
	public static T RandomItem<T>(this T[] array)
	{
		return array[UnityEngine.Random.Range(0, array.Length)];
	}
}

public class DialogueManager : MonoBehaviour {

	// Variables that the DialogueManager need to use
	public DialoguesTable dialoguesTable;
	private Queue<string> sentences;

	// Variables that the DialogueManager will change
	public Text nameText;
	public Text dialogueText;
	public Animator dialogueBoxAnimator;
	public List<Button> choicesPanel;

	// Initialization
	void Start () {
		sentences = new Queue<string>();
	}

	public void startDialogue(Dialogue dialogue) 
	{
		// Print in the console the start of conversation
		//Debug.Log("Starting conversation with " + dialogue.name);

		dialogueBoxAnimator.SetBool("isOpen", true);

		// Set the character's name
		nameText.text = dialogue.name;

		// Clear previous messages
		sentences.Clear();

		// Add the new next sentences in the queue
		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		// Display the next sentence
		displayNextSentence();

	}

	public void displayNextSentence()	
	{
		// Check if there is more sentences in the queue
		if(sentences.Count == 0)
		{
			endDialogue();
		} else {
			// Collect the next sentence
			string sentence = sentences.Dequeue();

			//Debug.Log(sentence);
			dialogueText.text = sentence;

			// Uncomment to add animation to text
			//StopAllCoroutines();
			//StartCoroutine(typeSentence(sentence));
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

		showChoices();
	}

	// TODO : Make choices appear randomly on buttons
	private void showChoices() 
	{
		// Get the corresponding scene
		List<DialoguesTable.Row> listChoices = dialoguesTable.FindAll_sceneID("1");

		// Randomly assign a choice a button
		/*foreach (Button button in choicesPanel) {
			int randomIndex = Random.Range(0, choicesPanel.Count - 1);
			button.GetComponentsInChildren<UnityEngine.UI.Text>().Text = choices.good_answer;

     	}*/

		/*System.Random rnd = new System.Random();
		Debug.Log(choicesPanel.RandomItem());
		Debug.Log(choicesPanel[rnd.Next(0, choicesPanel.Count)]);
		Debug.Log(choicesPanel[rnd.Next(0, choicesPanel.Count)]);*/

		foreach(DialoguesTable.Row row in listChoices) {
			if(row.good_answer != "0") {
				choicesPanel[0].GetComponentInChildren<Text>().text = row.good_answer;
				choicesPanel[1].GetComponentInChildren<Text>().text = row.wrong_answer;
			}
		}
	}
}
