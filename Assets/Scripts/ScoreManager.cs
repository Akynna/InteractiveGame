using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScoreManager : MonoBehaviour {

	public HashSet<Character> characterList;
	
	public Character currentCharacter;

	// ELements that the ScoreManager will modify
	public Text scoreText;
	public Animator scoreAnimator;
	public GameObject relationBar;

	void Start () {

		// Initlialize the relation states
		Text[] stateTexts = relationBar.GetComponentsInChildren<Text>();

		Text currentState = stateTexts[1];
		Text nextState = stateTexts[2];

		currentState.text = Character.RelationState.Unknown.ToString();
		nextState.text = Character.RelationState.Acquaintance.ToString();

		// Initialize the score bars (arrows)
		Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();

		foreach (Image bar in scoreBars)
		{
			var tempColor = bar.color;
			tempColor.a = 0.25f;
			bar.color = tempColor;
		}
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
