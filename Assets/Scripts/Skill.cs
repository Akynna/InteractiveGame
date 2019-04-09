using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public enum Mastery
    {
        Beginner,
        Intermediate,
        Advanced   
    }

    public string name;

    public Mastery skillMastery;
    public List<Skill> subSkills;

    // Constructor for a subskill
    public Skill(string name, Mastery skillMastery)
    {
        this.name = name;
        this.skillMastery = skillMastery;
        this.subSkills = new List<Skill>();
    }

    // Constructor for a main skill
    public Skill(string name, Mastery skillMastery, List<Skill> subSkills)
    {
        this.name = name;
        this.skillMastery = skillMastery;
        this.subSkills = subSkills;
    }

    /*public List<Skill> GetSkillsWithMastery(Mastery mastery) {

        List<Skill> skills = new List<Skill>();
        

        //return 
    }*/

}