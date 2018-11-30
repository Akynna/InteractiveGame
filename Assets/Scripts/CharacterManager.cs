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

	public void switchCharacter(string imageName)
	{
		if(imageName != "Me")
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + imageName);
	}

	// Change the expression of a character
    public void changeCharacterFace(int faceType) {

        string imageName = name;

		switch(faceType) 
		{
			case 1:
				// Debug.Log("good");
				imageName += " Good";
				break;
			case 2:
				//Debug.Log("bad.");
                imageName += " Bad";
				break;
			default:
				//Debug.Log("neutral");
				break;
		}
        switchCharacter(imageName);
	}
}
