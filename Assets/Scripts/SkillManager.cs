using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SkillManager : MonoBehaviour {

	// Managers with whom the Skill Manager communicates
	public StoryManager StoryManager;

	// List of the main Skills
	// We will especially keep track of their subskills
	public static List<Skill> skillsList = new List<Skill>();

	// Used to keep track of Skills' probabilities
	public List<KeyValuePair<string, double>> skillsProbs = new List<KeyValuePair<string, double>>();

	// Score goal a skill has to reach to move to the next level
	public int mainScoreGoal = 1;

	public void Initialize() {

		// Get the skills' names and their count
		HashSet<string> mainSkillNames = StoryManager.GetMainSkillsNames();
		
		// Iterate over each skill and instanciate it
		foreach(string skillName in mainSkillNames) {
			if(skillName != "NA") {

				// Get the set of subskills associated to that main skill
				HashSet<string> subSkillsNames = StoryManager.GetSubSkillsNames(skillName);

				// Instanciate the main skill with the subskills list and an initial weight of 1
				Skill skill = new Skill(skillName, 1.0f, subSkillsNames);
				skillsList.Add(skill);

				// At the beginning, all the Skills have the same probability to be picked
				skillsProbs.Add(new KeyValuePair<string, double>(skillName, 1.0f / mainSkillNames.Count));
			}
		}

		displaySkillsProbsAndWeights();
	}

	private void UpdateProbabilities() {

		// Clear the list of skills' probabilities
		skillsProbs.Clear();

		// Get the new total sum of weights
		float sumWeights = skillsList.Sum(skill => skill.importanceWeight);

		Debug.Log("Sum weights: " + sumWeights);

		// Build a new list of skills' probabilities
		foreach(Skill skill in skillsList) {
			skillsProbs.Add(new KeyValuePair<string, double>(skill.name, skill.importanceWeight / sumWeights));
		}

		// Sort the probabilities
		skillsProbs.Sort((x, y) => x.Value.CompareTo(y.Value));

		displaySkillsProbsAndWeights();
	}

	public void UpdateSkill(string skillName, string subskillName, int score) {

		// Find the index of the corresponding Skill with the skillName
		var skillIndex = skillsList.FindIndex(skill => skill.name == skillName);

		// If the user had a good score with this skill, we divide its weight by 2
		if(score > 0) {
			skillsList[skillIndex].importanceWeight /= 2.0f;
		}
		// Otherwise, we multiply it by 2
		else if (score < 0) {
			skillsList[skillIndex].importanceWeight *= 2.0f;
		}

		// Then we update the list of subskills of the current skill
		// (we remove the subskill that just was evaluated)
		skillsList[skillIndex].subskills.Remove(subskillName);

		// Finally, we update the probabilities of all the skills,
		// since the weighting has changed after making a choice
		UpdateProbabilities();
	}

	// Randomly choose a Skill to evaluate depending on all Skills' probabilities
	public string ChooseSkill() {

		string selectedSkill = "";

		System.Random r = new System.Random();
		double diceRoll = r.NextDouble();

		// Debug.Log(diceRoll);

		double cumulative = 0.0;
		for (int i = 0; i < skillsProbs.Count; i++)
		{
			cumulative += skillsProbs[i].Value;
			if (diceRoll < cumulative)
			{
				selectedSkill = skillsProbs[i].Key;
				break;
			}
		}

		Debug.Log("We have chosen to evaluate: " + selectedSkill);

		return selectedSkill;
	}


	// Randomly pick a subskill of a certain Skill
	public string ChooseSubskill(string skillName) {

		string chosenSubskill = "";

		// Find the index of the corresponding Skill with the skillName
		var skillIndex = skillsList.FindIndex(skill => skill.name == skillName);

		if(skillIndex == -1) {
			Debug.Log("Skill not found. You might have put an invalid skill name.");
		}

		Skill chosenSkill = skillsList[skillIndex];
		List<string> subskills = chosenSkill.subskills.ToList();

		// Choose randomly a subskill among the subkills
		System.Random r = new System.Random();
		int chosenIndex = r.Next(0, subskills.Count);
		chosenSubskill = subskills[chosenIndex];

		// Remove the chosen subskill to the list of the Skill
		// so it won't be chosen again
		skillsList[skillIndex].subskills.Remove(chosenSubskill);

		return chosenSubskill;
	}

	private void displaySkillsProbsAndWeights() {

		foreach(KeyValuePair<string, double> pair in skillsProbs) {
			// Find the index of the corresponding Skill with the skillName
			var skillIndex = skillsList.FindIndex(s => s.name == pair.Key);
			Skill skill = skillsList[skillIndex];

			Debug.Log("Skill: " + pair.Key + ", Probability: " + pair.Value + ", Weight: " + skill.importanceWeight);
		}
	}


}
