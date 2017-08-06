using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorRandomizer : MonoBehaviour {
	private Renderer mat;
	// Use this for initialization
	void Awake () {
		mat = GetComponent<Renderer> ();
		mat.material.color = Random.ColorHSV (0f, 1f, 0.7f, 0.8f, 0.8f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
