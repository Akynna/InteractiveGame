using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is the Skill instance. A Skill is defined by:
 *
 *  name: The name of the skill (e.g. Task, Empathy,...)
 *  subskills: A set of subskills associated to that Skill
 *  => It's a set because it is used to keep track of the
 *     subskills that have NOT been evaluated yet
 *
 * A "subskill" is a situation / scenario used to evaluate
 * the particular Skill it belongs to.
 * 
 */
public class Skill {

    public string name;
    public float importanceWeight;
    public HashSet<string> subskills; 

    // Constructor for a main skill
    public Skill(string name, float importanceWeight, HashSet<string> subskills)
    {
        this.name = name;
        this.importanceWeight = importanceWeight;
        this.subskills = subskills;
    }

    /*public List<Skill> GetSkillsWithMastery(Mastery mastery) {

        List<Skill> skills = new List<Skill>();
        

        //return 
    }*/

}