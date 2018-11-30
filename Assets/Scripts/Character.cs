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

    public int score;
    public RelationState relationState;
    public SpriteRenderer spriteRenderer;

    public Character(string name, int score, RelationState relationState)
    {
        this.name = name;
        this.score = score;
        this.relationState = relationState;
    }
}