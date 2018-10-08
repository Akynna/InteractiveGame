using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

	public string name;

	// Defines the min and max lines a dialogue can have
	[TextArea(3, 10)]
	public List<string> sentences;

}
