using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTitle : MonoBehaviour {

	public float speed;
	public float width;
	private int counter;

	// Use this for initialization
	void Start () {
		counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += this.transform.right * speed * Time.deltaTime;
		counter++;

		if(counter > width) {
			speed = -speed;
			counter = 0;
		}
	}
}
