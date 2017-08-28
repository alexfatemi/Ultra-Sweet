using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ccGun : enemy {
	private Animator anim;
	private Transform target;
	public GameObject bullet;
	private bool active;
	private bool attacking;
	private bool follow;
	private Vector3 lookTarget;
	private Transform shotSpawn;

	// Use this for initialization
	void Start () {
		anim = GetComponentInChildren<Animator> ();
		target = player.mainPlayer.transform;
		shotSpawn = transform.GetChild (1);
	}
	
	// Update is called once per frame
	void Update () {
		if (active && !attacking)
			StartCoroutine (attack ());
		if (follow) {
			lookTarget = target.position;
			lookTarget.y = transform.position.y;
			transform.LookAt (lookTarget);
		}
	}
	void OnTriggerEnter(Collider other){
		if (other.transform == target)
			active = true;
	}
	void OnTriggerExit(Collider other){
		if (other.transform == target)
			active = false;
	}

	IEnumerator attack () {
		attacking = true;
		follow = true;
		anim.SetTrigger ("attack");
		yield return new WaitForSeconds (0.4f);
		Vector3 targetPos1 = target.position;
		yield return null;
		Vector3 targetPos2 = target.position;
		follow = false;
		yield return null;
		Vector3 shotTarget = targetPos2 - targetPos1;
		shotTarget = targetPos2 + (shotTarget * 1);
		shotTarget.y = transform.position.y;
		transform.LookAt (shotTarget);
		Instantiate (bullet, shotSpawn.position, transform.rotation);
		yield return new WaitForSeconds(1.2f);
		attacking = false;
	}
}
