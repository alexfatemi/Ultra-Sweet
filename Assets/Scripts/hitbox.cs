using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitbox : MonoBehaviour {
	public int damage = 1;
	public LayerMask ignoreLayers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerStay(Collider other){
		if (ignoreLayers == (ignoreLayers | 1 << other.gameObject.layer))
			return;
		else {
			if (other.GetComponent<hitTarget> () != null) {
				hitTarget target = other.GetComponent<hitTarget> ();
				if(!target.gotHit)
					target.hit (damage);
			}
		}
	}
}
