using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
	
	void Start () {
		if (PlayerPrefs.GetInt("MusicEnabled", 1) == 0) {
			transform.Find("MusicButton/Text").GetComponent<Text>().text = "Music: OFF";
			AudioListener.pause = true;
		}
		if (PlayerPrefs.GetInt("FxaaEnabled", 0) == 0) {
			transform.Find("FxaaButton/Text").GetComponent<Text>().text = "Soft Graphics: OFF";
			GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.None;
		}
	}
	
	public void OnMusicTogglePressed() {
		if (PlayerPrefs.GetInt("MusicEnabled", 1) == 1) {
			PlayerPrefs.SetInt("MusicEnabled", 0);
			transform.Find("MusicButton/Text").GetComponent<Text>().text = "Music: OFF";
			AudioListener.pause = true;
		} else {
			PlayerPrefs.SetInt("MusicEnabled", 1);
			transform.Find("MusicButton/Text").GetComponent<Text>().text = "Music: ON";
			AudioListener.pause = false;
		}
	}

	public void OnFxaaTogglePressed() {
		if (PlayerPrefs.GetInt("FxaaEnabled", 1) == 1) {
			PlayerPrefs.SetInt("FxaaEnabled", 0);
			transform.Find("FxaaButton/Text").GetComponent<Text>().text = "Soft Graphics: OFF";
			GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.None;
		} else {
			PlayerPrefs.SetInt("FxaaEnabled", 1);
			transform.Find("FxaaButton/Text").GetComponent<Text>().text = "Soft Graphics: ON";
			GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
			GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().fastApproximateAntialiasing.fastMode = true;
		}
	}
}
