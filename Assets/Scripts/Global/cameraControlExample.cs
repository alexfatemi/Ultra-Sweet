﻿using UnityEngine;
using System.Collections;

public class cameraControlExample : MonoBehaviour {
	[Header("Camera Properties")]
	public float DistanceAway;                     //how far the camera is from the player.
	public float minDistance = 1f;
	public float maxDistance = 15f;
	public float DistanceUp;                    //how high the camera is above the player
	public float smooth = 4.0f;                    //how smooth the camera moves into place
	public float rotateAround = 70f;            //the angle at which you will rotate the camera (on an axis)
	[Header("Player to follow")]
	public Transform target;                    //the target the camera follows
	[Header("Layer(s) to include")]
	public LayerMask CamOcclusion;                //the layers that will be affected by collision
	[Header("Map coordinate script")]
	RaycastHit hit;
	float cameraHeight = 55f;
	float cameraPan = 0f;
	float camRotateSpeed = 180f;
	Vector3 camPosition;
	Vector3 camMask;
	Vector3 followMask;
	Transform mCam;
	// Use this for initialization
	void Start () {
		//the statement below automatically positions the camera behind the target.
		rotateAround = target.eulerAngles.y - 45f;
		mCam = Camera.main.transform;
	}
	void Update(){

	}
	// Update is called once per frame

	void LateUpdate () {
		//Offset of the targets transform (Since the pivot point is usually at the feet).
		Vector3 targetOffset = new Vector3(target.position.x, target.position.y, target.position.z);
		Quaternion rotation = Quaternion.Euler(cameraHeight, rotateAround, cameraPan);
		Vector3 vectorMask = Vector3.one;
		Vector3 rotateVector = rotation * vectorMask;
		//this determines where both the camera and it's mask will be.
		//the camMask is for forcing the camera to push away from walls.
		camPosition = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;
		camMask = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;

		occludeRay(ref targetOffset);
		smoothCamMethod();

		Vector3 floatingTarget = new Vector3 (target.position.x, transform.position.y, target.position.z);
		transform.LookAt (floatingTarget);
		mCam.LookAt(target);

		#region wrap the cam orbit rotation
		if(rotateAround > 360){
			rotateAround = 0f;
		}
		else if(rotateAround < 0f){
			rotateAround = (rotateAround + 360f);
		}
		#endregion

		rotateAround += Input.GetAxis ("rightXAxis") * camRotateSpeed * Time.deltaTime;
		DistanceAway = Mathf.Clamp(DistanceAway += Input.GetAxis ("rightYAxis") * 0.25f, minDistance, maxDistance);
	}
	void smoothCamMethod(){
		transform.position = Vector3.Lerp (transform.position, camPosition, Time.deltaTime * smooth);
	}
	void occludeRay(ref Vector3 targetFollow){
		#region prevent wall clipping
		//declare a new raycast hit.
		RaycastHit wallHit = new RaycastHit();
		//linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
		if(Physics.Linecast(targetFollow, camMask, out wallHit, CamOcclusion)){
			//the x and z coordinates are pushed away from the wall by hit.normal.
			//the y coordinate stays the same.
			camPosition = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, camPosition.y, wallHit.point.z + wallHit.normal.z * 0.5f);
		}
		#endregion
	}
}
