using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanel : MonoBehaviour
{
	// Display items
    public Image _star;
	public Text _valueText;
	public Text _divider;
	public Text _totalText;

	private int _levelType;


	// Data calculation
	private static int _starCount = 0;


	public void LoadPlayerData()
	{
		// Value
		_starCount = 0;
		_totalText.text = (LevelsList.NumberOfLevels * 3).ToString();
		for (int i = 1; i <= LevelsList.NumberOfLevels; i++)
		{
			string levelData = PlayerPrefs.GetString("Levels/" + LevelsList.LevelType + "-" + i, "");
			if (!levelData.Equals(""))
			{
				int stars = int.Parse(levelData.Split(',')[1]);
				if (stars > 0)
					_starCount += stars;
			}
		}
		_valueText.text = _starCount.ToString();


		// Color
		_levelType = (int)LevelsList.LevelType;
		Color _color = LevelButton.GetColorByLevelType(_levelType);
		_star.color = _color;
		_valueText.color = _color;
		_divider.color = _color;
		_totalText.color = _color;
	}
}
