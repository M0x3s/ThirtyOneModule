﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveManager : MonoBehaviour {
	public Renderer[] lights;
	public Material off;
	public Material on;
	private int count = 0;
	public void handlePass() {
		lights[count].material = on;
		count++;
	}
	public bool isComplete() {
		return count >= lights.Count();
	}
	public void handleStrike() {
		count = 0;
		foreach (Renderer i in lights) {
			i.material = off;
		}
	}
}
