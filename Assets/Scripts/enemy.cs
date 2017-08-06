using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : hitTarget {
	public int damage = 1;
	public float knockbackForce = 100;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	void OnCollisionEnter(Collision collision){
		if (collision.gameObject == player.mainPlayer.gameObject) {
			player p = collision.gameObject.GetComponent<player> ();
			p.hit (damage);
			Vector3 dir = collision.contacts [0].point - gameObject.transform.position;
			dir = dir.normalized;
			collision.gameObject.GetComponent<Rigidbody> ().AddForce (dir * knockbackForce);
		}
	}
}
