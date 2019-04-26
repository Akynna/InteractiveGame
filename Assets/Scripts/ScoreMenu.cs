using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreMenu : MonoBehaviour {

	public void Start()
	{
		TextMeshProUGUI[] texts = this.GetComponentsInChildren<TextMeshProUGUI>();
		TextMeshProUGUI namesText = texts[1];
		TextMeshProUGUI empScoresText = texts[2];
		TextMeshProUGUI taskScoresText = texts[3];

		namesText.text = "Characters\n";
		empScoresText.text = "Empathy\n";
		taskScoresText.text = "Task\n";

		foreach(Character character in CharacterManager.characterList)
		{
			namesText.text += character.name + "\n";
			empScoresText.text += character.empathyScore + "\n";
			taskScoresText.text += character.taskScore + "\n";
		}
	}
}
