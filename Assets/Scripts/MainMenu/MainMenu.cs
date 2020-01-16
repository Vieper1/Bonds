using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

	public enum MenuState {
		Intro,
		MainMenu,
		ModeSelect,
		LevelSelect
	}
	public enum LevelType {
		Ionic,
		Covalent,
		Mixed
	}

	private Animator anim;
	public static bool isSplashDirty;

	





	void Start () {
		anim = GetComponent<Animator>();
		if (isSplashDirty) {
			anim.SetBool("isSplashDirty", isSplashDirty);
			anim.SetInteger("MenuState", 1);
		}
	}







	// Game Over Event
	void OnEnable() {
		StartCoroutine(SubscribeCoroutine());
	}
	IEnumerator SubscribeCoroutine() {
		yield return new WaitForSeconds(0.5f);
		GameController.Instance.OnGameOver += OnGameOver;
	}
	void OnDisable() {
		GameController.Instance.OnGameOver -= OnGameOver;
		AdsController.HideBanner();
	}
	void OnGameOver() {
		anim.SetInteger("MenuState", (int)MenuState.MainMenu);
		isSplashDirty = true;
	}

	public void MenuStateChange(int value) {
		anim.SetInteger("MenuState", value);
	}
	public int GetMenuState() {
		return anim.GetInteger("MenuState");
	}
	public void MenuLevelState() {
		anim.SetInteger("MenuState", 3);
	}
}
