using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitTarget : MonoBehaviour {
	public int health = 1;
	public int hitStunTime = 24;
	public Transform[] meshes;
	[HideInInspector]
	public bool gotHit = false;
	private bool renderOn = true;
	private int hitTime = 0;


	// Use this for initialization
	void Awake () {
		
	}

	void Update(){
		if (Input.GetButtonDown("Y")){
			hit(1);
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (gotHit) {
			renderOn = !renderOn;
			hitTime++;
			if (hitTime >= hitStunTime) {
				renderOn = true;
				foreach (Transform r in meshes) {
					r.gameObject.SetActive(renderOn);
				}
				gotHit = false;
			} else {
				foreach (Transform r in meshes) {
					r.gameObject.SetActive(renderOn);
				}
			}
		}
	}
	public virtual void hit(int damage){
		health -= damage;
		if (health <= 0) {
			death ();
		}
		hitTime = 0;
		gotHit = true;
	}

	public virtual void death(){
		Destroy (gameObject);
	}
}
