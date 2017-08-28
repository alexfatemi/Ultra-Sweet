using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagingObstacle : MonoBehaviour {
	public int damage = 1;
	public LayerMask ignoreLayers;
	public float knockbackForce = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision other){
		if (ignoreLayers == (ignoreLayers | 1 << other.gameObject.layer))
			return;
		else {
			if (other.gameObject.GetComponent<hitTarget> () != null) {
				hitTarget target = other.gameObject.GetComponent<hitTarget> ();
				if (!target.gotHit)
					target.hit (damage);
			} else if (other.gameObject == player.mainPlayer.gameObject) {
				player p = other.gameObject.GetComponent<player> ();
				p.hit (damage);
				Vector3 dir = other.contacts [0].point - gameObject.transform.position;
				dir = dir.normalized;
				other.gameObject.GetComponent<Rigidbody> ().AddForce (dir * knockbackForce);
			}
		}
	}
}
