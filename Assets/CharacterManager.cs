using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterManager : MonoBehaviour {


	public Character currentCharacter;
	public HashSet<Character> characterList;

	public DialoguesTable dialoguesTable;

	// Use this for initialization
	void Start () {

		currentCharacter = new Character("Unknownnn", 0, Character.RelationState.Unknown);

		// Get characters' names
		HashSet<string> characterNames = dialoguesTable.getCharacterNames();

		// Initialize the score and state of all characters
		characterList = new HashSet<Character>();

		foreach(string characterName in characterNames) {
			Character character = new Character(characterName, 0, Character.RelationState.Unknown);
			characterList.Add(character);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
