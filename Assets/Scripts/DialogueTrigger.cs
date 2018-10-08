using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	private Dialogue dialogue;
	public DialoguesTable dialoguesTable;

	public void Start()
	{	
		dialogue = new Dialogue();
		dialogue.name = "Unknown";
		dialogue.sentences = new List<string>();
	}

	public void triggerDialogue()
	{
		// Debug.Log(dialoguesTable.Find_sceneID("1").character);
		dialogue.name = dialoguesTable.Find_sceneID("1").character;

		List<DialoguesTable.Row> dialoguesRows = dialoguesTable.FindAll_sceneID("1");
		int nbRows = dialoguesRows.Count;

		foreach(DialoguesTable.Row row in dialoguesRows)
		{
			dialogue.sentences.Add(row.dialogue);
		}

		FindObjectOfType<DialogueManager>().startDialogue(dialogue);
	}
}
