using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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

    public List<List<string>> chapterTracker;

	public string changingChapterKey = "intro";

	//=======================================
	//				INIALIZATION
	//=======================================
	void Start () {
        
        // Initialize all the Managers
        AudioManager.Initialize();
        CharacterManager.Initialize();
        DialogueManager.Initialize();
        SkillManager.Initialize();

        // Used to save the logs
        listAnswers = new List<string>();

        if (PlayerPrefs.GetFloat("LoadScene") != 1)
        {
            // Initialize the first scene
            currentSceneDialogues = dialoguesTable.FindAll_sceneID(dialoguesTable.GetRowList()[0].sceneID);

        } else
        {
            LoadScene();
        }

        // Initialize the background
        sceneChanger.SwitchBackground(currentSceneDialogues[0].background);

        // Launch the first scene !
        DialogueManager.TriggerDialogue();

        List<DialoguesTable.Row> rowTable = new List<DialoguesTable.Row>(dialoguesTable.GetRowList());
        List<string> names = rowTable.Select(x => x.sceneID).Distinct().ToList();
        chapterTracker = new List<List<string>>(FileManager.GetFileChapter(names));
    }

    void OnApplicationQuit()
    {
        FileManager.OverwriteFileChapter(chapterTracker);
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

	public void SwitchScene(string sceneID, string skillName, string subskillName, int score, string answerText, string audioName)
	{

        // Track the scene
        if(!sceneID.Equals("auto")) {
			chapterTracker[1][chapterTracker[0].IndexOf(sceneID)] = "1";
		}

		// If we previously displayed the choice panel
		if(DialogueManager.WasAChoice())
		{
            MicrophoneController.recording = true;
            MicrophoneController.answer = answerText;

			// Play the audio of the answer
        	AudioManager.UpdateEffectSound(audioName);
        	AudioManager.speechPlayer.PlayOneShot(AudioManager.speechPlayer.clip);

            if (score != 0 && skillName != "NA") {

				// Give a random feedback
				// characterManager.randomFeedback(answerType);

				// Modify the relation score
				ScoreManager.UpdatePoints(skillName, score);
			}

			// First method to update skill
			// Update the skill probabilities after evaluating the current skill
			// SkillManager.UpdateSkill(skillName, subskillName, score);

			// Second method to update skill
			// Update the skill probabilities after evaluating the current skill
			SkillManager.BKT(skillName, subskillName, score);
		}

		// Record the answer given in a file
		listAnswers.Add(answerText);

		if(sceneID.Equals("end"))
        {
            PlayRecordings.finalEvaluation = true;
            PlayRecordings.evaluating = true;
            //sceneChanger.FadeToLevel(2);
			SaveAnswers();

		} else {
			// Load the dialogues of the next scene in the Dialogue Manager
			string nextSceneID = "";

			// If we specified in the CSV that we want to choose automatically the next scene
			if(sceneID == "auto") {
		
				// Choose one of the next scene depending on probs
				var newSkillName = SkillManager.ChooseSkill();
				//Debug.Log("Skill name chosen: " + skill.name);
				nextSceneID = SkillManager.ChooseSubskill(newSkillName);
			}
			
			// Otherwise, just switch to the chosen scene given as argument
			else {
				nextSceneID = sceneID;
			}
			
			

            // Put the name of the key that indicates new chapter
            if (nextSceneID.Contains(changingChapterKey))
            {
                PlayRecordings.evaluating = true;
            }

			currentSceneDialogues =  dialoguesTable.FindAll_sceneID(nextSceneID);

			// Switch the background image if needed
			sceneChanger.SwitchBackground(currentSceneDialogues[0].background);

			// Trigger the dialogues of the next scene
			if (MicrophoneController.recording) {
				Debug.Log(AudioManager.speechPlayer.clip.length);
				StartCoroutine(WaitForAudio(AudioManager.speechPlayer.clip.length));
			} else {
				DialogueManager.TriggerDialogue();
			}
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
	
    public void LoadScene()
    {
        string sceneID = PlayerPrefs.GetString("SceneToLoad");
        // Initialize the dialogue to this scene
        currentSceneDialogues = dialoguesTable.FindAll_sceneID(sceneID);

        List<Character> characters = new List<Character>(CharacterManager.characterList);
        foreach(Character character in characters)
        {
            CharacterManager.GetCharacterByName(character.name).empathyScore += (int) PlayerPrefs.GetInt(character.name);
        }
        PlayerPrefs.SetFloat("LoadScene", 0f);
    }

    private IEnumerator WaitForAudio(float audioLength) {
		yield return new WaitForSeconds(audioLength);
		DialogueManager.TriggerDialogue();
	}
	
}
