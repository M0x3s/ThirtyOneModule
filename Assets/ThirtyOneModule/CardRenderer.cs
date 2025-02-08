using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRenderer : MonoBehaviour {
	public SpriteRenderer rank;
	public SpriteRenderer suit;
	

	// Use this for initialization
	void Start () {
		
	}
	
	public void hideDetails() {
		rank.enabled = false;
		suit.enabled = false;
	}
	public void updateRank(Sprite spriteimage) {
		rank.sprite = spriteimage;
	}
	public void updateSuit(Sprite spriteimage) {
		suit.sprite = spriteimage;
	}
}
