using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character {

    public enum RelationState
    {
        Unknown,
        Acquaintance,
        Friend,
        Mentor,
        GoodFriend,
        BestFriend
    }
    public string name;

    public int empathyScore;
    public int taskScore;
    public RelationState relationState;
    public SpriteRenderer spriteRenderer;

    public Character(string name, int empathyScore, int taskScore, RelationState relationState)
    {
        this.name = name;
        this.empathyScore = empathyScore;
        this.taskScore = taskScore;
        this.relationState = relationState;
    }
}