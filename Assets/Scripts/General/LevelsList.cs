using UnityEngine;
using System;

public class LevelsList : MonoBehaviour {

	public static int NumberOfLevels = 1;
	public static MainMenu.LevelType LevelType = MainMenu.LevelType.Ionic;	// Change to Ionic
	public static Color LevelTypeColor;

	public AchievementPanel _AchievementPanel;

	public void RefreshLevelsList(int levelType) {
		LevelType = (MainMenu.LevelType)levelType;
		switch (levelType) {                // Manual since button click event can't have 2 params
			case 0:                         // Ionic
				NumberOfLevels = 26;
				foreach (Transform trns in transform.Find("Panels").GetComponentInChildren<Transform>()) {
					trns.gameObject.SetActive(false);
				}
				transform.Find("Panels/IonicLevelsList").gameObject.SetActive(true);
				break;
			case 1:							// Covalent
				NumberOfLevels = 81;
				foreach (Transform trns in transform.Find("Panels").GetComponentInChildren<Transform>()) {
					trns.gameObject.SetActive(false);
				}
				transform.Find("Panels/CovalentLevelsList").gameObject.SetActive(true);
				break;
			case 2:                         // Mixed
				NumberOfLevels = 10;
				foreach (Transform trns in transform.Find("Panels").GetComponentInChildren<Transform>()) {
					trns.gameObject.SetActive(false);
				}
				transform.Find("Panels/MixedLevelsList").gameObject.SetActive(true);
				break;
		}
		_AchievementPanel.LoadPlayerData();
		return;
	}

	public void ResetPositions() {
		transform.Find("Panels/IonicLevelsList/ScrollRect/Content").localPosition = Vector3.zero;
		transform.Find("Panels/CovalentLevelsList/ScrollRect/Content").localPosition = Vector3.zero;
		transform.Find("Panels/MixedLevelsList/ScrollRect/Content").localPosition = Vector3.zero;
	}
}
