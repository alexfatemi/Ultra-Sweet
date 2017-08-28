using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death : MonoBehaviour {
	Transform respawnPoint;
	// Use this for initialization
	void Awake () {
		respawnPoint = transform.GetChild (0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){
		collision.gameObject.transform.position = respawnPoint.position;
		collision.gameObject.transform.rotation = respawnPoint.rotation;
	}
}
