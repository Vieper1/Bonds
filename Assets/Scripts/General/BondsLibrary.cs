using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BondsLibrary {

	public static string MDB_GREEN = "#1de9b6";
	public static string MDB_RED = "#ec407a";
	public static string MDB_LIGHT_GREY = "#90A4AE";
	public static string MDB_LIGHT_BLUE = "#009FB7";
	public static string MDB_GREY = "#607d8b";
	public static string MDB_DARK_GREY = "#232323";
	public static string MDB_POSITIVE = "#ff4444";
	public static string MDB_NEGATIVE = "#009FB7";
	public static string MDB_YELLOW = "#FED766";
	


	public static float LERP_SPEED = 5f;
	public static float E_LERP_SPEED = 0.1f;


	public static void DrawCircle(this GameObject container, Vector3 centre, float radius, float lineWidth, Color color) {
		var segments = 360;
		var line = container.GetComponent<LineRenderer>();
		if (line == null)
			line = container.AddComponent<LineRenderer>();
		line.useWorldSpace = true;
		//line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		line.material = (Material)Resources.Load("LineMaterial");
		line.startColor = color;
		line.endColor = color;
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		line.positionCount = segments + 1;

		var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
		var points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++) {
			var rad = Mathf.Deg2Rad * (i * 360f / segments);
			points[i] = centre + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
		}

		line.SetPositions(points);
	}

	public static float DegToRad(float x) {
		return x * Mathf.PI / 180f;
	}

	// Main
	public enum BondType {
		Ionic,
		Covalent_Mutual,
		Covalent_LonePair
	}

	[System.Serializable]
	public class Bond : IEquatable<Bond> {
		public GameObject fromAtom;
		public GameObject toAtom;
		public BondType bondType;
		public int n = 1;

		public bool Equals(Bond other) {
			if (other.fromAtom.name.Equals(fromAtom.name) && other.toAtom.name.Equals(toAtom.name) && other.bondType.Equals(bondType)) {
				return true;
			}
			return false;
		}
		
		public override string ToString() {
			return fromAtom.name + " " + toAtom.name + " " + bondType + " " + n;
		}

		public Bond GetReverseBond() {
			Bond b = new Bond();
			b.n = n;
			b.fromAtom = toAtom;
			b.toAtom = fromAtom;
			b.bondType = bondType;
			return b;
		}
	}



	// Comparers
	public class RigidBodyComparer : IEqualityComparer<Rigidbody2D[]> {
		public bool Equals(Rigidbody2D[] x, Rigidbody2D[] y) {
			string name1 = "";
			string name2 = "";

			foreach (Rigidbody2D rb2d in x)
				name1 += rb2d.name;
			foreach (Rigidbody2D rb2d in y)
				name2 += rb2d.name;

			return StringComparer.InvariantCulture.Equals(name1, name2);
		}

		public int GetHashCode(Rigidbody2D[] obj) {
			string name = "";
			foreach (Rigidbody2D rb in obj)
				name += rb.name;

			return StringComparer.InvariantCulture.GetHashCode(name);
		}
	}

	public static string GetNextLevelName(string levelName) {
		string levelType = levelName.Split('-')[0];
		int levelNumber = Convert.ToInt16(levelName.Split('-')[1]) + 1;
		return levelType + "-" + levelNumber;
	}

	//public class BondComparer : IEqualityComparer<Bond> {
	//	public bool Equals(Bond x, Bond y) {
	//		return (x.fromAtom == y.fromAtom && x.toAtom == y.toAtom && x.bondType == y.bondType)/* || (x.fromAtom == y.toAtom && x.toAtom == y.fromAtom && x.bondType == y.bondType)*/;
	//	}

	//	public int GetHashCode(Bond obj) {
	//		return StringComparer.InvariantCulture.GetHashCode(obj.fromAtom.name + obj.toAtom.name + obj.bondType);
	//	}
	//}
}
