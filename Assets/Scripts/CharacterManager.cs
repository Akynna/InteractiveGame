using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterManager : MonoBehaviour {


	public Character currentCharacter;
	public SpriteRenderer characterSprite;
	public HashSet<Character> characterList;

	public DialoguesTable dialoguesTable;

	public int isFeedBack;
	public string currentSpriteName;

	// Use this for initialization
	void Start () {

		currentCharacter = new Character("Unknownnn", 0, Character.RelationState.Unknown);
		isFeedBack = 0;

		// Get characters' names
		HashSet<string> characterNames = dialoguesTable.getCharacterNames();

		// Initialize the score and state of all characters
		characterList = new HashSet<Character>();

		foreach(string characterName in characterNames) {
			Character character = new Character(characterName, 0, Character.RelationState.Unknown);
			characterList.Add(character);
		}
		
	}

	public Character getCharacterByName(string characterName)
	{
		foreach(Character character in characterList)
		{
			if (String.Equals(character.name, characterName))
			{
				// Debug.Log(characterName);
				return new Character(character.name, character.score, character.relationState);
			}
		}	
		Debug.Log("No such character found.");
		return null;
	}

	public void updateCharacterSprite()
	{
		characterSprite.sprite = Resources.Load<Sprite>("Characters/" + currentSpriteName);
	}

	// Choose randomly whether or not the character should give a feedback
	public void randomFeedback(int answerType) {

		// Generate a random number between 0 and 1
		System.Random r = new System.Random();
		int	giveFeedback = 1; // r.Next(0, 2);

		if(giveFeedback == 1)
		{
			switch(answerType) 
			{
				case 1:
					currentSpriteName += " Good";
					isFeedBack = 1;
					break;
				case 2:
					currentSpriteName += " Bad";
					isFeedBack = 1;
					break;
				default:
					break;
			}
		} else {
			isFeedBack = 0;
		}
	}
}
