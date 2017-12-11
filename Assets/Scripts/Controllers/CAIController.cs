using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

namespace DangerousTown3D {
	public class CAIController : CResidentController, AI.ReinforcementProblem<QLearnState, QLearnAction>  {

		#region Fields

		// Life cyrcle
		protected float m_CurrentAxis;
		protected float m_MaxDistance = 1000f;
		protected float m_LifeTimer = 10f;
		protected bool m_IsCompleted = false;

		// Q-Learning
		private static QLearning<QLearnState, QLearnAction> algorithm = null;
		private List<QLearnAction> actions = new List<QLearnAction>();
		private QLearnReward reward = new QLearnReward();

		// Holds a reference to a state in the prior frame of updating
		protected static QLearnState m_State = null;
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

		protected QLearnState State {
			get {
				var position = this.GetPosition ();
				var rotation = this.m_Transform.rotation.eulerAngles;
				return new QLearnState(
					position, 
					rotation
				);
			}
		}

		protected float Reward {
			get {
				if (this.IsAlive) {
					if (this.m_IsCompleted == false) {
						var distance = (this.GetTarget ().GetPosition () - this.GetPosition ()).sqrMagnitude;
						var currentScore = ((this.m_MaxDistance - distance) / this.m_MaxDistance) * reward.aliveReward;
						return currentScore;
					} else {
						return reward.aliveReward;
					}
				} 
				return reward.deathReward;
			}
		}

		#endregion

		#region Implementation Monobehaviour

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
				m_State = State;
				m_Action = QLearningAlgorithm.Explore (m_State);
				m_Action.Action ();
			}

			if (!this.IsAlive) {
				CGameManager.Instance.TriggerGameOver ();
			} else {
				this.IsAlive = m_LifeTimer > 0f;
			}

			if (this.m_IsCompleted) {
				CGameManager.Instance.Restart ();
			}
		}

		#endregion

		#region Implementation Q-Learning

		protected virtual void InitQLearnAlgorithm() {
			if (algorithm == null) {
				algorithm = new QLearning<QLearnState, QLearnAction> ();
			}
			algorithm.Problem = this;
			actions.Add (DoMoveForwardAction);
			actions.Add (DoTurnLeft);
			actions.Add (DoTurnRight);
			actions.Add (DoMoveFollow);
			actions.Add (DoNothingAction);
			m_Action = DoMoveFollow;
		}

		protected virtual void OnTriggerEnter(Collider coll) {
			if (coll.gameObject.tag == "Obstacle") {
				this.m_IsAlive = false;
			}
			if (coll.gameObject.tag == "Point") {
				this.m_IsCompleted = true;
			}
		}

		public virtual void TurnLeft() {
			this.TurnObject (-1);
			this.MoveForward ();
		}

		public virtual void TurnRight() {
			this.TurnObject (1);
			this.MoveForward ();
		}

		public virtual void TurnObject(int turn) {
			this.m_CurrentAxis = this.m_Transform.rotation.eulerAngles.y;
			this.m_CurrentAxis += turn * this.m_RotationSpeed;
			this.m_Transform.rotation = Quaternion.AngleAxis (this.m_CurrentAxis, Vector3.up);
			m_LifeTimer = 10f;
		}

		public override void MoveForward ()
		{
			base.MoveForward ();
			m_LifeTimer = 10f;
		}

		public override void MoveFollow() {
			var dt = Time.deltaTime;
			var isNearTarget = this.IsNearTarget ();
			if (isNearTarget) {
				return;
			}
			var direction = this.m_TargetObject.GetPosition() - this.GetPosition();
			var target = Quaternion.LookRotation (direction);
			this.m_Transform.rotation = target;

			this.MoveForward ();
		}

		public virtual void DoNothing() {
			m_LifeTimer -= Time.deltaTime;
		}

		protected QLearnAction DoRandomAction {
			get {
				var random = UnityEngine.Random.Range (0, 3);
				return (random == 0 ? DoMoveForwardAction
						: random == 1 ? DoTurnLeft 
							: random == 2 ? DoTurnRight  
								: DoNothingAction);
			}
		}

		protected QLearnAction DoMoveFollow {
			get { 
				return new QLearnAction ("DoMoveFollow", MoveFollow);
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

		protected QLearnAction DoMoveForwardAction {
			get {
				return new QLearnAction("MoveForward", MoveForward);
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

		#endregion

	}
}
