using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScoreManager : MonoBehaviour {

	public HashSet<Character> characterList;
	
	public CharacterManager characterManager;

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

	private void resetBar() {
		Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();
		var tempColor = scoreBars[0].color;
		tempColor.a = 0.25f;

		for(int i=0; i < scoreBars.Length; ++i) {
			scoreBars[i].color = tempColor;
		}
	}

	public void updatePoints(int points)
	{
		int oldScore = characterManager.currentCharacter.score;
		int newScore = characterManager.currentCharacter.score + points;

		// Initialize the score bars (arrows)
		Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();
		var tempColor = scoreBars[0].color;

		if (oldScore < newScore) {
			upScore(newScore - oldScore);
			tempColor.a = 0.75f;

			for (int i=oldScore; i < newScore && i < scoreBars.Length; ++i) {
				scoreBars[i].color = tempColor;
			}

		} else if (newScore < oldScore) {
			downScore(oldScore - newScore);
			tempColor.a = 0.25f;

			for (int i=oldScore; i > newScore && i > 0; --i) {
				scoreBars[i].color = tempColor;
			}
		} else {
			resetBar();
			
			tempColor.a = 0.75f;
			for(int i=0; i < oldScore; ++i) {
				scoreBars[i].color = tempColor;
			}
		}
		characterManager.currentCharacter.score += points;
	}

	public void upScore(int points) {
		scoreAnimator.SetTrigger("Up");
		scoreText.text = "Score : +" + points.ToString();
	}

	public void downScore(int points)
	{
		scoreAnimator.SetTrigger("Down");
		scoreText.text = "Score : -" + points.ToString();
	}

	private void changeScoreBar(int changeType, int nbBars)
	{
			
	}

}
