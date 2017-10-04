using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DangerousTown3D {
	public class CResidentController : CObjectController {

		[Header("Target")]
		[SerializeField]	protected CObjectController m_TargetObject;
		[Header("Controller")]
		[SerializeField]	protected CapsuleCollider m_CapsuleCollider;
		[SerializeField]	protected Animator m_Animator;
		[Header("Move")]
		[SerializeField]	protected AnimationCurve m_MoveCurve;
		[SerializeField]	protected float m_MoveSpeed = 5f;
		[SerializeField]	protected float m_RotationSpeed = 150f;

		protected float m_MoveDetailCurve = 0f;
		protected bool m_IsAlive;

		public bool IsAlive {
			get { return this.m_IsAlive; }
			set { this.m_IsAlive = value; }
		}

		protected override void Awake ()
		{
			base.Awake ();
			this.m_IsAlive = true;
		}

		protected override void Update ()
		{
			base.Update ();
			if (this.m_TargetObject == null)
				return;
			this.MoveFollow (this.m_TargetObject.GetPosition(), Time.deltaTime);
		}

		public virtual void MoveFollow(Vector3 position, float dt) {
			var isNearTarget = this.IsNearTarget ();
			this.SetAnimation ("AnimParam", isNearTarget ? 0 : 1);
			if (isNearTarget) {
				this.m_MoveDetailCurve = 0f;
				return;
			}
			var direction = this.m_TargetObject.GetPosition() - this.GetPosition();
			var target = Quaternion.LookRotation (direction);
			this.m_Transform.rotation = Quaternion.Lerp (this.m_Transform.rotation, target, 0.5f);

			this.MoveForward ();
		}

		public virtual void MoveForward() {
			var dt = Time.deltaTime;
			var forward = this.m_Transform.forward;
			var movePosition = forward.normalized * this.m_MoveSpeed * dt;
			this.m_Transform.position += movePosition * this.m_MoveCurve.Evaluate (this.m_MoveDetailCurve);
			this.m_MoveDetailCurve += dt * this.m_MoveSpeed;
		}

		public virtual void SetAnimation(string name, object param) {
			if (this.m_Animator == null)
				return;
			if (param is int) {
				this.m_Animator.SetInteger (name, (int)param);
			} else if (param is bool) {
				this.m_Animator.SetBool (name, (bool)param);
			} else if (param is float) {
				this.m_Animator.SetFloat (name, (float)param);
			} else if (param == null) {
				this.m_Animator.SetTrigger (name);
			}
		}

		public override CObjectController GetTarget ()
		{
			return this.m_TargetObject;
		}

		public virtual float GetRadius() {
			if (this.m_CapsuleCollider != null)
				return this.m_CapsuleCollider.radius;
			return 0f;
		}

		public virtual bool IsNearTarget() {
			if (this.m_TargetObject == null)
				return true;
			var radius = this.m_CapsuleCollider == null ? 0.01f : this.m_CapsuleCollider.radius;
			var nearestPoint = this.m_TargetObject.GetNearestPoint (this.GetPosition());
			var direction = nearestPoint - this.GetPosition();
			return direction.sqrMagnitude <= radius;
		}

	}
}
