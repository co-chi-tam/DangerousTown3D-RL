using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DangerousTown3D {
	public class CPlayerController : CObjectController {

		[SerializeField]	protected GameObject m_Point;
		[SerializeField]	protected float m_MoveSpeed = 5f;
		[SerializeField]	protected float m_RotationSpeed = 5f;

		protected float m_CurrentAxis;

		protected override void Awake ()
		{
			base.Awake ();
			Application.targetFrameRate = 60;
		}

		protected override void Update() {
			base.Update ();
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN
			this.StandaloneSetupPoint(Time.deltaTime);
#else
			this.MobileSetupPoint (Time.deltaTime);
#endif
		}

		public virtual void MobileSetupPoint(float dt) {
			if (Input.GetMouseButton (0)) {
				var mousePosition = Input.mousePosition;
				var isLeft = mousePosition.x - (Screen.width / 2f) <= 0f;
				this.SteeringPoint (isLeft ? -1 : 1, dt);
			}
			this.MoveForward (dt);
		}

		public virtual void StandaloneSetupPoint(float dt) {
			var isLeft = Mathf.Clamp (Input.GetAxis ("Horizontal"), -1, 1);
			this.SteeringPoint (isLeft, dt);
			this.MoveForward (dt);
		}

		public virtual void MoveForward(float dt) {
			var forward = this.m_Point.transform.forward;
			var movePosition = forward.normalized * this.m_MoveSpeed * dt;
			this.m_Point.transform.position += movePosition;
		}

		public virtual void SteeringPoint(float turnLR, float dt) {
			this.m_CurrentAxis += turnLR * this.m_RotationSpeed * dt;
			this.m_Point.transform.rotation = Quaternion.AngleAxis (this.m_CurrentAxis, Vector3.up);
		}
		
	}
}
