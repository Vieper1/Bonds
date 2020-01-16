using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateController : MonoBehaviour {

	public int targetFRate = 60;

	// Use this for initialization
	void Awake() {
		QualitySettings.vSyncCount = 0;
	}

	// Update is called once per frame
	void Update() {
		if (Application.targetFrameRate != targetFRate) {
			Application.targetFrameRate = targetFRate;
			Debug.Log("Frame rate fixed!");
		}
			
	}
}
