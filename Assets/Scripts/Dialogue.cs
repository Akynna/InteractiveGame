using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

	// Contains an ordered list of the characters involved in the dialogue
	public List<string> names;

	// Ordered list of sprites involved in this dialogue
	public List<string> spritesNames;

	// Defines the min and max lines a dialogue can have
	[TextArea(3, 10)]
	public List<string> sentences = new List<string>();

	// Ordered list of audio files
	public List<string> audioFileNames; 

}
