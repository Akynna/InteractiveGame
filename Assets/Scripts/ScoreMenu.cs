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
		TextMeshProUGUI skillScoresText = texts[3];

		namesText.text = "Characters\n";
		empScoresText.text = "Empathy\n";
		skillScoresText.text = "Skill\n";

		foreach(Character character in CharacterManager.characterList)
		{
			namesText.text += character.name + "\n";
			empScoresText.text += character.empathyScore + "\n";
			skillScoresText.text += character.skillScore + "\n";
		}
	}
}
