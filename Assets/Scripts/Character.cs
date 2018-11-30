using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character {

    public enum RelationState
    {
        Unknown,
        Acquaintance,
        Friend,
        Mentor,
        GoodFriend,
        BestFriend
    }
    public string name;

    public int score;
    public RelationState relationState;
    public SpriteRenderer spriteRenderer;

    public Character(string name, int score, RelationState relationState)
    {
        this.name = name;
        this.score = score;
        this.relationState = relationState;
    }

	// Use this for initialization
	void Start () {

		// Initialize the list of all the characters
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    // Change the character sprite
    public void switchCharacter(string imageName)
	{
	    spriteRenderer.sprite = Resources.Load<Sprite>("Characters/" + imageName);
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
