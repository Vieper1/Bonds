using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackController : MonoBehaviour
{
	private MainMenu mainMenu;

	void Start() {
		mainMenu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
	}

    void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			switch (mainMenu.GetMenuState()) {
				case 0:
				case 1:
					Application.Quit();
					break;
				case 2:
					mainMenu.MenuStateChange(1);
					break;
				case 3:
					mainMenu.MenuStateChange(2);
					break;
				case 4:
					mainMenu.MenuStateChange(1);
					break;
			}
		}
    }
}
