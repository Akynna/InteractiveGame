using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillManager : MonoBehaviour {

	// Managers with whom the Skill Manager communicates
	public StoryManager StoryManager;

	public static List<Skill> mainSkillsList;
	public int mainScoreGoal = 100;
	
	public float[] probabilities;

	public void Initialize() {

		// Set the probabilities (can be changed)
		probabilities = new float[] {0.9f, 0.3f, 0.05f};

		IDictionary<string, float> masteries = new Dictionary<string, float>();
		
		// Iterate over each mastery level and assign it a probability
		int i = 0;
		foreach(string mastery in Skill.Mastery.GetValues(typeof(Skill.Mastery))) {
			masteries.Add(mastery, probabilities[i]);
			i++;
		}

		// Get the MAIN skills' names
		HashSet<string> mainSkillNames = StoryManager.GetMainSkillsNames();

		mainSkillsList = new List<Skill>();
		
		// Iterate over each main skill and instanciate it
		foreach(string skillName in mainSkillNames) {
			if(skillName != "NA") {

				// Get the set of subskills associated to that main skill
				HashSet<string> subSkillsNames = StoryManager.GetSubSkillsNames(skillName);

				// Instanciate each subskills and store them in a list
				List<Skill> subSkills = new List<Skill>();

				foreach(string subSkillName in subSkillsNames) {
					Skill subSkill = new Skill(subSkillName, Skill.Mastery.Beginner);
					subSkills.Add(subSkill);
				}

				// Instanciate the main skill with the subskills list
				Skill skill = new Skill(skillName, Skill.Mastery.Beginner, subSkills);

				mainSkillsList.Add(skill);
			}
		}
	}



}
