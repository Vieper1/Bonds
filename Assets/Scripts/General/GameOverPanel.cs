using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour {

	private static readonly int AdThreshold = 4;
	private bool isLevelComplete;
	private bool shown;

	public void HomeButtonClicked() {
		SceneManager.LoadScene("MainMenu");
	}
	//public void AdButtonClicked() {
	//	Application.OpenURL("market://details?id=com.vishanstudios.colorshifter2d");
	//}

	public void NextButtonClicked() {
		string sceneName = SceneManager.GetActiveScene().name;
		try {
			if (isLevelComplete)
				SceneManager.LoadScene(BondsLibrary.GetNextLevelName(SceneManager.GetActiveScene().name));
			else
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		} catch (Exception) {
			SceneManager.LoadScene("MainMenu");
		}
	}

	void OnEnable() {
		StartCoroutine(SubscribeCoroutine());
		Color bgColor;
		ColorUtility.TryParseHtmlString("#1C2331F0", out bgColor);
		//bgColor.a = 225f / 255f;
		transform.Find("Background").GetComponent<Image>().color = bgColor;
	}
	IEnumerator SubscribeCoroutine() {
		yield return new WaitForSeconds(0.5f);
		GameController.Instance.OnGameOver += UpdateScore;
	}
	void OnDisable() {
		GameController.Instance.OnGameOver -= UpdateScore;
	}



	void UpdateScore() {
		if (shown) return;

		int timesPlayed = PlayerPrefs.GetInt("TimesPlayed", 0);
		PlayerPrefs.SetInt("TimesPlayed", timesPlayed + 1);
		if ((timesPlayed + 1) % AdThreshold == 0) {
			if (PlayerPrefs.GetInt("RemoveAds", 0) != 1911)
				AdsController.ShowInterstitial();
		}

		GetComponent<Animator>().SetBool("isGameOver", true);
		transform.Find("CompoundNameText").GetComponent<Text>().text = GameController.Instance.TargetCompoundName;
		if (GameController.Instance.GameTime < GameController.Instance.GoldTime) {
			WriteLevelData(3, true);
			DisplayStars(3);
			DisplayText("Awesome!");
		} else if (GameController.Instance.GameTime < GameController.Instance.SilverTime) {
			WriteLevelData(2, true);
			DisplayStars(2);
			DisplayText("Beautiful!");
		} else if (GameController.Instance.GameTime < GameController.Instance.BronzeTime) {
			WriteLevelData(1, true);
			DisplayStars(1);
			DisplayText("Nice!");
		} else {
			WriteLevelData(0, false);
			DisplayStars(0);
			DisplayText("You took too long!");
			transform.Find("NextButton/Next").GetComponent<SpriteRenderer>().sprite = GameObject.Find("GameplayCanvas/ReloadButton/Reload").GetComponent<SpriteRenderer>().sprite;
		}
		shown = true;
	}

	
	void WriteLevelData(int value, bool unlockNextLevel) {
		string levelData = PlayerPrefs.GetString("Levels/" + SceneManager.GetActiveScene().name, "");
		if (levelData.Equals("")) {
			PlayerPrefs.SetString("Levels/" + SceneManager.GetActiveScene().name, "true," + value);
		} else {
			if (value > Convert.ToInt16(levelData.Split(',')[1])) {
				PlayerPrefs.SetString("Levels/" + SceneManager.GetActiveScene().name, "true," + value);
			}
		}
		
		if (unlockNextLevel) {
			PlayerPrefs.SetString("Levels/" + BondsLibrary.GetNextLevelName(SceneManager.GetActiveScene().name), "true,0");
			isLevelComplete = true;
		}
	}
	void DisplayText(string value) {
		transform.Find("LevelCompleteText").GetComponent<Text>().text = value;
		if (value.Equals("You took too long!")) {
			transform.Find("LevelCompleteText").GetComponent<Text>().fontSize = 124;
		}
	}
	void DisplayStars(int value) {
		Color enableColor;
		ColorUtility.TryParseHtmlString(BondsLibrary.MDB_LIGHT_BLUE, out enableColor);
		switch (value) {
			case 3:
				transform.Find("Stars/Star3").GetComponent<Image>().color = enableColor;
				goto case 2;
			case 2:
				transform.Find("Stars/Star2").GetComponent<Image>().color = enableColor;
				goto case 1;
			case 1:
				transform.Find("Stars/Star1").GetComponent<Image>().color = enableColor;
				break;
			default:
				break;
		}
	}
}
