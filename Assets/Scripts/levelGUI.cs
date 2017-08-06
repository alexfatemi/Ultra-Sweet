using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelGUI : MonoBehaviour {
	public static levelGUI gui;
	public Transform hearts;
	public Image[] heartPieces;
	private int currentHealth;
	// Use this for initialization
	void Awake () {
		if (gui == null) {
			gui = this;
		} else if (gui != this) {
			Destroy (gameObject);
		}
		currentHealth = globals.maxHp;
		globals.hp = currentHealth;
		heartPieces = hearts.GetComponentsInChildren<Image>();
		int hCount = -1;
		foreach (Image i in heartPieces) {
			hCount++;
			if (hCount >= currentHealth)
				i.enabled = false;
		}
	}

	public void refreshGUI(){
		currentHealth = globals.hp;
		int hCount = -1;
		foreach (Image i in heartPieces) {
			hCount++;
			if (hCount >= currentHealth)
				i.enabled = false;
		}
	}
}
