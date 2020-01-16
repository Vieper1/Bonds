using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {

	public Color FinalColor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, FinalColor, Time.deltaTime * BondsLibrary.LERP_SPEED);
	}
}
