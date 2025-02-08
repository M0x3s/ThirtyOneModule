using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveManager : MonoBehaviour {
	public KMBombModule Module;
	public Renderer[] lights;
	public Material off;
	public Material on;
	private int count = 0;
	public void handlePass() {
		lights[count].material = on;
		count++;
		if (count >= lights.Count()) {
			Module.HandlePass();
		}
	}
	public void handleStrike() {
		count = 0;
		Module.HandleStrike();
		foreach (Renderer i in lights) {
			i.material = off;
		}
	}
}
