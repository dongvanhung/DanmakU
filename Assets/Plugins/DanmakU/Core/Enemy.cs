// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;
using UnityUtilLib;

/// <summary>
/// A development kit for quick development of 2D Danmaku games
/// </summary>
namespace DanmakU {

	/// <summary>
	/// An abstract class to define all enemies seen in Danmaku games.
	/// These are fired at by the player, and fire back at the player in various patterns.
	/// Killing these generally rewards the player with pickups or points.
	/// The bullets fired by these also can easily kill the player.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public abstract class Enemy : DanmakuCollider, IPausable, IDanmakuObject {

		public virtual AttackPattern CurrentAttackPattern { 
			get {
				return null;
			}
		}

		private Renderer enemyRenderer;
		private DanmakuField field;

		#region IPausable implementation

		public bool Paused {
			get;
			set;
		}

		#endregion

		#region IDanmakuObject implementation

		public DanmakuField Field {
			get;
			set;
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this instance is dead.
		/// </summary>
		/// <value><c>true</c> if this instance is dead; otherwise, <c>false</c>.</value>
		public abstract bool IsDead { 
			get; 
		}

		public override void Awake () {
			base.Awake ();
			EnemyManager.RegisterEnemy (this);
			enemyRenderer = GetComponent<Renderer> ();
			Field = DanmakuField.FindClosest (this);
		}

		public virtual void Start() {
		}

		public void Hit(float damage) {
			Damage (damage);
			if(IsDead) {
				Die ();
			}
		}

		/// <summary>
		/// Damages this enemy with agiven amount of damage.
		/// </summary>
		/// <param name="damage">the damage taken.</param>
		protected abstract void Damage (float damage);

		private void Die () {
			EnemyManager.UnregisterEnemy (this);
			Destroy (gameObject);
			OnDeath ();
		}

		/// <summary>
		/// An overridable event call usable in subclasses to add custom behavior for when this instance takes damage
		/// </summary>
		protected virtual void OnDamage() {
		}

		/// <summary>
		/// An overridable event call usable in subclasses to add custom behavior for when this instance dies
		/// </summary>
		protected virtual void OnDeath() {
		}


		#region implemented abstract members of DanmakuCollider
		protected override void ProcessDanmaku (Danmaku danmaku, RaycastHit2D info){
			if (enemyRenderer.isVisible) {
				Hit (danmaku.Damage);
				danmaku.Deactivate();
			}
		}
		#endregion
	}
}