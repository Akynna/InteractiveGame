using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillManager : MonoBehaviour {

	public DialoguesTable dialoguesTable;

	public static List<Skill> skillsList;
	public int mainScoreGoal = 100;
	


	public float[] probabilities = new float[3];

    void Start () {

		// Get skills' names
		HashSet<string> mainSkillNames = dialoguesTable.getMainSkillsNames();

		// Initialize the list of different subskills
		skillsList = new List<Skill>();
		
		foreach(string skillName in mainSkillNames) {
			if(skillName != "NA") {
				Skill skill = new Skill(skillName, Skill.Mastery.Beginner);
				skillsList.Add(skill);
			}
		}
	}

	//	public void




}
