using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRenderer : MonoBehaviour {
	public SpriteRenderer rank;
	public SpriteRenderer suit;
	public SpriteRenderer cardFrame;
	public Sprite cardFront;
	public Sprite cardBack;
	
	
	public void hideCard() {
		rank.enabled = false;
		suit.enabled = false;
		cardFrame.sprite = cardBack;
	}
	public void showCard() {
		cardFrame.sprite = cardFront;
		rank.enabled = true;
		suit.enabled = true;
	}
	public void updateRank(Sprite spriteimage) {
		rank.sprite = spriteimage;
	}
	public void updateSuit(Sprite spriteimage) {
		suit.sprite = spriteimage;
	}
}
