using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelButton : MonoBehaviour {
	public int Level;
	public bool ForceUnlock;
	public string LevelType;

	void Start() {
		string levelData = PlayerPrefs.GetString("Levels/" + LevelType + "-" + Level, "");
		if (levelData.Equals("")) {
			if (ForceUnlock)
				InitializeButton(Level, 0, false);
			else
				InitializeButton(Level, 0, true);
			return;
		}


		bool isUnlocked = levelData.Split(',')[0].Equals("true") ? true : false;
		int stars = Convert.ToInt16(levelData.Split(',')[1]);

		if (ForceUnlock) {
			InitializeButton(Level, stars, false);
		} else {
			if (isUnlocked)
				InitializeButton(Level, stars, false);
			else
				InitializeButton(Level, 0, true);
		}
	}

	

	public void InitializeButton(int level, int stars, bool isLocked) {
		Level = level;

		// Interactability
		if (isLocked) {
			GetComponent<Button>().interactable = false;
		}

		// Color Init
		Color onColor = GetColorByLevelType((int)LevelsList.LevelType);
		Color offColor = new Color();
		ColorUtility.TryParseHtmlString("#90A4AE40", out offColor);


		// Components to show
		onColor = GetComponent<Image>().color;
		LevelsList.LevelTypeColor = onColor;
		if (isLocked) {
			transform.Find("Number/Text").gameObject.SetActive(false);
			transform.Find("Number/Lock").gameObject.SetActive(true);
			Color lockedColor; ColorUtility.TryParseHtmlString(BondsLibrary.MDB_LIGHT_GREY, out lockedColor); GetComponent<Image>().color = lockedColor; // Set button color to grey
		} else {
			transform.Find("Number/Text").GetComponent<Text>().text = level.ToString();
			transform.Find("Number/Text").GetComponent<Text>().color = onColor;
		}

		
		// Star pattern
		switch (stars) {
			case 1:
				transform.Find("Stars/Star1").GetComponent<Image>().color = onColor;
				transform.Find("Stars/Star2").GetComponent<Image>().color = offColor;
				transform.Find("Stars/Star3").GetComponent<Image>().color = offColor;
				break;
			case 2:
				transform.Find("Stars/Star1").GetComponent<Image>().color = onColor;
				transform.Find("Stars/Star2").GetComponent<Image>().color = onColor;
				transform.Find("Stars/Star3").GetComponent<Image>().color = offColor;
				break;
			case 3:
				transform.Find("Stars/Star1").GetComponent<Image>().color = onColor;
				transform.Find("Stars/Star2").GetComponent<Image>().color = onColor;
				transform.Find("Stars/Star3").GetComponent<Image>().color = onColor;
				break;
			default:
				transform.Find("Stars/Star1").GetComponent<Image>().color = offColor;
				transform.Find("Stars/Star2").GetComponent<Image>().color = offColor;
				transform.Find("Stars/Star3").GetComponent<Image>().color = offColor;
				break;
		}
	}

	public void OnCustomButtonPressed() {
		SceneManager.LoadScene(LevelsList.LevelType + "-" + Level);
	}


	public static Color GetColorByLevelType(int _levelType)
	{
		Color color = new Color();
		switch ((int)LevelsList.LevelType)
		{
			case 0:
				ColorUtility.TryParseHtmlString("#009FB7", out color);
				break;
			case 1:
				ColorUtility.TryParseHtmlString("#FED766", out color);
				break;
			case 2:
				ColorUtility.TryParseHtmlString("#FE4A49", out color);
				break;
		}
		return color;
	}
}
