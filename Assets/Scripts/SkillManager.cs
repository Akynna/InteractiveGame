using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

/*
 * The Skill Manager is the instance that keeps track of
 * the progress the player is making during the game.
 * It handles different skills and their states and 
 * chooses which skill has to be evaluated depending
 * on the performance of the user, using two different
 * methods :
 * - The first method is an algorithm designed by a
 * member of the CHILI Laboratory
 * - The second method is the Bayesian Knowledge Tracing
 * 
 */
public class SkillManager : MonoBehaviour {

	// Managers with whom the Skill Manager communicates
	public StoryManager StoryManager;

	// List of the main Skills
	// We will especially keep track of their subskills
	public static List<Skill> skillsList = new List<Skill>();

	// ========================================================
	//          FIRST METHOD: Thibault's Algorithm
	// ========================================================

	// Used to keep track of Skills' probabilities
	public List<KeyValuePair<string, double>> skillsProbs =
	new List<KeyValuePair<string, double>>();

	// Score goal a skill has to reach to move to the next level
	public int mainScoreGoal = 1;


	// ========================================================
	//   SECOND METHOD: Bayesian Knowledge Tracing Algorithm
	// ========================================================

	// ============== Parameters of BKT ==============
	// p(L0) = The probability of the player knowing the skill beforehand
	private float p_init = 0.1f;
	// p(T) = The probability of the player demonstrating knowledge of
	// the skill after an opportunity to apply it
	private float p_transit = 0.6f;
	// p(S) = The probability the player makes a mistake when
	// applying a known skill
	private float p_slip = 0.2f;
	// p(G) = The probability that the player correctly applies an
	// unknown skill (has a lucky guess)
	private float p_guess = 0.15f;

	// The conditional probabilities
	// It will be used to update the probability of each skill mastery
	private List<float> cond_probs;

	// We will use Empirical Probabilities to estimate and update the 4 parameters
	// of BKT. We will keep track of each answer the player gives.
	private List<KeyValuePair<string, List<int>>> performance_data =
	new List<KeyValuePair<string, List<int>>>();


	public void Initialize() {

		// Get the skills' names
		HashSet<string> mainSkillNames = StoryManager.GetMainSkillsNames();
		cond_probs = new List<float>();
		
		// Iterate over each skill and instanciate it
		foreach(string skillName in mainSkillNames) {
			if(skillName != "NA") {

				// Get the set of subskills / situations associated to that main skill
				HashSet<string> subSkillsNames = StoryManager.GetSubSkillsNames(skillName);

				// Instanciate the main skill with the subskills list and an initial weight of 1
				Skill skill = new Skill(skillName, 1.0f, subSkillsNames);
				skillsList.Add(skill);

				// At the beginning, all the Skills have the same probability to be picked
				skillsProbs.Add(new KeyValuePair<string, double>(skillName, 1.0f / mainSkillNames.Count));

				// Initialize the performance data with an empty list for each skill
				performance_data.Add(new KeyValuePair<string, List<int>>(skillName, new List<int>()));

				// Initialize the conditional probability of the skill to p_init
				cond_probs.Add(p_init);
			}
		}

		displaySkillsProbsAndWeights();
	}


	// ========================================================
	//          FIRST METHOD: Thibault's Algorithm
	// ========================================================

	// This is the first method to evaluate and update the skills
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
	
	// ========================================================
	//   SECOND METHOD: Bayesian Knowledge Tracing Algorithm
	// ========================================================
	private List<List<int>> CreateKnowledgeSequences(int size) {

		List<List<int>> possibleSequences = new List<List<int>>();

		// Initialize the list of lists
		for(int i=0; i <= size; ++i) {
			possibleSequences.Add(new List<int>());
		}

		// Generate the possible Knowledge Sequences
		for(int i=0; i <= size; ++i) {
			for(int j=0; j < size-i; ++j) {
				possibleSequences[i].Add(0);
			}
			for(int j = size-i; j < size; ++j) {
				possibleSequences[i].Add(1);
			}
		}

		return possibleSequences;
	}


	public void BKT(string skillName, string subskillName, int score) {

		// Find the index of the corresponding Skill with the skillName
		var skillIndex = skillsList.FindIndex(skill => skill.name == skillName);

		if(score > 0) {

			// Update the performance data of the player
			// We put a 1 for a CORRECT answer
			performance_data[skillIndex].Value.Add(1);

			// Update the 4 parameters of our algorithm
			UpdateParameters(skillIndex);

			// Apply the formulas of BKT for the conditional probability
			// for a CORRECT application
			cond_probs[skillIndex] = cond_probs[skillIndex] * (1.0f - p_slip) 
			/ (cond_probs[skillIndex] * (1.0f - p_slip) + (1.0f - cond_probs[skillIndex]) * p_guess);
			//Debug.Log("Positive cond prob: " + cond_prob);
		}
		// Otherwise, we consider that the player has given a BAD answer
		else if (score < 0) {
			// Update the performance data of the player
			// We put a 0 for an INCORRECT answer
			performance_data[skillIndex].Value.Add(0);

			// Update the 4 parameters of our algorithm
			UpdateParameters(skillIndex);

			// Apply the formulas of BKT for the conditional probability
			// for an INCORRECT application
			cond_probs[skillIndex] = cond_probs[skillIndex] * p_slip 
			/ (cond_probs[skillIndex] * p_slip + (1.0f - cond_probs[skillIndex]) * (1.0f - p_guess));
			//Debug.Log("Negative cond prob: " + cond_prob);
		}

		// Then we compute the probability of mastering the current skill
		float p_skill = cond_probs[skillIndex] + (1.0f - cond_probs[skillIndex]) * p_transit;
		//Debug.Log("Probability of mastering the skill: " + p_skill);

		// The more the probability is close to 1, the more the player has mastered it
		// Update the importance weight to prioritize skills that are the less mastered
		skillsList[skillIndex].importanceWeight = 1.0f - p_skill;

		// Then we update the list of subskills of the current skill
		// (we remove the subskill that just was evaluated)
		skillsList[skillIndex].subskills.Remove(subskillName);

		// Finally, we update the probabilities of all the skills,
		// since the weighting has changed after making a choice
		UpdateProbabilities();
	}

	private void UpdateParameters(int skillIndex) {

		int nbAnswers = performance_data[skillIndex].Value.Count;
		List<int> givenAnswers = performance_data[skillIndex].Value;

		// Generate new possible knowledge sequences
		List<List<int>> knowledge_sequences = CreateKnowledgeSequences(nbAnswers);
		List<KeyValuePair<List<int>, float>> accuracies = new List<KeyValuePair<List<int>, float>>();

		// Compare the student data with all the knowledge sequences
		foreach(List<int> sequence in knowledge_sequences) {

			float accuracy = 0.0f;

			for(int i=0; i < nbAnswers; ++i) {
				if(sequence[i] == givenAnswers[i]) {
					accuracy += 1.0f;
				}
			}

			accuracy /= nbAnswers;

			// Store the computed accuracy for each sequence
			accuracies.Add(new KeyValuePair<List<int>, float>(sequence, accuracy));
		}

		// Find the sequence that has the highest accuracy
		// to build the Knowledge Sequence
		accuracies.Sort((x, y) => x.Value.CompareTo(y.Value));
		KeyValuePair<List<int>, float> maxAccuracy = accuracies.First();
		List<float> knowledgeSequence = new List<float>();
		
		// It is possible that there are multiple max values
		// Hence, we will check the next sequences to see if their
		// accuracy is equal to the first sequence
		if(maxAccuracy.Value == accuracies[1].Value) {
			List<KeyValuePair<List<int>, float>> maxAccuracies = new List<KeyValuePair<List<int>, float>>();
			maxAccuracies.Add(maxAccuracy);

			foreach(KeyValuePair<List<int>, float> sequence in accuracies) {
				if(maxAccuracy.Value == sequence.Value) {
					maxAccuracies.Add(sequence);
				}
			}

			float sumAccuracies = 0.0f;

			// Compute the average of all the sequences with max accuracy
			for(int i=0; i < nbAnswers; ++i) {
				foreach(KeyValuePair<List<int>, float> sequence in maxAccuracies) {
					sumAccuracies += sequence.Key[i];
				}
				knowledgeSequence.Add(sumAccuracies / maxAccuracies.Count);
			}
		} else {
			knowledgeSequence = maxAccuracy.Key.ConvertAll(x => (float) x);
		}

		// Get the correctness sequence, which is the answers from the performance data

		// Now that we have the best sequence, we can now update our parameters following
		// Empirical Probabilities
		p_init = knowledgeSequence.Average();

		Debug.Log("Init probability: " + p_init);

		p_transit = UpdateTransitProbability(knowledgeSequence, nbAnswers);
		p_guess = UpdateGuessProbability(knowledgeSequence, givenAnswers, nbAnswers);
		p_slip = UpdateSlipProbability(knowledgeSequence, givenAnswers, nbAnswers);	
	}

	private float UpdateTransitProbability(List<float> sequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=1; i < size; ++i) {
			upperSum += (1.0f - sequence[i-1]) * sequence[i];
			lowerSum += (1.0f - sequence[i-1]);
		}

		//Debug.Log("Transit probability: " + upperSum / lowerSum);

		return upperSum / lowerSum;
	}

	private float UpdateGuessProbability(List<float> knowledgeSequence, List<int> correctnessSequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=0; i < size; ++i) {
			upperSum += correctnessSequence[i] * (1.0f - knowledgeSequence[i]);
			lowerSum += (1.0f - knowledgeSequence[i]);
		}

		Debug.Log("Guess probability: " + upperSum / lowerSum);

		return upperSum / lowerSum;
	}

	private float UpdateSlipProbability(List<float> knowledgeSequence, List<int> correctnessSequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=0; i < size; ++i) {
			upperSum += (1.0f - correctnessSequence[i]) * knowledgeSequence[i];
			lowerSum += knowledgeSequence[i];
		}

		Debug.Log("Slip probability: " + upperSum / lowerSum);

		return upperSum / lowerSum;
	}

	private void UpdateProbabilities() {

		// Clear the list of skills' probabilities
		skillsProbs.Clear();

		// Get the new total sum of weights
		float sumWeights = skillsList.Sum(skill => skill.importanceWeight);

		// Debug.Log("Sum weights: " + sumWeights);

		// Build a new list of skills' probabilities
		foreach(Skill skill in skillsList) {
			skillsProbs.Add(new KeyValuePair<string, double>(skill.name, skill.importanceWeight / sumWeights));
		}

		// Sort the probabilities
		skillsProbs.Sort((x, y) => x.Value.CompareTo(y.Value));

		displaySkillsProbsAndWeights();
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

		// Check if the skill exists
		if(skillIndex == -1) {
			Debug.Log("Skill not found. You might have put an invalid skill name.");
		}

		Skill chosenSkill = skillsList[skillIndex];
		List<string> subskills = chosenSkill.subskills.ToList();

		// Choose randomly a subskill among the subskills
		// The probability to pick a specific subskill is completely homogeneous
		System.Random r = new System.Random();
		int chosenIndex = r.Next(0, subskills.Count);
		chosenSubskill = subskills[chosenIndex];

		// Remove the chosen subskill to the list of the Skill
		// so it won't be chosen again
		skillsList[skillIndex].subskills.Remove(chosenSubskill);

		return chosenSubskill;
	}

	// Used for debugging to display the skills' weights and probabilities
	private void displaySkillsProbsAndWeights() {

		foreach(KeyValuePair<string, double> pair in skillsProbs) {
			// Find the index of the corresponding Skill with the skillName
			var skillIndex = skillsList.FindIndex(s => s.name == pair.Key);
			Skill skill = skillsList[skillIndex];

			Debug.Log("Skill: " + pair.Key + ", Probability: " + pair.Value + ", Weight: " + skill.importanceWeight);
		}
	}


}
