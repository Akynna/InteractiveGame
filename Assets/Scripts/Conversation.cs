using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour {

	public int id;
	public List<Dialogue> dialogues;

	// Contructor
	public Conversation(int id, List<Dialogue> dialogues)
	{
		this.id = id;
		this.dialogues = dialogues;
	}

	// Initialize the dialogue as empty at the beginning
	void Start () {
		id = 0;
		dialogues = new List<Dialogue>();
	}
}
