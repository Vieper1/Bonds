using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electron : MonoBehaviour {

	public static float SeparationDistance = -0.10f;
	public static float ColumnDistance = 0.32f;
	
	// For General Electrons
	public float TargetRadius;
	public float TargetPhaseDeg;
	public float TargetRevSpeed;
	public Color TargetColor;
	public GameObject ParentAtom;
	
	public float radius = 100;
	private float phase;
	private float revSpeed;
	private SpriteRenderer sprite;

	// For Covalently Shared Electrons
	public bool isShared;
	public int eNumber;
	public int mutualPairBondNumber;
	public int lonePairBondNumber;
	public GameObject SharedAtom1;
	public GameObject SharedAtom2;







	void Start () {
		sprite = GetComponent<SpriteRenderer>();
	}

	
	void Update () {
		sprite.color = Color.Lerp(sprite.color, TargetColor, Time.deltaTime * BondsLibrary.LERP_SPEED);
		transform.localScale = Vector3.one * 0.13f;

		if (!isShared) {
			radius = Mathf.Lerp(radius, TargetRadius, Time.deltaTime * BondsLibrary.LERP_SPEED);
			phase = Mathf.Lerp(phase, TargetPhaseDeg, Time.deltaTime * BondsLibrary.LERP_SPEED);
			revSpeed = Mathf.Lerp(revSpeed, TargetRevSpeed, Time.deltaTime * BondsLibrary.LERP_SPEED);
			float scaler = (transform.parent.localScale.x + transform.parent.localScale.y) / 2f;

			Vector3 targetPosition = ParentAtom.transform.position +
				new Vector3(
					radius * scaler * Mathf.Sin(((ParentAtom.GetComponent<Atom>().timeStep * revSpeed) + BondsLibrary.DegToRad(phase)) % (2 * Mathf.PI)),
					radius * scaler * Mathf.Cos(((ParentAtom.GetComponent<Atom>().timeStep * revSpeed) + BondsLibrary.DegToRad(phase)) % (2 * Mathf.PI)),
					0f
				);

			transform.position = Vector3.Lerp(transform.position, targetPosition, BondsLibrary.E_LERP_SPEED);
		} else {
			//Vector3 halfDistVector = (SharedAtom2.transform.position - SharedAtom1.transform.position) / 2;
			Vector3 offsetDirection = (SharedAtom2.transform.position - SharedAtom1.transform.position).normalized;

			float offsetCorrection = SharedAtom1.transform.localScale.x * 0.023f;
			Vector3 targetPosition =
				SharedAtom1.transform.position +
				offsetDirection * SharedAtom1.GetComponent<CircleCollider2D>().radius * offsetCorrection +
				offsetDirection * (
					(SharedAtom2.transform.position - SharedAtom1.transform.position).magnitude -
					SharedAtom1.GetComponent<CircleCollider2D>().radius * offsetCorrection -
					SharedAtom2.GetComponent<CircleCollider2D>().radius * offsetCorrection) / 2;

			//Vector3 targetPosition = SharedAtom1.transform.position + halfDistVector;
			if (eNumber == 1) targetPosition = targetPosition + offsetDirection * SeparationDistance;
			else if (eNumber == 2) targetPosition = targetPosition - offsetDirection * SeparationDistance;

			if (mutualPairBondNumber != 0) {
				targetPosition = targetPosition + Quaternion.AngleAxis(90, Vector3.back) * offsetDirection * (ColumnDistance * (mutualPairBondNumber - 1) - ColumnDistance / 2);
			}

			transform.position = Vector3.Lerp(transform.position, targetPosition, BondsLibrary.E_LERP_SPEED);
		}
		
	}
}
