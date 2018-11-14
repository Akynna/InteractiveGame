using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Background {

	public float xCoords;
    public float yCoords;
    public float zCoords;
    public string name;

    public Background(string name, float x, float y, float z)
    {
        this.name = name;
        this.xCoords = x;
        this.yCoords = y;
        this.zCoords = z;
    }
}
