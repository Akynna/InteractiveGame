using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*
 *	This is the main Manager. The Story Manager is the only one that have
 *  access to the Dialogues Table. This Manager communicates with all other
 *	Managers to to build the story.
 */
public class StoryManager : MonoBehaviour {


	public DialoguesTable dialoguesTable;

	// Managers with whom the Story Manager communicates
	public AudioManager AudioManager;
	public CharacterManager CharacterManager;
	public DialogueManager DialogueManager;
	public ScoreManager ScoreManager;
	public SkillManager SkillManager;
	
	
	public List<DialoguesTable.Row> currentSceneDialogues;

	// Variables for the backgrounds
	public SceneChanger sceneChanger;

	// Used to save the logs
	private List<string> listAnswers;

	// Used for the sound of dialogue
	public AudioSource effectsSource;


	//=======================================
	//				INIALIZATION
	//=======================================
	void Start () {

		// Initialize all the Managers
		CharacterManager.Initialize();
		DialogueManager.Initialize();
		SkillManager.Initialize();
		
		// Used to save the logs
		listAnswers = new List<string>();
		
		// Initialize the first scene
		currentSceneDialogues = dialoguesTable.FindAll_sceneID("intro_0");
		
		// Initialize the background
		sceneChanger.SwitchBackground(currentSceneDialogues[0].background);

		// Launch the first scene !
		DialogueManager.TriggerDialogue();
	}

	//=======================================
	//			GETTERS & ACCESSORS
	//=======================================

	// Return the set of characters' names
	public HashSet<string> GetCharacterNames() {

		HashSet<string> names = new HashSet<string>();

		foreach(DialoguesTable.Row row in dialoguesTable.rowList)
		{
			names.Add(row.character);
		}

		return names;
	}

	// Return the set of main skills' names
	public HashSet<string> GetMainSkillsNames() {

		HashSet<string> mainSkills = new HashSet<string>();

		foreach(DialoguesTable.Row row in dialoguesTable.rowList)
		{
			if(row.main_skill != "NA") {
				mainSkills.Add(row.main_skill);
			}
		}


		return mainSkills;
	}

	// Return the set of subskills' names associated with the given main skill name
	public HashSet<string> GetSubSkillsNames(string mainSkillName) {

		HashSet<string> subSkills = new HashSet<string>();

		// Get only the rows that are concerned by the specified skill
		List<DialoguesTable.Row> rows = dialoguesTable.FindAll_main_skill(mainSkillName);

		foreach(DialoguesTable.Row row in rows)
		{
			// Add the name of the corresponding scene associated with that skill
			subSkills.Add(row.sceneID);
		}

		return subSkills;
	}


	//=======================================
	//			HELPER FUNCTIONS
	//=======================================

	public void SwitchScene(string sceneID, string skillName, string subskillName, int score, string answerText)
	{
		// If we previously displayed the choice panel
		if(DialogueManager.WasAChoice())
		{	
			if(score != 0) {

				// Give a random feedback
				// characterManager.randomFeedback(answerType);

				// Modify the relation score
				ScoreManager.UpdatePoints(skillName, score);
			}

			
			// Update the skill probabilities after evaluating the current skill
			SkillManager.UpdateSkill(skillName, subskillName, score);
		}

		// Record the answer given in a file
		listAnswers.Add(answerText);

		if(sceneID == "end") {
			sceneChanger.FadeToLevel(2);
			SaveAnswers();

		} else {
			// Load the dialogues of the next scene in the Dialogue Manager
			// TODO : CHOOSE THE NEXT SCENE DEPENDING ON PROBABILITIES

			string nextSceneID = "";

			// If we specified in the CSV that we want to choose automatically the next scene
			if(sceneID == "auto") {
		
				// Choose one of the next scene depending on probs
				string newSkillName = SkillManager.ChooseSkill();
				//Debug.Log("Skill name chosen: " + skill.name);
				nextSceneID = SkillManager.ChooseSubskill(newSkillName);
			}
			
			// Otherwise, just switch to the chosen scene given as argument
			else {
				nextSceneID = sceneID;
			}


			currentSceneDialogues =  dialoguesTable.FindAll_sceneID(nextSceneID);

			// Switch the background image if needed
			sceneChanger.SwitchBackground(currentSceneDialogues[0].background);
			
			// Trigger the dialogues of the next scene
			DialogueManager.TriggerDialogue();

		}
	}

	private void SaveAnswers() {

		using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter("testfile.txt", false))

		foreach(string answer in listAnswers)
		{
			file.WriteLine(answer);
		}
	}
	
	
}
