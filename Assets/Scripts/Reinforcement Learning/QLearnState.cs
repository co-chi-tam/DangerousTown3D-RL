using System;
using UnityEngine;

[Serializable]
public class QLearnState {
	
	public QLearnState() : this(0f, 0f, 0f) {
		
	}

	public QLearnState(float point1, float point2, float point3) {
		pointDetect1 = point1.Round2();
		pointDetect2 = point2.Round2();
		pointDetect3 = point3.Round2();
	}

	public float pointDetect1 { get; set; }
	public float pointDetect2 { get; set; }
	public float pointDetect3 { get; set; }

	public override bool Equals(object rhs) {
		QLearnState rhState = rhs as QLearnState;

		if (rhState != null) {
			return pointDetect1 == rhState.pointDetect1 
				&& pointDetect2 == rhState.pointDetect2 
				&& pointDetect3 == rhState.pointDetect3;
		}

		return false;
	}

	public override int GetHashCode() {
		// Reference: http://stackoverflow.com/questions/11795104/is-the-hashcode-function-generated-by-eclipse-any-good
		const int prime = 31;
		int hashCode = 1;

		hashCode = prime * hashCode + pointDetect1.GetHashCode() + pointDetect2.GetHashCode() + pointDetect3.GetHashCode();

		return hashCode;
	}

	public override string ToString() {
		return string.Format("[QLearnState: pointDetect1={0} + pointDetect2={1} + pointDetect3={2}]", pointDetect1, pointDetect2, pointDetect3);
	}

	public QLearnState Clone() {
		return new QLearnState(pointDetect1, pointDetect2, pointDetect3);
	}
}
