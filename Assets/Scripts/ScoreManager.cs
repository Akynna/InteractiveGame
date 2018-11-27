using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScoreManager : MonoBehaviour {

	private HashSet<string> characterNames;
	public HashSet<Character> characterList;
	public GameObject relationBar;
	public DialoguesTable dialoguesTable;
	public Character currentCharacter;
	public Text scoreText;

	public Animator scoreAnimator;

	void Start () {

		// Get characters' names
		characterNames = dialoguesTable.getCharacterNames();

		// Initialize the attributes of all characters
		characterList = new HashSet<Character>();

		foreach(string characterName in characterNames) {
			Character character = new Character(characterName, 0, Character.RelationState.Unknown);
			characterList.Add(character);
		}

		Text[] stateTexts = relationBar.GetComponentsInChildren<Text>();
		stateTexts[1].text = Character.RelationState.Unknown.ToString();
		stateTexts[3].text = Character.RelationState.Acquaintance.ToString();
	}

	public Character getCharacterByName(string characterName)
	{
		foreach(Character character in characterList)
		{
			if (String.Equals(character.name, characterName))
			{
				return new Character(character.name, character.score, character.relationState);
			}
		}
		Debug.Log("No such character found.");
		return null;
	}

	public void changeCurrentState(Character.RelationState newState)
	{
		Text[] stateTexts = relationBar.GetComponentsInChildren<Text>();
		stateTexts[1].text = newState.ToString();
	}

	public void updatePoints()
	{
		//changeScoreBar(currentCharacter.score, currentCharacter.score + points);
		//currentCharacter.score += points;
	}

	public void upScore(int points) {
		scoreAnimator.SetTrigger("Up");
		scoreText.text = "Score : +" + points.ToString();

	}

	private void changeScoreBar(int actualScore, int newScore)
	{
		/*Text[] texts = relationBar.GetComponentsInChildren<Text>();
		Debug.Log(texts[2].text);

		if (actualScore < newScore) {

    		 // Replace text with color value for character.
			for(int i=actualScore; i < newScore; i++) {
				texts[2].text = texts[2].text.Replace(texts[2].text[actualScore].ToString(), "<color=#ffffff>" + texts[2].text [actualScore].ToString () + "</color>");
			}

		} else if (newScore < actualScore) {
			
    		 // Replace text with color value for character.
			for(int i=actualScore; i > newScore; i--) {
				texts[2].text = texts[2].text.Replace (texts[2].text[actualScore].ToString(), "<color=#ffffff>" + texts[2].text [actualScore].ToString () + "</color>");
			}
		}*/
	}

}
