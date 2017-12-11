using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSingleton;

namespace DangerousTown3D {
	public class CGameManager : CMonoSingleton<CGameManager> {

		[SerializeField]	protected bool m_IsGameOver = false;
		public bool IsGameOver {
			get { return this.m_IsGameOver; }
			private set { this.m_IsGameOver = value; }
		}

		protected override void Awake ()
		{
			base.Awake ();
			Application.targetFrameRate = 60;
			this.m_IsGameOver = false;
		}

		public void TriggerGameOver() {
			if (this.m_IsGameOver == false) {
				this.m_IsGameOver = true;

				Invoke("Restart", 0.5f);
			}
		}

		public void Restart() {
			this.m_IsGameOver = false;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

	}
}
