using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Atom : MonoBehaviour {

	public static float CouplingForceMultiplier = 12f;
	public static float PushingForceMultiplier = 6f;

	public string AtomName;
	public int InitElectrons;
	public bool zeroElectronGenerosity;
	public bool octetException;
	public GameObject[] Orbit1 = new GameObject[2];
	public GameObject[] Orbit2 = new GameObject[8];
	public GameObject[] Orbit3 = new GameObject[18];
	public GameObject[] Covalent = new GameObject[36];
	public List<GameObject> BoundTo;


	public int charge;
	private Text chargeText;

	private GameObject OrbitCircle1;
	private float targetRadius1;
	private float radius1;
	private Color orbitColor1;

	private GameObject OrbitCircle2;
	private float targetRadius2;
	private float radius2;
	private Color orbitColor2;

	private GameObject OrbitCircle3;
	private float targetRadius3;
	private float radius3;
	private Color orbitColor3;

	private Color targetColor;

	public float timeStep;
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	public CircleCollider2D col2d;

	public float ColliderRadiusMultiplier = 56;
	public float ThicknessMultiplier = 0.004f;








	void Start () {
		gameObject.GetComponentInChildren<Text>().text = AtomName;
		rb2d = GetComponent<Rigidbody2D>();
		sprite = GetComponent<SpriteRenderer>();
		col2d = GetComponent<CircleCollider2D>();
		chargeText = transform.Find("Charge").GetComponent<Text>();

		OrbitCircle1 = new GameObject { name = "Circle" };
		OrbitCircle1.transform.Translate(transform.position);
		OrbitCircle1.transform.parent = transform;

		OrbitCircle2 = new GameObject { name = "Circle" };
		OrbitCircle2.transform.Translate(transform.position);
		OrbitCircle2.transform.parent = transform;

		OrbitCircle3 = new GameObject { name = "Circle" };
		OrbitCircle3.transform.Translate(transform.position);
		OrbitCircle3.transform.parent = transform;

		StartCoroutine(FillInitElectrons());

		rb2d.mass = (float)InitElectrons;
	}

	void Update () {
		timeStep += Time.deltaTime;

		radius1 = Mathf.Lerp(radius1, targetRadius1, Time.deltaTime * BondsLibrary.LERP_SPEED);
		radius2 = Mathf.Lerp(radius2, targetRadius2, Time.deltaTime * BondsLibrary.LERP_SPEED);
		radius3 = Mathf.Lerp(radius3, targetRadius3, Time.deltaTime * BondsLibrary.LERP_SPEED);

		float scaler = (transform.localScale.x + transform.localScale.y) / 2f;

		col2d.radius = Mathf.Lerp(col2d.radius, Mathf.Max(new List<float>() { radius1, radius2, radius3, 0.1f }.ToArray()) * ColliderRadiusMultiplier, Time.deltaTime * BondsLibrary.LERP_SPEED);

		// -------------------------------------------------- Circle Radius Control
		// Old Method (FIXED)
		OrbitCircle1.DrawCircle(transform.position, radius1 * scaler * 0.963f, ThicknessMultiplier * scaler, orbitColor1);
		OrbitCircle2.DrawCircle(transform.position, radius2 * scaler * 0.985f, ThicknessMultiplier * scaler, orbitColor2);
		OrbitCircle3.DrawCircle(transform.position, radius3 * scaler * 0.995f, ThicknessMultiplier * scaler, orbitColor3);

		// New Method (DERIVED) - [[[[[Pending]]]]]
		


		sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * BondsLibrary.LERP_SPEED);

		foreach (GameObject atom in BoundTo) {
			rb2d.AddForce((atom.transform.position - transform.position).normalized * CouplingForceMultiplier);
		}
	}


	


	





	////////////////////////////////////////////////////////////////////// Utility
	IEnumerator FillInitElectrons() {
		while (GameController.Instance == null)
			yield return null;

		for (int i = 0; i < InitElectrons; i++) {
			GameObject electron = SpawnNewElectron();
			AddElectron(electron);
		}
	}

	// 0 to len-1
	int	LastIndex(GameObject[] array) {
		for (int i = 0; i < array.Length; i++) {
			if (array[i] == null) {
				return i - 1;
			}
		}
		return array.Length - 1;
	}
	int	CheckArrayFill(GameObject[] array) {
		if (LastIndex(array) == array.Length - 1)
			return 1;
		else if (LastIndex(array) == -1)
			return 0;
		else
			return -1;
	}
	GameObject Push(GameObject[] array, GameObject element) {
		int i = LastIndex(array);
		if (i < array.Length) {
			array[i + 1] = element;
			return element;
		} else {
			return null;
		}
	}
	GameObject Enqueue(GameObject[] array, GameObject element) {
		int i = LastIndex(array);
		if (i == array.Length - 1) return null;
		if (i == -1) {
			array[i + 1] = element;
			return element;
		}
		for (int j = i; j >=0; j--) {
			array[j + 1] = array[j];
		}
		array[0] = element;
		return element;
	}
	GameObject Pop(GameObject[] array) {
		int i = LastIndex(array);
		if (i > -1) {
			GameObject retVal = array[i];
			array[i] = null;
			return retVal;
		} else {
			return null;
		}
	}

	GameObject SpawnNewElectron() {
		GameObject electron = Instantiate(GameController.Instance.ElectronTemplate);
		//electron.transform.parent = gameObject.transform;

		Electron electronScript = electron.GetComponent<Electron>();
		electronScript.TargetRadius = 1f;
		electronScript.TargetPhaseDeg = 0f;
		electronScript.ParentAtom = gameObject;

		return electron;
	}
	public bool CheckAtomFull() {
		if (LastIndex(Orbit3) == Orbit3.Length - 1)
			return true;
		return false;
	}
	public bool CheckIfLastOrbitFull() {
		if (LastIndex(Orbit3) == Orbit3.Length - 1) {
			return true;
		} else if (LastIndex(Orbit3) == -1) {
			if (LastIndex(Orbit2) == Orbit2.Length - 1) {
				return true;
			} else if (LastIndex(Orbit2) == -1) {
				if (LastIndex(Orbit1) == Orbit1.Length - 1) {
					return true;
				}
			}
		}
		return false;
	}
	public bool CheckAtomEmpty() {
		if (LastIndex(Orbit1) == -1)
			return true;
		return false;
	}
	public bool AddElectron(GameObject electron) {
		electron.transform.parent = gameObject.transform;
		if (LastIndex(Orbit1) == Orbit1.Length - 1) {
			if (LastIndex(Orbit2) == Orbit2.Length - 1) {
				if (LastIndex(Orbit3) == Orbit3.Length - 1) {
					return false;
				} else {
					Push(Orbit3, electron);
				}
			} else {
				Push(Orbit2, electron);
			}
		} else {
			Push(Orbit1, electron);
		}

		electron.GetComponent<Electron>().ParentAtom = gameObject;
		RefreshAllOrbits();
		return true;
	}
	public bool AddElectronToStart(GameObject electron) {
		electron.transform.parent = gameObject.transform;
		if (LastIndex(Orbit1) == Orbit1.Length - 1) {
			if (LastIndex(Orbit2) == Orbit2.Length - 1) {
				if (LastIndex(Orbit3) == Orbit3.Length - 1) {
					return false;
				} else {
					Enqueue(Orbit3, electron);
				}
			} else {
				Enqueue(Orbit2, electron);
			}
		} else {
			Enqueue(Orbit1, electron);
		}

		electron.GetComponent<Electron>().ParentAtom = gameObject;
		RefreshAllOrbits();
		return true;
	}
	public GameObject PopElectron() {
		GameObject retVal;

		if (LastIndex(Orbit3) != -1) {
			retVal = Pop(Orbit3);
		} else {
			if (LastIndex(Orbit2) != -1) {
				retVal = Pop(Orbit2);
			} else {
				if (LastIndex(Orbit1) != -1) {
					retVal = Pop(Orbit1);
				} else {
					retVal = null;
				}
			}
		}

		RefreshAllOrbits();
		return retVal;
	}

	void RefreshAllOrbits() {
		// -------------------------------------------------- Electron Radius Control
		SetAutoOrbitConfig(ref Orbit1, ref targetRadius1, ref orbitColor1, 0.190f, 0.210f);
		SetAutoOrbitConfig(ref Orbit2, ref targetRadius2, ref orbitColor2, 0.250f, 0.300f);
		SetAutoOrbitConfig(ref Orbit3, ref targetRadius3, ref orbitColor3, 0.380f, 0.420f);
		UpdateChargeIndicators();
	}
	void SetAutoOrbitConfig(ref GameObject[] array, ref float targetRadius, ref Color orbitColor, float r1, float r2) {
		for (int i = 0; i < array.Length; i++) {
			if (array[i] != null) {
				float newRad = r1 + (r2 - r1) * LastIndex(array) / array.Length;
				Electron electron = array[i].GetComponent<Electron>();
				electron.TargetPhaseDeg = ((float)LastIndex(array) - i) * 360f / (LastIndex(array) + 1);
				electron.TargetRadius = newRad;
				electron.TargetRevSpeed = 3f - ((3f-0.3f) * newRad / 0.525f);	// 0.4 to 3.0
				targetRadius = newRad;


				// Need some very good brainboost to solve the "Last orbit color by covalent bonds" problem
				if (LastIndex(array) == array.Length - 1)
					ColorUtility.TryParseHtmlString(BondsLibrary.MDB_GREEN, out electron.TargetColor);
				else
					ColorUtility.TryParseHtmlString(BondsLibrary.MDB_RED, out electron.TargetColor);
			}
		}

		if (LastIndex(array) == array.Length - 1)
			ColorUtility.TryParseHtmlString(BondsLibrary.MDB_GREEN, out orbitColor);
		else if (LastIndex(array) == -1)
			targetRadius = 0;
		else
			ColorUtility.TryParseHtmlString(BondsLibrary.MDB_RED, out orbitColor);
	}
	void UpdateChargeIndicators() {
		switch (charge) {
			case 3:
				chargeText.text = "+++";
				break;
			case 2:
				chargeText.text = "++";
				break;
			case 1:
				chargeText.text = "+";
				break;
			case 0:
				chargeText.text = "";
				break;
			case -1:
				chargeText.text = "-";
				break;
			case -2:
				chargeText.text = "--";
				break;
			case -3:
				chargeText.text = "---";
				break;
		}

		switch (charge) {
			case 3:
			case 2:
			case 1:
				ColorUtility.TryParseHtmlString(BondsLibrary.MDB_POSITIVE, out targetColor);
				break;
			case 0:
				ColorUtility.TryParseHtmlString(BondsLibrary.MDB_DARK_GREY, out targetColor);
				break;
			case -1:
			case -2:
			case -3:
				ColorUtility.TryParseHtmlString(BondsLibrary.MDB_NEGATIVE, out targetColor);
				break;
		}
	}

	public void AddBinding(GameObject atom) {
		BoundTo.Add(atom);
	}
	////////////////////////////////////////////////////////////////////// Utility
}
