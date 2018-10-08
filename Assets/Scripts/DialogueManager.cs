using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
	public Text dialogueText;

	public Animator dialogueBoxAnimator;

	private Queue<string> sentences;

	// Use this for initialization
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
		}
	}

	void endDialogue()
	{
		Debug.Log("End of conversation.");

		dialogueBoxAnimator.SetBool("isOpen", false);
	}

}
