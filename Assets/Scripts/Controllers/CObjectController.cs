using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DangerousTown3D {
	public class CObjectController : MonoBehaviour {

		[Header("Collider")]
		[SerializeField]	protected Collider m_Collider;

		protected Transform m_Transform;

		protected virtual void Awake() {
			this.m_Transform = this.transform;
		}

		protected virtual void Start() {
		
		}

		protected virtual void Update() {
		
		}

		protected virtual void LateUpdate() {
			
		}

		public virtual Vector3 GetNearestPoint(Vector3 sample) {
			if (this.m_Collider == null)
				return this.m_Transform.position;
			return this.m_Collider.ClosestPointOnBounds (sample);
		}

		public virtual Vector3 GetPosition() {
			return this.m_Transform.position;
		}

		public virtual void SetPosition(Vector3 value) {
			this.m_Transform.position = value;
		}

		public virtual CObjectController GetTarget() {
			return null;
		}
	
	}
}
