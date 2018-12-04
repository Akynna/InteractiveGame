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
		// Text[] stateTexts = relationBar.GetComponentsInChildren<Text>();

		// Text currentState = stateTexts[1];
		// Text nextState = stateTexts[2];

		// currentState.text = Character.RelationState.Unknown.ToString();
		// nextState.text = Character.RelationState.Acquaintance.ToString();

		// Initialize the score bars (arrows)
		/*Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();

		foreach (Image bar in scoreBars)
		{
			var tempColor = bar.color;
			tempColor.a = 0.25f;
			bar.color = tempColor;
		}*/
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

	public void updatePoints(int empathyScore, int skillScore)
	{
		int oldEmpathyScore = characterManager.currentCharacter.empathyScore;
		int newEmpathyScore = characterManager.currentCharacter.empathyScore + empathyScore;

		int oldSkillScore = characterManager.currentCharacter.skillScore;
		int newSkillScore = characterManager.currentCharacter.skillScore + skillScore;

		int finalEmpathyScore = oldEmpathyScore;
		int finalSkillScore = oldSkillScore;

		// Initialize the score bars (arrows)
		//Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();
		//var tempColor = scoreBars[0].color;



		if (oldEmpathyScore < newEmpathyScore) {
			upScore(empathyScore, 1);
			/*tempColor.a = 0.75f;

			for (int i=oldScore; i < newScore && i < scoreBars.Length; ++i) {
				scoreBars[i].color = tempColor;
			}*/

		} else if (newEmpathyScore < oldEmpathyScore) {
			downScore(empathyScore, 1);
			/*tempColor.a = 0.25f;

			for (int i=oldScore; i > newScore && i > 0; --i) {
				scoreBars[i].color = tempColor;
			}*/
		} else {
			// resetBar();

			/*tempColor.a = 0.75f;
			for(int i=0; i < oldScore; ++i) {
				scoreBars[i].color = tempColor;
			}*/
		}

		// yield return new WaitForSeconds(5);

		if(oldSkillScore < newSkillScore) {
			upScore(skillScore, 2);
		} else if(oldSkillScore > newSkillScore) {
			downScore(skillScore, 2);
		}

		finalEmpathyScore += empathyScore;
		finalSkillScore += skillScore;

		// Update the character score
		characterManager.updateCharacter(characterManager.currentCharacter.name, finalEmpathyScore, finalSkillScore, characterManager.currentCharacter.relationState);

		Character charTest = characterManager.getCharacterByName(characterManager.currentCharacter.name);		
	}

	public void upScore(int points, int scoreType) {
		switch(scoreType) {
			case 1:
				scoreText.text = "Empathy Score : +" + points.ToString();
				break;
			case 2:
				scoreText.text = "Skill Score : +" + points.ToString();
				break;
			default:
				break;
		}
		scoreAnimator.SetTrigger("Up");
	}

	public void downScore(int points, int scoreType)
	{
				switch(scoreType) {
			case 1:
				scoreText.text = "Empathy Score : " + points.ToString();
				break;
			case 2:
				scoreText.text = "Skill Score : " + points.ToString();
				break;
			default:
				break;
		}
		scoreAnimator.SetTrigger("Down");
	}
}
