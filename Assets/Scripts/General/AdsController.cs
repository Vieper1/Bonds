using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour
{
	bool testMode = false;

#if UNITY_ANDROID
	string gameId = "3221250";
#elif UNITY_IOS
	string gameId = "3221251";
#elif UNITY_EDITOR
	string gameId = "3221250";
#endif


	void Start() {
		Advertisement.Initialize(gameId, testMode);
		StartCoroutine(ShowBannerWhenReady());
	}

	IEnumerator ShowBannerWhenReady () {
		while (!Advertisement.IsReady("menu_banner")) {
			yield return new WaitForSeconds(0.5f);
		}
		Debug.LogWarning("Viper Banner Showing!");
		Advertisement.Banner.SetPosition(BannerPosition.TOP_RIGHT);
		Advertisement.Banner.Show("menu_banner");
	}

	public static void HideBanner() {
		Advertisement.Banner.Hide();
	}

	public static void ShowInterstitial() {
		Advertisement.Show("video");
	}
}
