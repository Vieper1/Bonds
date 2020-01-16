using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorwaveButton : MonoBehaviour {

	public float ChangeTime = 2f;
	public float LerpSpeed = 2f;

	private Image btn;
	private string targetColorHex = "";
	private Color targetColor;
	private float textTime;
	private float onTime;

	void Start () {
		btn = GetComponent<Image>();
		ColorUtility.TryParseHtmlString(BondsLibrary.MDB_RED, out targetColor);
	}

	void Update () {
		textTime += Time.deltaTime;
		onTime += Time.deltaTime;
		if (textTime > ChangeTime) {
			textTime = 0f;
			if (targetColorHex.Equals(BondsLibrary.MDB_LIGHT_BLUE)) {
				targetColorHex = BondsLibrary.MDB_GREEN;
			} else if (targetColorHex.Equals(BondsLibrary.MDB_GREEN)) {
				targetColorHex = BondsLibrary.MDB_RED;
			} else if (targetColorHex.Equals(BondsLibrary.MDB_RED)) {
				targetColorHex = BondsLibrary.MDB_LIGHT_BLUE;
			} else {
				targetColorHex = BondsLibrary.MDB_LIGHT_BLUE;
			}
			ColorUtility.TryParseHtmlString(targetColorHex, out targetColor);
		}

		btn.transform.localScale = Vector3.one + (Vector3.one * Mathf.Sin(onTime * 3f) * 0.12f);
		btn.color = Color.Lerp(btn.color, targetColor, Time.deltaTime * LerpSpeed);
	}
}
