using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScoreManager : MonoBehaviour {

	// Managers with whom the Score Manager communicates
	public StoryManager StoryManager;
	public CharacterManager CharacterManager;

	public HashSet<Character> characterList;
	
	

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

	public void ChangeCurrentState(Character.RelationState newState)
	{
		Text[] stateTexts = relationBar.GetComponentsInChildren<Text>();
		stateTexts[1].text = newState.ToString();
	}

	private void ResetBar() {
		Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();
		var tempColor = scoreBars[0].color;
		tempColor.a = 0.25f;

		for(int i=0; i < scoreBars.Length; ++i) {
			scoreBars[i].color = tempColor;
		}
	}

	public void UpdatePoints(int empathyScore, int skillScore)
	{
		int oldEmpathyScore = CharacterManager.currentCharacter.empathyScore;
		int newEmpathyScore = CharacterManager.currentCharacter.empathyScore + empathyScore;

		int oldSkillScore = CharacterManager.currentCharacter.skillScore;
		int newSkillScore = CharacterManager.currentCharacter.skillScore + skillScore;

		int finalEmpathyScore = oldEmpathyScore;
		int finalSkillScore = oldSkillScore;

		// Initialize the score bars (arrows)
		//Image[] scoreBars = GameObject.Find("PointsBar").GetComponentsInChildren<Image>();
		//var tempColor = scoreBars[0].color;



		if (oldEmpathyScore < newEmpathyScore) {
			//StopAllCoroutines();
			StartCoroutine(UpScore(empathyScore, 1));
			/*tempColor.a = 0.75f;

			for (int i=oldScore; i < newScore && i < scoreBars.Length; ++i) {
				scoreBars[i].color = tempColor;
			}*/

		} else if (newEmpathyScore < oldEmpathyScore) {
			//StopAllCoroutines();
			StartCoroutine(DownScore(empathyScore, 1));
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
			StartCoroutine(UpScore(skillScore, 2));
		} else if(oldSkillScore > newSkillScore) {
			//StopAllCoroutines();
			StartCoroutine(DownScore(skillScore, 2));
		}

		finalEmpathyScore += empathyScore;
		finalSkillScore += skillScore;

		// Update the character score
		CharacterManager.UpdateCharacter(CharacterManager.currentCharacter.name, finalEmpathyScore, finalSkillScore, CharacterManager.currentCharacter.relationState);	
	}

	IEnumerator UpScore(int points, int scoreType) {
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

	IEnumerator DownScore(int points, int scoreType)
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
