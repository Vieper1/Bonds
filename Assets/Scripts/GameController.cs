using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static float REVOLUTION_SPEED = 1.5f;
	public static float ELECTRON_FORCE_MAG = 1.0f;
	public static GameController Instance;

	public Camera MainCamera;
	public GameObject ElectronTemplate;
	public string TargetCompoundName;
	public List<BondsLibrary.Bond> TargetState;

	public string Hint = "";
	public float GoldTime = 6f;
	public float SilverTime = 12f;
	public float BronzeTime = 18f;
	public float GameTime = 0f;

	private List<BondsLibrary.Bond> currentState = new List<BondsLibrary.Bond>();
	private bool isDragging;
	private Vector2 startPos;
	private Atom fromAtom;
	private Vector2 endPos;
	private Atom toAtom;

	// Star Timer
	private Text starTimerText;
	private Image starTimerStar1;
	private Image starTimerStar2;
	private Image starTimerStar3;

	// Moves
	private int moveCount;
	private int targetMoveCount;
	
	// Game Over Event
	public delegate void GameOver();
	public event GameOver OnGameOver;
	public bool isGameOver;


	// Miscellaneous
	public bool isIntro;






	void Start() {
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);
		GameTime = 0;
		GenerateCollidersAroundScreen();
		try {
			foreach (BondsLibrary.Bond bond in TargetState) {
				targetMoveCount += bond.n;
			}
			starTimerText = transform.Find("StarTimer").GetComponent<Text>();
			foreach (Image star in transform.Find("StarTimer").GetComponentsInChildren<Image>()) {
				star.color = LevelsList.LevelTypeColor;
			}
			transform.Find("MoveCount").GetComponent<Text>().text = moveCount + "/" + targetMoveCount;
			transform.Find("HintPanel/HintContainer/Hint").GetComponent<Text>().text = Hint;
			//GoogleAds.RequestInterstitialAd();

			if (PlayerPrefs.GetInt("FxaaEnabled", 1) == 1) {
				GameObject.Find("MainCamera").GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.None;
			} else {
				GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				GameObject.Find("Main Camera").GetComponent<PostProcessLayer>().fastApproximateAntialiasing.fastMode = true;
			}
		} catch (Exception e) {
			// Nothing
		}
	}

	void Update() {
		if (!isGameOver) {
			GameTime += Time.deltaTime;
			UpdateStarTimer();
			CheckTouchDrag();
		}
		ApplySeparationForce();

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (!SceneManager.GetActiveScene().name.Equals("MainMenu")) {
				SceneManager.LoadSceneAsync("MainMenu");
			}
		}
	}


	void ApplySeparationForce() {
		List<GameObject> atomArray = new List<GameObject>();
		foreach (BondsLibrary.Bond bond in currentState) {
			if (!atomArray.Contains(bond.fromAtom)) atomArray.Add(bond.fromAtom);
			if (!atomArray.Contains(bond.toAtom)) atomArray.Add(bond.toAtom);
		}

		for (int i = 0; i < atomArray.Count - 1; i++) {
			for (int j = i + 1; j < atomArray.Count; j++) {
				Vector3 dist = atomArray[j].transform.position - atomArray[i].transform.position;
				atomArray[j].GetComponent<Rigidbody2D>().AddForce(dist.normalized * Atom.PushingForceMultiplier / dist.sqrMagnitude);
				atomArray[i].GetComponent<Rigidbody2D>().AddForce(dist.normalized * -1f * Atom.PushingForceMultiplier / dist.sqrMagnitude);
			}
		}
	}




	// Atoms
	int fingerId = -1;
	bool covalency = false;
	Atom touchedAtom;
	bool secondTouch = false;
	void CheckTouchDrag() {
		// For testing with mouse
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			if (!isDragging) {
				isDragging = true;
				startPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.zero, 0f);
				if (hit) {
					fromAtom = hit.transform.GetComponent<Atom>();
				}
			}
		}
		if (Input.GetMouseButtonUp(0)) {
			if (isDragging) {
				isDragging = false;
				endPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(endPos, Vector2.zero, 0f);
				if (hit) {
					toAtom = hit.transform.GetComponent<Atom>();
					CreateIonicBond(ref fromAtom, ref toAtom);
				}

			}
		}
		if (Input.GetMouseButtonDown(1)) {
			if (!isDragging) {
				isDragging = true;
				startPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.zero, 0f);
				if (hit) {
					fromAtom = hit.transform.GetComponent<Atom>();
				}
			}
		}
		if (Input.GetMouseButtonUp(1)) {
			if (isDragging) {
				isDragging = false;
				endPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(endPos, Vector2.zero, 0f);
				if (hit) {
					toAtom = hit.transform.GetComponent<Atom>();
					CreateCovalentBond(ref fromAtom, ref toAtom);
				}

			}
		}
		if (Input.GetMouseButtonDown(2)) {
			if (!isDragging) {
				isDragging = true;
				startPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.zero, 0f);
				if (hit) {
					fromAtom = hit.transform.GetComponent<Atom>();
				}
			}
		}
		if (Input.GetMouseButtonUp(2)) {
			if (isDragging) {
				isDragging = false;
				endPos = new Vector2(MainCamera.ScreenToWorldPoint(Input.mousePosition).x, MainCamera.ScreenToWorldPoint(Input.mousePosition).y);

				RaycastHit2D hit = Physics2D.Raycast(endPos, Vector2.zero, 0f);
				if (hit) {
					toAtom = hit.transform.GetComponent<Atom>();
					CreateLonePairBond(ref fromAtom, ref toAtom);
				}

			}
		}
#endif

		// For touches on mobile
		foreach (Touch touch in Input.touches) {
			
			// Touch Began
			if (touch.phase == TouchPhase.Began) {
				RaycastHit2D hit = Physics2D.Raycast(
					new Vector2(MainCamera.ScreenToWorldPoint(touch.position).x, MainCamera.ScreenToWorldPoint(touch.position).y),
					Vector2.zero,
					0f
				);
				if (hit) {
					fromAtom = hit.transform.GetComponent<Atom>();
					fingerId = touch.fingerId;
					try { if (transform.Find("MoveText").GetComponent<Text>().text.Equals("")) transform.Find("MoveText").GetComponent<Text>().text = "Ionic"; } catch (Exception) { };
				} else {
					StopAllCoroutines();
					ResetTouch();
				}
			}

			// Touch Ended
			if (touch.phase == TouchPhase.Ended && touch.fingerId == fingerId) {
				try { if (transform.Find("MoveText").GetComponent<Text>().text.Equals("Ionic")) transform.Find("MoveText").GetComponent<Text>().text = ""; } catch (Exception) { };
				RaycastHit2D hit = Physics2D.Raycast(
					new Vector2(MainCamera.ScreenToWorldPoint(touch.position).x, MainCamera.ScreenToWorldPoint(touch.position).y),
					Vector2.zero,
					0f
				);
				if (hit) {
					toAtom = hit.transform.GetComponent<Atom>();
					if (toAtom == fromAtom) {				// TAP
						if (!covalency) {                   // First Touch
							StopAllCoroutines();
							ResetTouch();
							StartCoroutine(TouchTimer());
							touchedAtom = hit.transform.GetComponent<Atom>();
							try { transform.Find("MoveText").GetComponent<Text>().text = "Mutual Pair"; } catch (Exception) { };
						} else {                            // (>1st) touch
							if (!secondTouch && touchedAtom != toAtom) {
								CreateCovalentBond(ref touchedAtom, ref toAtom);
								StopAllCoroutines();
								ResetTouch();
							}
							if (!secondTouch && touchedAtom == toAtom) {
								secondTouch = true;
								try { transform.Find("MoveText").GetComponent<Text>().text = "Lone Pair"; } catch (Exception) { };
							}
							if (secondTouch && touchedAtom != toAtom) {
								CreateLonePairBond(ref touchedAtom, ref toAtom);
								StopAllCoroutines();
								ResetTouch();
							}
						}
					} else {
						CreateIonicBond(ref fromAtom, ref toAtom);
					}
					fingerId = -1;
					fromAtom = null;
					toAtom = null;
				}
			}
		}
	}
	IEnumerator TouchTimer() {
		covalency = true;
		yield return new WaitForSeconds(3f);
		Debug.Log("TouchTimer Ended!");
		ResetTouch();
	}
	void ResetTouch() {
		touchedAtom = null;
		covalency = false;
		secondTouch = false;
		try { transform.Find("MoveText").GetComponent<Text>().text = ""; } catch (Exception) { };
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////// IONIC BOND
	void CreateIonicBond(ref Atom from, ref Atom to) {
		if (moveCount + 1 > targetMoveCount) return;
		if (!to) return;
		if (to == from) return;
		if (from.zeroElectronGenerosity) return;
		if (to.CheckIfLastOrbitFull()) return;
		if (to.CheckAtomFull()) return;
		if (from.CheckAtomEmpty()) return;

		BondsLibrary.Bond bond = new BondsLibrary.Bond() {
			fromAtom = from.gameObject,
			toAtom = to.gameObject,
			bondType = BondsLibrary.BondType.Ionic
		};

		BondsLibrary.Bond reverseBond = new BondsLibrary.Bond() {
			fromAtom = to.gameObject,
			toAtom = from.gameObject,
			bondType = BondsLibrary.BondType.Ionic
		};


		if (!currentState.Contains(reverseBond)) {
			from.charge++;
			to.charge--;
			GameObject electron;
			// Success
			if (electron = from.PopElectron()) {
				to.AddElectron(electron);
				IncrementMove();
			}

			if (!currentState.Contains(bond)) {
				currentState.Add(bond);
				bond.fromAtom.GetComponent<Atom>().AddBinding(to.gameObject);
				bond.toAtom.GetComponent<Atom>().AddBinding(from.gameObject);
			} else {
				currentState.Find(x => x.Equals(bond)).n++;
			}
		}

		if (CheckIfTargetState()) {
			isGameOver = true;
			transform.Find("NextButton").gameObject.SetActive(true);
			transform.Find("NextButton").GetComponent<Button>().onClick.AddListener(OnNextPressed);
		} else if (targetMoveCount == moveCount) {
			transform.Find("RestartIndicator").GetComponent<Animation>().Play();
		}
	}
	//////////////////////////////////////////////////////////////////////////////////////////////////// MUTUAL PAIR BOND
	void CreateCovalentBond(ref Atom atom1, ref Atom atom2) {
		if (isIntro) return;
		if (moveCount + 1 > targetMoveCount) return;
		if (!atom2) return;
		if (atom2 == atom1) return;
		if (atom1.zeroElectronGenerosity) return;
		if (atom1.CheckAtomEmpty() || atom2.CheckAtomEmpty()) return;
		if (atom1.CheckIfLastOrbitFull() || atom2.CheckIfLastOrbitFull()) return;

		BondsLibrary.Bond bond = new BondsLibrary.Bond() {
			fromAtom = atom1.gameObject,
			toAtom = atom2.gameObject,
			bondType = BondsLibrary.BondType.Covalent_Mutual
		};

		GameObject e1;
		GameObject e2;
		if (e1 = atom1.PopElectron()) {
			if (e2 = atom2.PopElectron()) {
				IncrementMove();
				atom1.AddElectronToStart(e1); atom2.AddElectronToStart(e1);
				atom1.AddElectronToStart(e2); atom2.AddElectronToStart(e2);

				Electron eScript1 = e1.GetComponent<Electron>();
				eScript1.isShared = true;
				eScript1.eNumber = 1;
								
				Electron eScript2 = e2.GetComponent<Electron>();
				eScript2.isShared = true;
				eScript2.eNumber = 2;
				

				if (currentState.Contains(bond) || currentState.Contains(bond.GetReverseBond())) {
					BondsLibrary.Bond tempBond = currentState.Find(x => x.Equals(bond) || x.Equals(bond.GetReverseBond()));
					tempBond.n++;
					eScript1.mutualPairBondNumber = tempBond.n; eScript2.mutualPairBondNumber = tempBond.n;

					eScript1.SharedAtom1 = tempBond.fromAtom; eScript1.SharedAtom2 = tempBond.toAtom;
					eScript2.SharedAtom1 = tempBond.fromAtom; eScript2.SharedAtom2 = tempBond.toAtom;
				} else {
					currentState.Add(bond);
					eScript1.mutualPairBondNumber = 1; eScript2.mutualPairBondNumber = 1;
					bond.fromAtom.GetComponent<Atom>().AddBinding(atom2.gameObject);
					bond.toAtom.GetComponent<Atom>().AddBinding(atom1.gameObject);

					eScript1.SharedAtom1 = atom1.gameObject; eScript1.SharedAtom2 = atom2.gameObject;
					eScript2.SharedAtom1 = atom1.gameObject; eScript2.SharedAtom2 = atom2.gameObject;
				}
			}
		}

		if (CheckIfTargetState()) {
			isGameOver = true;
			transform.Find("NextButton").gameObject.SetActive(true);
			transform.Find("NextButton").GetComponent<Button>().onClick.AddListener(OnNextPressed);
		} else if (targetMoveCount == moveCount) {
			transform.Find("RestartIndicator").GetComponent<Animation>().Play();
		}
	}
	//////////////////////////////////////////////////////////////////////////////////////////////////// LONE PAIR BOND
	void CreateLonePairBond(ref Atom from, ref Atom to) {
		if (isIntro) return;
		if (moveCount + 1 > targetMoveCount) return;
		if (!to) return;
		if (to == from) return;
		if (from.zeroElectronGenerosity) return;
		if (from.CheckAtomEmpty()) return;

		BondsLibrary.Bond bond = new BondsLibrary.Bond() {
			fromAtom = from.gameObject,
			toAtom = to.gameObject,
			bondType = BondsLibrary.BondType.Covalent_LonePair
		};

		GameObject e1;
		GameObject e2;
		if (e1 = from.PopElectron()) {
			if (e2 = from.PopElectron()) {
				IncrementMove();
				from.AddElectronToStart(e1); to.AddElectronToStart(e1);
				from.AddElectronToStart(e2); to.AddElectronToStart(e2);

				Electron eScript1 = e1.GetComponent<Electron>();
				eScript1.isShared = true;
				eScript1.eNumber = 1;

				Electron eScript2 = e2.GetComponent<Electron>();
				eScript2.isShared = true;
				eScript2.eNumber = 2;


				if (currentState.Contains(bond) || currentState.Contains(bond.GetReverseBond())) {
					return;
				} else {
					currentState.Add(bond);
					eScript1.lonePairBondNumber = 1; eScript2.lonePairBondNumber = 1;
					bond.fromAtom.GetComponent<Atom>().AddBinding(to.gameObject);
					bond.toAtom.GetComponent<Atom>().AddBinding(from.gameObject);

					eScript1.SharedAtom1 = from.gameObject; eScript1.SharedAtom2 = to.gameObject;
					eScript2.SharedAtom1 = from.gameObject; eScript2.SharedAtom2 = to.gameObject;
				}
			}
		}


		if (CheckIfTargetState()) {
			isGameOver = true;
			transform.Find("NextButton").gameObject.SetActive(true);
			transform.Find("NextButton").GetComponent<Button>().onClick.AddListener(OnNextPressed);
		} else if (targetMoveCount == moveCount) {
			transform.Find("RestartIndicator").GetComponent<Animation>().Play();
		}
	}


	// NEXT BUTTON
	public void OnNextPressed() {
		Debug.Log("Viper");
		OnGameOver();
		transform.Find("NextButton").gameObject.SetActive(false);
	}


	// Most Valuable Function
	bool CheckIfTargetState() {
		foreach (BondsLibrary.Bond bond in TargetState) {
			Debug.Log(bond.fromAtom.GetComponent<Atom>().AtomName + " " + bond.toAtom.GetComponent<Atom>().AtomName + " | " + GetCountInState(bond, TargetState) + " " + GetCountInState(bond, currentState));
			if (GetCountInState(bond, TargetState) != GetCountInState(bond, currentState))
				return false;
		}
		Debug.Log("----------");
		return true;
	}

	bool FindBondInState(BondsLibrary.Bond bond, List<BondsLibrary.Bond> state) {
		// Cannot use Bond.Equals() from BondLibrary because of a different functionality requirement
		foreach (BondsLibrary.Bond targetBond in state) {
			if (bond.bondType == BondsLibrary.BondType.Ionic || bond.bondType == BondsLibrary.BondType.Covalent_LonePair) {
				if (bond.n == targetBond.n)
					if (targetBond.fromAtom.name.Equals(bond.fromAtom.name) && 
						targetBond.toAtom.name.Equals(bond.toAtom.name) && 
						targetBond.bondType.Equals(bond.bondType))
						return true;
			} else if (bond.bondType == BondsLibrary.BondType.Covalent_Mutual) {
				if (bond.n == targetBond.n)
					if ((targetBond.fromAtom.name.Equals(bond.fromAtom.name) &&
						targetBond.toAtom.name.Equals(bond.toAtom.name) &&
						targetBond.bondType.Equals(bond.bondType))
						||
						(targetBond.GetReverseBond().fromAtom.name.Equals(bond.fromAtom.name) &&
						targetBond.GetReverseBond().toAtom.name.Equals(bond.toAtom.name) &&
						targetBond.GetReverseBond().bondType.Equals(bond.bondType)))
						return true;
			}
		}
		return false;
	}

	int GetCountInState(BondsLibrary.Bond bond, List<BondsLibrary.Bond> state) {
		// Cannot use Bond.Equals() from BondLibrary because of a different functionality requirement
		int count = 0;
		foreach (BondsLibrary.Bond targetBond in state) {
			if (bond.bondType == BondsLibrary.BondType.Ionic || bond.bondType == BondsLibrary.BondType.Covalent_LonePair) {
				if (bond.n == targetBond.n)
					if (targetBond.fromAtom.GetComponent<Atom>().AtomName.Equals(bond.fromAtom.GetComponent<Atom>().AtomName) &&
						targetBond.toAtom.GetComponent<Atom>().AtomName.Equals(bond.toAtom.GetComponent<Atom>().AtomName) &&
						targetBond.bondType.Equals(bond.bondType))
						count++;
			} else if (bond.bondType == BondsLibrary.BondType.Covalent_Mutual) {
				if (bond.n == targetBond.n)
					if ((targetBond.fromAtom.GetComponent<Atom>().AtomName.Equals(bond.fromAtom.GetComponent<Atom>().AtomName) &&
						targetBond.toAtom.GetComponent<Atom>().AtomName.Equals(bond.toAtom.GetComponent<Atom>().AtomName) &&
						targetBond.bondType.Equals(bond.bondType))
						||
						(targetBond.GetReverseBond().fromAtom.GetComponent<Atom>().AtomName.Equals(bond.fromAtom.GetComponent<Atom>().AtomName) &&
						targetBond.GetReverseBond().toAtom.GetComponent<Atom>().AtomName.Equals(bond.toAtom.GetComponent<Atom>().AtomName) &&
						targetBond.GetReverseBond().bondType.Equals(bond.bondType)))
						count++;
			}
		}
		return count;
	}





	// General
	void IncrementMove() {
		try {
			moveCount++;
			transform.Find("MoveCount").GetComponent<Text>().text = moveCount + "/" + targetMoveCount;
			//transform.Find("MoveCount").GetComponent<Text>().text = (moveCount - targetMoveCount) + " moves left";
		} catch (Exception e) {
			// Nothing
		}
	}
	void UpdateStarTimer() {
		int ss = ((int)GameTime) % 60;
		int mm = ((int)GameTime) / 60;

		try {
			starTimerText.text = mm.ToString("00") + ":" + ss.ToString("00");
			if (GameTime < GoldTime) {
				// Nothing
			} else if (GameTime < SilverTime) {
				transform.Find("StarTimer/Star3").GetComponent<Image>().color = Color.Lerp(transform.Find("StarTimer/Star3").GetComponent<Image>().color, new Color(1, 1, 1, 0), Time.deltaTime * BondsLibrary.LERP_SPEED);
			} else if (GameTime < BronzeTime) {
				transform.Find("StarTimer/Star2").GetComponent<Image>().color = Color.Lerp(transform.Find("StarTimer/Star2").GetComponent<Image>().color, new Color(1, 1, 1, 0), Time.deltaTime * BondsLibrary.LERP_SPEED);
			} else {
				transform.Find("StarTimer/Star1").GetComponent<Image>().color = Color.Lerp(transform.Find("StarTimer/Star1").GetComponent<Image>().color, new Color(1, 1, 1, 0), Time.deltaTime * BondsLibrary.LERP_SPEED);
			}
		} catch (Exception e) {
			// Nothing
		}
	}
	void GenerateCollidersAroundScreen() {
		Vector2 lDCorner = MainCamera.ViewportToWorldPoint(new Vector3(0, 0f, MainCamera.nearClipPlane));
		Vector2 rUCorner = MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, MainCamera.nearClipPlane));
		Vector2[] colliderpoints;

		EdgeCollider2D upperEdge = new GameObject("upperEdge").AddComponent<EdgeCollider2D>();
		colliderpoints = upperEdge.points;
		colliderpoints[0] = new Vector2(lDCorner.x, rUCorner.y);
		colliderpoints[1] = new Vector2(rUCorner.x, rUCorner.y);
		upperEdge.points = colliderpoints;

		EdgeCollider2D lowerEdge = new GameObject("lowerEdge").AddComponent<EdgeCollider2D>();
		colliderpoints = lowerEdge.points;
		colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
		colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
		lowerEdge.points = colliderpoints;

		EdgeCollider2D leftEdge = new GameObject("leftEdge").AddComponent<EdgeCollider2D>();
		colliderpoints = leftEdge.points;
		colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
		colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
		leftEdge.points = colliderpoints;

		EdgeCollider2D rightEdge = new GameObject("rightEdge").AddComponent<EdgeCollider2D>();
		colliderpoints = rightEdge.points;
		colliderpoints[0] = new Vector2(rUCorner.x, rUCorner.y);
		colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
		rightEdge.points = colliderpoints;
	}
	bool isBackPressed;
	public void HomeButtonPressed() {
		SceneManager.LoadSceneAsync("MainMenu");
	}
	IEnumerator HomePressedCoroutine() {
		isBackPressed = true;
		SSTools.ShowMessage("Press again to exit!", SSTools.Position.bottom, SSTools.Time.oneSecond);
		yield return new WaitForSeconds(2f);
		isBackPressed = false;
	}
	public void RetryButtonPressed() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void HintButtonPressed() {
		Animator hintAnim = transform.Find("HintPanel/HintContainer").GetComponent<Animator>();
		hintAnim.SetBool("isActive", !hintAnim.GetBool("isActive"));
	}
	public void HintCloseButtonPressed() {
		Animator hintAnim = transform.Find("HintPanel/HintContainer").GetComponent<Animator>();
		hintAnim.SetBool("isActive", false);
	}
}