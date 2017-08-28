using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour {
	public Transform target;
	public float rotationSpeed = 10f;
	public float distance = 10f;
	public float distMax = 15f;
	public float distMin = 5f;
	public float height = 5f;
	public float heightDamping = 2f;
	public float rotationDamping = 3f;
	public float smoothing = 4f;
	public LayerMask camOcclusion;
	private Vector3 camMask;
	private Vector3 camPos;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	void LateUpdate(){
		if (!target)
			return;
		if (Input.GetAxis ("rightYAxis") != 0) {
			distance += Input.GetAxis ("rightYAxis") * 0.2f;
			if (distance >= distMax)
				distance = distMax;
			else if (distance <= distMin)
				distance = distMin;
		}
		if (Input.GetAxis ("rightXAxis") != 0) {
			//Debug.Log (Input.GetAxis ("rightXAxis"));
			target.transform.Rotate (Vector3.up * Input.GetAxis("rightXAxis") * rotationSpeed * Time.deltaTime);
		}

		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		camPos = target.position - (currentRotation * Vector3.forward * distance);
		camPos.y = currentHeight;
		occludeRay ();
		camPos = Vector3.Lerp (transform.position, camPos, Time.deltaTime * smoothing);
		transform.position = camPos;
		transform.LookAt (target);
	}

	void occludeRay(){
		//declare a new raycast hit.
		RaycastHit wallHit = new RaycastHit();
		//linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
		if (Physics.Linecast (target.position, transform.position, out wallHit, camOcclusion)) {
			//the x and z coordinates are pushed away from the wall by hit.normal.
			//the y coordinate stays the same.
			//camPos = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, transform.position.y, wallHit.point.z + wallHit.normal.z * 0.5f);
			distance -= 0.5f;
		} else {
			if (Input.GetAxis ("rightYAxis") != 0) {
				distance += Input.GetAxis ("rightYAxis") * 0.2f;
				if (distance >= distMax)
					distance = distMax;
				else if (distance <= distMin)
					distance = distMin;
			}
		}
	}
}
