using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genericMotion : MonoBehaviour {
	public bool active = true;
	public enum type {
		translate,
		rotate,
		scale,
		textureOffset,
	}
	public type motionType;
	public float speed = 1f;
	public bool xAxis = false;
	public bool yAxis = false;
	public bool zAxis = false;
	private Renderer mat;
	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (active) {
			if (motionType == type.translate) {
				Vector3 currentPos = transform.localPosition;
				if (xAxis)
					currentPos = currentPos + (transform.right * speed);
				if (yAxis)
					currentPos = currentPos + (transform.up * speed);
				if (zAxis)
					currentPos = currentPos + (transform.forward * speed);
				transform.localPosition = currentPos;
			} else if (motionType == type.scale) {
				Vector3 currentScale = transform.localScale;
				if (xAxis)
					currentScale.x += speed;
				if (yAxis)
					currentScale.y += speed;
				if (zAxis)
					currentScale.z += speed;
				transform.localScale = currentScale;
			} else if (motionType == type.rotate) {
				Vector3 currentRot = transform.localEulerAngles;
				if (xAxis)
					currentRot.x += speed;
				if (yAxis)
					currentRot.y += speed;
				if (zAxis)
					currentRot.z += speed;
				transform.localEulerAngles = currentRot;
			} else if (motionType == type.textureOffset) {
				Vector2 currentUV = mat.material.mainTextureOffset;
				if (xAxis)
					currentUV.x += speed;
				if (yAxis)
					currentUV.y += speed;
				mat.material.mainTextureOffset = currentUV;
			}
		}
	}
}
