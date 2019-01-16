using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScoreManager : MonoBehaviour {

	public HashSet<Character> characterList;
	
	public CharacterManager characterManager;

	public Text empathyScoreText;
	public Animator empathyScoreAnimator;

	public Text skillScoreText;
	public Animator skillScoreAnimator;

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
			//StopAllCoroutines();
			StartCoroutine(upScore(empathyScore, 1));
			/*tempColor.a = 0.75f;

			for (int i=oldScore; i < newScore && i < scoreBars.Length; ++i) {
				scoreBars[i].color = tempColor;
			}*/

		} else if (newEmpathyScore < oldEmpathyScore) {
			//StopAllCoroutines();
			StartCoroutine(downScore(empathyScore, 1));
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
			//StopAllCoroutines();
			StartCoroutine(upScore(skillScore, 2));
		} else if(oldSkillScore > newSkillScore) {
			//StopAllCoroutines();
			StartCoroutine(downScore(skillScore, 2));
		}

		finalEmpathyScore += empathyScore;
		finalSkillScore += skillScore;

		// Update the character score
		characterManager.updateCharacter(characterManager.currentCharacter.name, finalEmpathyScore, finalSkillScore, characterManager.currentCharacter.relationState);	
	}

	IEnumerator upScore(int points, int scoreType) {
		switch(scoreType) {
			case 1:
				empathyScoreText.text = "Empathy Score : +" + points.ToString();
				empathyScoreAnimator.SetTrigger("Up");
				break;
			case 2:
				yield return new WaitForSeconds(1);
				skillScoreText.text = "Skill Score : +" + points.ToString();
				skillScoreAnimator.SetTrigger("Up");
				break;
			default:
				break;
		}
		yield return null;
	}

	IEnumerator downScore(int points, int scoreType)
	{
		switch(scoreType) {
			case 1:
				empathyScoreText.text = "Empathy Score : " + points.ToString();
				empathyScoreAnimator.SetTrigger("Down");
				break;
			case 2:
				yield return new WaitForSeconds(1);
				skillScoreText.text = "Skill Score : " + points.ToString();
				skillScoreAnimator.SetTrigger("Down");
				break;
			default:
				break;
		}
		yield return null;
	}
}
