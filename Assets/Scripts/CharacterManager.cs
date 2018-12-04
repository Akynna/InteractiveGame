using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterManager : MonoBehaviour {

	public Character currentCharacter;
	public SpriteRenderer characterSprite;
	public static List<Character> characterList;

	public DialoguesTable dialoguesTable;

	public int isFeedBack;
	public string currentSpriteName;

	// Use this for initialization
	void Start () {

		currentCharacter = new Character("Unknown", 0, 0, Character.RelationState.Unknown);
		isFeedBack = 0;

		// Get characters' names
		HashSet<string> characterNames = dialoguesTable.getCharacterNames();

		// Initialize the score and state of all characters
		characterList = new List<Character>();

		foreach(string characterName in characterNames) {
			if(characterName != "Me" && characterName != "NA") {
				Character character = new Character(characterName, 0, 0, Character.RelationState.Unknown);
				characterList.Add(character);
			}
		}
		
	}

	public Character getCharacterByName(string characterName)
	{
		foreach(Character character in characterList)
		{
			if (String.Equals(character.name, characterName))
			{
				return new Character(character.name, character.empathyScore, character.skillScore, character.relationState);
			}
		}	
		Debug.Log("No such character found.");
		return null;
	}

	public void updateCharacter(string characterName, int newEmpathyScore, int newSkillScore, Character.RelationState newRelationState)
	{
		bool updated = false;
		for(int i = 0; i < characterList.Count; ++i)
		{
			if (String.Equals(characterList[i].name, characterName))
			{
				Character character = characterList[i];

				character.empathyScore = newEmpathyScore;
				character.skillScore = newSkillScore;
				character.relationState = newRelationState;

				characterList[i] = character;
				updated = true;
			}
		}
		if(!updated) {
			Debug.Log("No such character found. Update failed.");
		}
	}

	public void displaycharactersList() {
		foreach(Character character in characterList) {
			Debug.Log("Character name : " + character.name);
			Debug.Log("Empathy Score : " + character.empathyScore);
			Debug.Log("Skill Score : " + character.skillScore);
		}
	}

	public void updateCharacterSprite()
	{
		characterSprite.sprite = Resources.Load<Sprite>("Characters/" + currentSpriteName);
	}

	public void updateCharacterSprite(string spriteName)
	{
		if(spriteName != currentSpriteName) {
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + spriteName);
		}
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
