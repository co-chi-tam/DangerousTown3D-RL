using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DangerousTown3D {
	public class CTargetPointController : CObjectController {

		[SerializeField]	protected float m_RandomRange = 1000f;

		protected virtual void OnTriggerEnter(Collider coll) {
			if (coll.gameObject.tag == "AI") {
				this.NewRandomPoint();
			}
		}

		protected void NewRandomPoint() {
			var randomPoint = Random.insideUnitCircle;
			var random = randomPoint * this.m_RandomRange;
			this.m_Transform.position = new Vector3 (random.x, 0f, random.y);
		}
		
	}
}
