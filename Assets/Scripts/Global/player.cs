using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

	// Use this for initialization
	public static player mainPlayer;
	private Transform playerBody;
	public Transform groundCheck;
	public Animator anim;
	public float groundHeight = 0.2f;
	public LayerMask whatIsGround;
	public float moveSpeed = 10.0F;
	public float currentMoveSpeed;
	public float dashSpeed = 20.0f;
	public float rotateSpeed = 15.0f;
	public int dashLimit = 25;
	public float jumpForce = 700.0f;
	public float jumpFalloff = 30f;
	private float currentJumpForce = 0f;
	public float gravityMultiplier = 1.0f;
	private int jumpBuffer = 0;
	private Vector3 dashDir;
	private int dashTime;
	private ParticleSystem dashPart;
	private ParticleSystem.EmissionModule em;
	private bool attacking = false;
	public int attackTimer = 0;
	//[HideInInspector]
	//public bool controlled = true;
	public enum state
	{
		idle,
		walking,
		jumping,
		falling,
		busy,
		dash,
		landing,
		hitStunned
	}
	//[HideInInspector]
	public state currentState;
	private state attackState;
	[HideInInspector]
	public bool grounded = false;
	private Rigidbody charPhys;
	private Transform[] hitboxes;

	public int health = 4;
	public float hitStunTime = 24f;
	public Transform[] meshes;
	[HideInInspector]
	public bool gotHit = false;
	private bool renderOn = true;
	private float hitTime = 0f;
	private Transform cameraHolder;

	// Use this for initialization
	void Awake (){
		if (mainPlayer == null) {
			mainPlayer = this;
		} else if (mainPlayer != this) {
			Destroy (gameObject);
		}
		charPhys = GetComponent<Rigidbody> ();
		currentMoveSpeed = moveSpeed;
		dashPart = transform.GetChild (1).GetComponent <ParticleSystem>();
		em = dashPart.emission;
		playerBody = transform.GetChild (0);
		hitboxes = playerBody.GetChild(0).GetComponentsInChildren<Transform> ();
		foreach (Transform t in hitboxes)
			t.gameObject.SetActive (false);
		hitboxes [0].gameObject.SetActive (true);
		cameraHolder = Camera.main.transform.parent;
	}
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate(){
		switch (currentState) {
		case state.idle:
			movePlayerGround ();
			break;
		case state.walking:
			movePlayerGround ();
			break;
		case state.jumping:
			movePlayerAir ();
			break;
		case state.falling:
			movePlayerAir ();
			break;
		case state.busy:
			break;
		case state.dash:
			dash ();
			break;
		case state.landing:
			movePlayerGround ();
			break;
		case state.hitStunned:
			grounded = Physics.CheckBox (groundCheck.position, new Vector3 (0.4f, groundHeight, 0.4f), Quaternion.identity, whatIsGround);
			if (!grounded) 
				GetComponent<Rigidbody> ().AddForce (Physics.gravity * charPhys.mass * gravityMultiplier);
			break;
		}
		if (attacking)
			attackTimer++;
		if (gotHit) {
			renderOn = !renderOn;
			hitTime++;
			if ((hitTime >= hitStunTime / 2f) && grounded)
				currentState = state.idle;
			if (hitTime >= hitStunTime) {
				renderOn = true;
				foreach (Transform r in meshes) {
					r.gameObject.SetActive(renderOn);
				}
				gotHit = false;
				currentState = state.idle;
			} else {
				foreach (Transform r in meshes) {
					r.gameObject.SetActive(renderOn);
				}
			}
		}
	}

	void Update() {
		anim.SetBool ("attacking", attacking);
		switch (currentState) {
		case state.idle:
			anim.SetBool ("walking", false);
			anim.SetBool ("grounded", grounded);
			groundInputs ();
			break;
		case state.walking:
			anim.SetBool ("walking", true);
			anim.SetBool ("grounded", grounded);
			groundInputs ();
			break;
		case state.jumping:
			anim.SetBool ("grounded", false);
			anim.SetBool ("falling", false);
			airInputs ();
			break;
		case state.falling:
			anim.SetBool ("grounded", false);
			anim.SetBool ("falling", true);
			airInputs ();
			break;
		case state.busy:
			anim.SetBool ("walking", false);
			anim.SetBool ("grounded", grounded);
			break;
		case state.dash:
			anim.SetBool ("dash", true);
			em.enabled = true;
			if (Input.GetButtonUp ("R") && grounded) {
				em.enabled = false;
				attacking = false;
				attackTimer = 0;
				foreach (Transform t in hitboxes)
					t.gameObject.SetActive (false);
				hitboxes [0].gameObject.SetActive (true);
				anim.SetBool ("dash", false);
				currentState = state.idle;
			}
			groundInputs ();
			break;
		case state.landing:
			currentMoveSpeed = moveSpeed;
			currentState = state.idle;
			break;
		case state.hitStunned:
			currentMoveSpeed = moveSpeed;
			anim.SetBool ("dash", false);
			attacking = false;
			attackTimer = 0;
			em.enabled = false;
			break;
		}
	}

	public void jump (){
		em.enabled = false;
		anim.SetBool ("dash", false);
		if (!grounded)
			return;
		else {
			jumpBuffer = 0;
			if (Input.GetButton("R") && currentState==state.dash)
				currentMoveSpeed = moveSpeed * 2;
			currentJumpForce = jumpForce;
			grounded = false;
			GetComponent<Rigidbody>().AddForce(new Vector3 (0, currentJumpForce, 0));
			currentState = state.jumping;
		}
	}
	public void addJump(){
		currentJumpForce -= jumpFalloff;
		if (currentJumpForce > 0)
			GetComponent<Rigidbody>().AddForce(new Vector3 (0, currentJumpForce, 0));
	}

	public void movePlayerGround() {
		grounded = Physics.CheckBox (groundCheck.position, new Vector3 (0.4f, groundHeight, 0.4f), Quaternion.identity, whatIsGround);
		if (!grounded) {
			currentState = state.falling;
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
			return;
		}
		else if (grounded && Input.GetAxis ("leftYAxis") != 0 || Input.GetAxis ("leftXAxis") != 0) {
			currentState = state.walking;
		}
		else
			currentState = state.idle;

		if (currentState != state.idle) {
			/*float translationVert = -Input.GetAxis ("leftYAxis");
			float translationHorz = Input.GetAxis ("leftXAxis");
			Vector3 movement = new Vector3 (translationHorz, 0.0f, translationVert);
			movement = movement * Camera.main.transform.forward;*/
			Vector3 movement = -Input.GetAxis ("leftYAxis") * cameraHolder.forward + Input.GetAxis ("leftXAxis") * cameraHolder.right;
			movement.y = 0f;
			transform.Translate (movement * currentMoveSpeed * Time.deltaTime, Space.World);
			Vector3 rotateStep = Vector3.RotateTowards (playerBody.forward, movement, rotateSpeed * Time.deltaTime, 0.0f);
			playerBody.rotation = Quaternion.LookRotation (rotateStep);
			GetComponent<Rigidbody> ().AddForce (Physics.gravity * charPhys.mass * gravityMultiplier);
		}
		if (attacking) {
			if (attackTimer > 24 && !hitboxes [2].gameObject.activeSelf && !hitboxes [3].gameObject.activeSelf) {
				attacking = false;
				attackTimer = 0;
				foreach (Transform t in hitboxes)
					t.gameObject.SetActive (false);
				hitboxes [0].gameObject.SetActive (true);
			} else if (attackTimer > 48 && !hitboxes [3].gameObject.activeSelf) {
				attacking = false;
				attackTimer = 0;
				foreach (Transform t in hitboxes)
					t.gameObject.SetActive (false);
				hitboxes [0].gameObject.SetActive (true);
			} else if (attackTimer >= 72) {
				attacking = false;
				attackTimer = 0;
				foreach (Transform t in hitboxes)
					t.gameObject.SetActive (false);
				hitboxes [0].gameObject.SetActive (true);
			}
		}
	}

	public void movePlayerAir(){
		jumpBuffer++;
		grounded = Physics.CheckBox (groundCheck.position, new Vector3 (0.4f, groundHeight, 0.4f), Quaternion.identity, whatIsGround);
		if (grounded && jumpBuffer > 3) {
			currentState = state.landing;
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
			return;
		}
		else if (!grounded && charPhys.velocity.y < 0.1)
			currentState = state.falling;
		else if (!grounded)
			currentState = state.jumping;
		/*(float translationVert = -Input.GetAxis ("leftYAxis");
		float translationHorz = Input.GetAxis ("leftXAxis");
		Vector3 movement = new Vector3 (translationHorz, 0.0f, translationVert);*/
		Vector3 movement = -Input.GetAxis ("leftYAxis") * cameraHolder.forward + Input.GetAxis ("leftXAxis") * cameraHolder.right;
		movement.y = 0f;
		transform.Translate (movement * currentMoveSpeed * Time.deltaTime, Space.World);
		Vector3 rotateStep = Vector3.RotateTowards (playerBody.forward, movement, rotateSpeed * Time.deltaTime, 0.0f);
		playerBody.rotation = Quaternion.LookRotation (rotateStep);
		GetComponent<Rigidbody> ().AddForce (Physics.gravity * charPhys.mass * gravityMultiplier);

		if (attacking && attackTimer >= 24) {
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
		}
	}

	public void groundInputs(){
		if (Input.GetButtonDown ("A")) {
			jump ();
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
		}
		else if (Input.GetButtonDown ("X"))
			attack ();
		else if (grounded && Input.GetButtonDown ("R") && (Input.GetAxis ("leftYAxis") != 0 || Input.GetAxis ("leftXAxis") != 0)) {
			currentState = state.dash;
			/*float translationVert = -Input.GetAxis ("leftYAxis");
			float translationHorz = Input.GetAxis ("leftXAxis");
			dashDir = new Vector3 (translationHorz, 0.0f, translationVert);*/
			Vector3 movement = -Input.GetAxis ("leftYAxis") * Camera.main.transform.forward + Input.GetAxis ("leftXAxis") *Camera.main.transform.right;
			movement.y = 0f;
			dashDir = movement;
			dashTime = 0;
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
		}
	}

	public void airInputs(){
		if (Input.GetButtonUp ("A"))
			currentJumpForce = 0f;
		if (Input.GetButton("A"))
			addJump();
		if (Input.GetButtonDown ("X"))
			attack ();
	}

	public void dash(){
		dashTime++;
		grounded = Physics.CheckBox (groundCheck.position, new Vector3 (0.4f, groundHeight, 0.4f), Quaternion.identity, whatIsGround);
		if (!grounded) {
			em.enabled = false;
			anim.SetBool ("dash", false);
			currentState = state.falling;
		} else if (dashTime <= dashLimit) {
			em.enabled = true;
			playerBody.rotation = Quaternion.LookRotation (dashDir);
			transform.Translate (playerBody.forward * dashSpeed * Time.deltaTime, Space.World);
		} else {
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);
			currentState = state.idle;
			em.enabled = false;
			anim.SetBool ("dash", false);
		}
	}

	public void attack() {
		attackState = currentState;
		if (attackState == state.idle || attackState == state.walking) {
			if (attackTimer == 0) {
				anim.SetInteger ("combo", 1);
				hitboxes [1].gameObject.SetActive (true);
			}
			else if (attackTimer < 12)
				return;
			else if (attackTimer >= 12 && attackTimer <= 24) {
				anim.SetInteger ("combo", 2);
				hitboxes [1].gameObject.SetActive (false);
				hitboxes [2].gameObject.SetActive (true);
			} else if (attackTimer > 24 && attackTimer < 36) {
				return;
			} else if (attackTimer >= 36 && attackTimer <= 48) {
				anim.SetInteger ("combo", 3);
				hitboxes [2].gameObject.SetActive (false);
				hitboxes [3].gameObject.SetActive (true);
			} else if (attackTimer > 48) {
				return;
			}
		} else if (attackState == state.dash) {
			if (attacking)
				return;
			hitboxes [5].gameObject.SetActive (true);
		} else if (attackState == state.jumping || attackState == state.falling) {
			if (attacking)
				return;
			hitboxes [4].gameObject.SetActive (true);
		}
		attacking = true;
	}

	public void hit(int damage){
		if (gotHit)
			return;
		else {
			em.enabled = false;
			attacking = false;
			attackTimer = 0;
			foreach (Transform t in hitboxes)
				t.gameObject.SetActive (false);
			hitboxes [0].gameObject.SetActive (true);

			globals.hp -= damage;
			if (globals.hp <= 0) {
				globals.hp = 0;
			}
			hitTime = 0;
			gotHit = true;
			currentState = state.hitStunned;
			levelGUI.gui.refreshGUI ();
		}
	}


}
