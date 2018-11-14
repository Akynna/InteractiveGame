using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;
	public DialogueManager dialogueManager;
	public DialoguesTable dialoguesTable;

	public void Start()
	{	
		// Initialize the dialogue content
		dialogue = new Dialogue();
		dialogue.names = new List<string>();
		dialogue.sentences = new List<string>();
	}

	public void triggerDialogue()
	{
		// Clear the list at the beginning
		dialogue.names.Clear();
		dialogue.sentences.Clear();

		List<DialoguesTable.Row> sceneDialogues = dialogueManager.currentSceneDialogues;

		foreach(DialoguesTable.Row row in sceneDialogues)
		{
			dialogue.names.Add(row.character);
			dialogue.sentences.Add(row.dialogue);
		}

		//Debug.Log(dialogue.names.Count);

		dialogueManager.startDialogue(dialogue);
	}
}
