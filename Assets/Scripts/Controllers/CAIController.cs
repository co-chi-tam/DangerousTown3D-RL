using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

namespace DangerousTown3D {
	public class CAIController : CResidentController, AI.ReinforcementProblem<QLearnState, QLearnAction>  {

		protected float m_CurrentAxis;
		protected float m_MaxDistance = 1000f;

		private static QLearning<QLearnState, QLearnAction> algorithm = null;
		private List<QLearnAction> actions = new List<QLearnAction>();
		private QLearnReward reward = new QLearnReward();

		// Holds a reference to a state in the prior frame of updating
		protected QLearnState m_State = null;
		// Holds a reference to an action in the prior frame of updating
		protected QLearnAction m_Action = null;

		public QLearnReward Rewards {
			get { return reward; }
			set { reward = value; }
		}

		public QLearning<QLearnState, QLearnAction> QLearningAlgorithm {
			get { return algorithm; }
			set {
				algorithm = value;
				// Ensure that the reinforcement problem references
				// this instance so that updates and actions eventually
				// reference this instance.
				algorithm.Problem = this;
			}
		}

		private float[] detectRays = new float[] { 0f, -25f, 25f };
		private Vector3[] detectDistances = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
		protected QLearnState State {
			get {
				var forward = this.m_Transform.forward;
				var distance = 2f;
				var origin = this.GetPosition ();
				for (int i = 0; i < detectRays.Length; i++) {
					var rayCast = Quaternion.AngleAxis(detectRays[i], this.m_Transform.up) * forward * distance;
					RaycastHit rayCastHit;
					var fixOrigin = this.m_Transform.position + (rayCast.normalized * this.GetRadius ());
					if (Physics.Raycast (fixOrigin, rayCast, out rayCastHit, distance)) {
						var hitPoint = rayCastHit.point;
						detectDistances [i] = hitPoint;
					} else {
						detectDistances [i] = origin;
					}
#if UNITY_EDITOR
					Debug.DrawRay (fixOrigin, rayCast, Color.red);
					Debug.DrawLine (origin, this.GetTarget().GetPosition(), Color.blue);
#endif
				}
				return new QLearnState(detectDistances [0], detectDistances [1], detectDistances [2]);
			}
		}

		protected float Reward {
			get {
				var distance = (this.GetTarget().GetPosition() - this.GetPosition()).sqrMagnitude;
				distance = distance > this.m_MaxDistance ? this.m_MaxDistance : distance;
				var currentScore = ((this.m_MaxDistance - distance) / this.m_MaxDistance) * reward.aliveReward;
				return (this.IsAlive 
					? currentScore 
					: reward.deathReward);
			}
		}

		protected override void Start ()
		{
			base.Start ();
			this.InitQLearnAlgorithm ();
		}

		protected override void Update ()
		{
			// This has to be checked here since in constructor
			// or when Player is assigned, Pipes may not have been
			// spawned yet...
			if (m_State == null) {
				m_State = State;
			}

			if (!CGameManager.Instance.IsGameOver) {
				QLearningAlgorithm.Update(m_State, m_Action, Reward, State);
				if (CGameManager.Instance.IsTeaching == false) {
					m_State = State;
					m_Action = QLearningAlgorithm.Explore (m_State);
					m_Action.Action ();
				} else {
					if (Input.GetKeyDown (KeyCode.A)) {
						this.TurnLeft ();
					}
					if (Input.GetKeyDown (KeyCode.D)) {
						this.TurnRight ();
					}
				}
			}

			if (!this.IsAlive) {
				CGameManager.Instance.TriggerGameOver ();
			} else {
				this.MoveForward ();
			}
		}

		protected virtual void InitQLearnAlgorithm() {
			if (algorithm == null) {
				algorithm = new QLearning<QLearnState, QLearnAction> ();
			}
			algorithm.Problem = this;
			actions.Add (DoNothingAction);
			actions.Add (DoTurnLeft);
			actions.Add (DoTurnRight);
			m_Action = DefaultAction;
		}

		protected virtual void OnTriggerEnter(Collider coll) {
			if (coll.gameObject.tag == "Obstacle") {
				this.m_IsAlive = false;
			}
		}

		public virtual void TurnLeft() {
			this.TurnObject (-1);
		}

		public virtual void TurnRight() {
			this.TurnObject (1);
		}

		public virtual void TurnObject(int turn) {
			this.m_CurrentAxis += turn * this.m_RotationSpeed;
			this.m_Transform.rotation = Quaternion.AngleAxis (this.m_CurrentAxis, Vector3.up);
		}

		public virtual void DoNothing() {
			// TODO
		}

		protected QLearnAction RandomAction {
			get {
				var random = UnityEngine.Random.Range (0, 3);
				return (random == 0 ? DoNothingAction 
						: random == 1 ? DoTurnLeft 
							: DoTurnRight);
			}
		}

		protected QLearnAction DefaultAction {
			get {
				return DoNothingAction;
			}
		}

		protected QLearnAction DoNothingAction {
			get {
				return new QLearnAction("DoNothing", DoNothing);
			}
		}

		protected QLearnAction DoTurnLeft {
			get {
				return new QLearnAction("TurnLeft", TurnLeft);
			}
		}

		protected QLearnAction DoTurnRight {
			get {
				return new QLearnAction("TurnRight", TurnRight);
			}
		}

		public List<QLearnAction> GetAvailableActions(QLearnState state) {
//			List<QLearnAction> actions = new List<QLearnAction>();
//
//			 Order is important so in case of equality,
//			 DoNothing is preferred over Jump action.
//			actions.Add(new QLearnAction("DoNothing", DoNothing));
//
//			 Jump action should only be made available when the bird is
//			 under a specific threshold, else it learns how to cheat and
//			 fly over the pipes!
//			
//			 NOTE Fixed by adding cieling in scene
//			
//			if (Player.transform.position.y < Player.jumpThreshold) {
//				actions.Add(new QLearnAction("Jump", Jump));
//			}
//
//			 Independent of the state, the same actions are always returned
			return actions;
		}

	}
}
