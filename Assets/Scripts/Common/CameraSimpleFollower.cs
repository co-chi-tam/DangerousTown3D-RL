using UnityEngine;
using System;
using System.Collections;
using SimpleSingleton;

namespace SimpleUtil {
	public class CameraSimpleFollower : CMonoSingleton<CameraSimpleFollower> {

	    public Transform target;	
		public Vector3 offsetPosition = Vector3.zero;
		public AnimationCurve speedCurve;
		public float speed = 5f;
	
		private float m_SpeedDetailCurve;
		protected Transform m_Transform;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_Transform = this.transform;
		}

		private void Update() {
			if (target == null)
				return;
			var targetPosition = target.transform.position + offsetPosition;
			var direction = targetPosition - m_Transform.position;
			if (direction.sqrMagnitude < 0.01f) {
				return;
			}
			var movePosition = direction.normalized * speed * Time.deltaTime;
			m_Transform.position += movePosition * speedCurve.Evaluate (m_SpeedDetailCurve);
			m_SpeedDetailCurve += speed * Time.deltaTime;
		}
	}
}