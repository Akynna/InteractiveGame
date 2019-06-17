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
	private float p_init = 0.5f;
	// p(T) = The probability of the player demonstrating knowledge of
	// the skill after an opportunity to apply it
	private float p_transit = 0.3f;
	// p(S) = The probability the player makes a mistake when
	// applying a known skill
	private float p_slip = 0.3f;
	// p(G) = The probability that the player correctly applies an
	// unknown skill (has a lucky guess)
	private float p_guess = 0.3f;

	//private List<float> initial_parameters = new List

	// We keep track of a different list of parameters for each Skill
	private List<KeyValuePair<string, List<float>>> parameters =
	new List<KeyValuePair<string, List<float>>>();

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
				Skill skill = new Skill(skillName, 1.0f / (mainSkillNames.Count + 1), subSkillsNames);
				skillsList.Add(skill);

				// At the beginning, all the Skills have the same probability to be picked
				skillsProbs.Add(new KeyValuePair<string, double>(skillName, 1.0f / mainSkillNames.Count));

				// Initialize the performance data with an empty list for each skill
				performance_data.Add(new KeyValuePair<string, List<int>>(skillName, new List<int>()));

				// Initialize the conditional probability of the skill to p_init
				cond_probs.Add(p_init);

				// Initialize the parameters' list for each Skill
				parameters.Add(new KeyValuePair<string, List<float>>(skillName, new List<float>{p_init, p_transit, p_slip, p_guess}));
			}
		}

		//displaySkillsProbsAndWeights();
	}

	// Randomly choose a Skill to evaluate depending on all Skills' probabilities
	public string ChooseSkill() {

		bool hasMadeChoice = false;
		string selectedSkill = "";

		do {

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

			// Debug.Log("We have chosen to evaluate: " + selectedSkill);

			var skillIndex = skillsList.FindIndex(skill => skill.name == selectedSkill);

			// Check if the skill exists
			if(skillIndex == -1) {
				Debug.Log("Skill not found. You might have put an invalid skill name.");
			}

			// Get the list of corresponding subskills
			Skill chosenSkill = skillsList[skillIndex];
			List<string> subskills = chosenSkill.subskills.ToList();

			// Check that there are still subskills in this skill
			if (subskills.Count != 0) {
				hasMadeChoice = true;
			} else {
				for (int i = 0; i < skillsProbs.Count; i++) {
					if (skillsProbs[i].Key.Equals(selectedSkill)) {
						// Update the probability of picking that skill to 0, so it won't be chosen
						skillsProbs.RemoveAll(item => item.Key.Equals(selectedSkill));
						skillsProbs.Add(new KeyValuePair<string, double>(selectedSkill, 0.0));
					}
				}
			}

		} while (!hasMadeChoice);

		return selectedSkill;
	}


	// Randomly pick a subskill of a certain Skill
	public string ChooseSubskill(string skillName) {

		string chosenSubskill = "";

		var skillIndex = skillsList.FindIndex(skill => skill.name == skillName);

		// Check if the skill exists
		if(skillIndex == -1) {
			Debug.Log("Skill not found. You might have put an invalid skill name.");
		}

		// Get the list of corresponding subskills
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

	// General function for both methods to update the probability to evaluate skills
	private void UpdateProbabilities() {

		// Clear the list of skills' probabilities
		skillsProbs.Clear();

		// Get the new total sum of weights
		float sumWeights = skillsList.Sum(skill => skill.importanceWeight);

		// Build a new list of skills' probabilities
		foreach(Skill skill in skillsList) {
			skillsProbs.Add(new KeyValuePair<string, double>(skill.name, skill.importanceWeight / sumWeights));
		}

		// Sort the probabilities
		skillsProbs.Sort((x, y) => x.Value.CompareTo(y.Value));

		displaySkillsProbsAndWeights();
	}

	// Used for debugging to display the skills' weights and probabilities
	private void displaySkillsProbsAndWeights() {

		foreach(KeyValuePair<string, double> pair in skillsProbs) {
			
			// Find the index of the corresponding Skill with the skillName
			var skillIndex = skillsList.FindIndex(s => s.name == pair.Key);
			Skill skill = skillsList[skillIndex];

			//Debug.Log("Skill: " + pair.Key + ", Probability: " + pair.Value + ", Weight: " + skill.importanceWeight);
		}
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
		var skillIndex = skillsList.FindIndex(skill => skill.name.Equals(skillName));

		// Get the current parameters of that skill
		float p_init = parameters[skillIndex].Value[0];
		float p_transit = parameters[skillIndex].Value[1];
		float p_slip = parameters[skillIndex].Value[2];
		float p_guess = parameters[skillIndex].Value[3];

		// If the user had a good score with this skill
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

			//Debug.Log("Positive cond prob: " + cond_probs[skillIndex]);
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
			//Debug.Log("Negative cond prob: " + cond_probs[skillIndex]);
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

	// ========================================================
	//  				 PARAMETERS UPDATE
	// ========================================================

	private void UpdateParameters(int skillIndex) {

		// Get the current parameters of that skill
		float p_init = parameters[skillIndex].Value[0];
		float p_transit = parameters[skillIndex].Value[1];
		float p_slip = parameters[skillIndex].Value[2];
		float p_guess = parameters[skillIndex].Value[3];

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

		// Take the last to have the sequence with the MAX accuracy
		KeyValuePair<List<int>, float> maxAccuracy = accuracies[accuracies.Count-1];
		List<float> knowledgeSequence = new List<float>();
		
		// It is possible that there are multiple max values
		// Hence, we will check the next sequences to see if their
		// accuracy is equal to the first sequence
		if(maxAccuracy.Value == accuracies[accuracies.Count-2].Value) {
			List<KeyValuePair<List<int>, float>> maxAccuracies = new List<KeyValuePair<List<int>, float>>();
			maxAccuracies.Add(maxAccuracy);

			foreach(KeyValuePair<List<int>, float> sequence in accuracies) {
				if(maxAccuracy.Value == sequence.Value) {
					maxAccuracies.Add(sequence);
				}
			}

			float sumAccuracies = 0.0f;

			// Compute the average of all the sequences with max accuracy
			foreach(KeyValuePair<List<int>, float> sequence in maxAccuracies) {
				for(int i=0; i < nbAnswers; ++i) {
					sumAccuracies += sequence.Key[i];
				}
				knowledgeSequence.Add(sumAccuracies / maxAccuracies.Count);
			}
		} else {
			knowledgeSequence = maxAccuracy.Key.ConvertAll(x => (float) x);
		}

		// Now that we have the best sequence, we can now update our parameters following
		// Empirical Probabilities
		if (nbAnswers > 1) {
			parameters[skillIndex].Value[0] = /*(knowledgeSequence.Average() != 0.0f && knowledgeSequence.Average() != 1.0f) ? knowledgeSequence.Average() :*/ p_init;
			parameters[skillIndex].Value[1] = UpdateTransitProbability(p_transit, knowledgeSequence, nbAnswers);
			parameters[skillIndex].Value[2] = UpdateGuessProbability(p_guess, knowledgeSequence, givenAnswers, nbAnswers);
			parameters[skillIndex].Value[3] = UpdateSlipProbability(p_slip, knowledgeSequence, givenAnswers, nbAnswers);
		}
		
		/*Debug.Log("Init probability: " + parameters[skillIndex].Value[0]);
		Debug.Log("Transit probability: " + parameters[skillIndex].Value[1]);	
		Debug.Log("Guess probability: " + parameters[skillIndex].Value[2]);
		Debug.Log("Slip probability: " + parameters[skillIndex].Value[3]);*/
	}

	private float UpdateTransitProbability(float p_transit, List<float> sequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=1; i < size; ++i) {
			upperSum += (1.0f - sequence[i-1]) * sequence[i];
			lowerSum += (1.0f - sequence[i-1]);
		}

		// If the update gives 0 or 1, don't update the probability
		if (upperSum == 0.0f || lowerSum == 0.0f) {
			return p_transit;
		}

		//Debug.Log("Transit probability: " + upperSum / lowerSum);

		return upperSum / lowerSum;
	}

	private float UpdateGuessProbability(float p_guess, List<float> knowledgeSequence, List<int> correctnessSequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=0; i < size; ++i) {
			upperSum += correctnessSequence[i] * (1.0f - knowledgeSequence[i]);
			lowerSum += (1.0f - knowledgeSequence[i]);
		}

		// If the update gives 0 or 1, don't update the probability
		if (upperSum == 0.0f || lowerSum == 0.0f) {
			return p_guess;
		}

		return upperSum / lowerSum;
	}

	private float UpdateSlipProbability(float p_slip, List<float> knowledgeSequence, List<int> correctnessSequence, int size) {

		float upperSum = 0.0f;
		float lowerSum = 0.0f;

		for(int i=0; i < size; ++i) {
			upperSum += (1.0f - correctnessSequence[i]) * knowledgeSequence[i];
			lowerSum += knowledgeSequence[i];
		}

		// If the update gives 0 or 1, don't update the probability
		if (upperSum == 0.0f || lowerSum == 0.0f) {
			return p_slip;
		}

		return upperSum / lowerSum;
	}
}
