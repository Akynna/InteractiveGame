using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*
 *	This Manager handle everything that is related to the Character with
 * 	which the user is interacting. This class can be adapted to keep track
 * 	of the relation state the user can have with a character.
 * 
 */
public class CharacterManager : MonoBehaviour {

	// Managers with whom the Character Manager communicates
	public StoryManager StoryManager;

	// Elements that the Manager will keep track of
	public Character currentCharacter;
	public string currentSpriteName;
	public SpriteRenderer characterSprite;
	public static List<Character> characterList;
	public int isFeedBack;
	private int reset = 0;

	public void Initialize() {

		// Initialize the current character
		currentCharacter = new Character("Unknown", 0, 0, Character.RelationState.Unknown);

		// Get characters' names
		HashSet<string> characterNames = StoryManager.GetCharacterNames();

		// Initialize the score and state of all characters
		characterList = new List<Character>();

		foreach(string characterName in characterNames) {
			if(characterName != "Me" && characterName != "NA") {
				Character character = new Character(characterName, 0, 0, Character.RelationState.Unknown);
				characterList.Add(character);
			}
		}
	}

	public Character GetCharacterByName(string characterName)
	{
		foreach(Character character in characterList)
		{
			if (String.Equals(character.name, characterName))
			{
				return new Character(character.name, character.empathyScore, character.taskScore, character.relationState);
			}
		}

		if(characterName != "Me") {
			Debug.Log("No such character found.");
		}
		
		return null;
	}

	public void UpdateCharacter(string characterName, int newEmpathyScore, int newTaskScore, Character.RelationState newRelationState)
	{
		bool updated = false;
		
		for(int i = 0; i < characterList.Count; ++i)
		{
			if (String.Equals(characterList[i].name, characterName))
			{
				Character character = characterList[i];

				character.empathyScore = newEmpathyScore;
				character.taskScore = newTaskScore;
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
			Debug.Log("Task Score : " + character.taskScore);
		}
	}

	public void UpdateCharacterSprite()
	{
		characterSprite.sprite = Resources.Load<Sprite>("Characters/" + currentSpriteName);
	}

	public void UpdateCharacterSprite(string characterSpriteName)
	{
		/*if(isFeedBack == 0) {
			if(characterName != "Me" && (currentCharacter.name != characterName || currentSpriteName != characterName))
			{
				currentCharacter = getCharacterByName(characterName);
				currentSpriteName = characterName;
			} 
		} else
		{
			isFeedBack = 0;
		}

		if(reset != 1 || characterName != "Me") {
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + currentSpriteName);
		} else {
			reset = 0;
		}*/

		// Debug.Log(characterSpriteName);
		characterSprite.sprite = Resources.Load<Sprite>("Characters/" + characterSpriteName);
	}

	public void ResetCharacterSprite() {
		reset = 1;
		characterSprite.sprite = null;
	}

	// Choose randomly whether or not the character should give a feedback
	public void RandomFeedback(int answerType) {

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
