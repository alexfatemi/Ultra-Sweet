using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minecart : MonoBehaviour {
	public float cartSpeed = 25f;
	private player marz;
	private bool moving;
	public Transform[] path;
	// Use this for initialization
	void Start () {
		marz = player.mainPlayer;
		path = transform.GetChild (0).GetComponentsInChildren<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerStay(Collider other){
		if (other.gameObject == marz.gameObject && marz.grounded && marz.transform.parent != transform) {
			marz.transform.parent = transform;
			if (!moving) {
				moving = true;
				iTween.MoveTo(gameObject, iTween.Hash("path", path, "orienttopath", true, "lookahead", 0.1f, "speed", cartSpeed, "delay", 1f, "easetype", iTween.EaseType.linear, "looktime", 0f));
			}
		}
	}
	void OnTriggerExit(Collider other){
		if (other.gameObject == marz.gameObject)
			marz.transform.parent = null;
	}
}
