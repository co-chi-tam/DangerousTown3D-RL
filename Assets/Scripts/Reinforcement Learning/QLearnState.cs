﻿using System;
using UnityEngine;

[Serializable]
public class QLearnState {
	
	public QLearnState() : this(Vector3.zero, Vector3.zero) {
		
	}

	public QLearnState(Vector3 point1, Vector3 point2) {
		pointDetect1 = point1.ToV3Round2();
		pointDetect2 = point2.ToV3Round2();
	}

	public Vector3 pointDetect1 { get; set; }
	public Vector3 pointDetect2 { get; set; }

	public override bool Equals(object rhs) {
		QLearnState rhState = rhs as QLearnState;

		if (rhState != null) {
			return pointDetect1 == rhState.pointDetect1 
				&& pointDetect2 == rhState.pointDetect2;
		}

		return false;
	}

	public override int GetHashCode() {
		// Reference: http://stackoverflow.com/questions/11795104/is-the-hashcode-function-generated-by-eclipse-any-good
		const int prime = 31;
		int hashCode = 1;

		hashCode = prime * hashCode + pointDetect1.GetHashCode ();
		hashCode = prime * hashCode + pointDetect2.GetHashCode ();

		return hashCode;
	}

	public override string ToString() {
		return string.Format("[QLearnState: pointDetect1={0} " +
			"+ pointDetect2={1}]", 
			pointDetect1, pointDetect2);
	}

	public QLearnState Clone() {
		return new QLearnState(pointDetect1, pointDetect2);
	}
}
