// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;
using System.Collections;
using UnityUtilLib;
using System.Collections.Generic;

namespace DanmakU {

	/// <summary>
	/// A container behavior used on prefabs to define how a bullet looks or behaves
	/// </summary>
	[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(SpriteRenderer)), AddComponentMenu("Danmaku 2D/Danmaku Prefab")]
	public sealed class DanmakuPrefab : DanmakuObjectPrefab {

		private DanmakuPrefab runtime;

		[SerializeField]
		private ParticleSystem danmakuParticlePrefab;

		private static ParticleSystem.Particle hiddenParticle;

		//public int j;

		private Mesh spriteMesh;
		private ParticleSystem runtimeSystem;
		private ParticleSystemRenderer runtimeRenderer;
		private ParticleSystem.Particle[] particles;
		//private Danmaku[] currentDanmaku;
		private HashSet<Danmaku> currentDanmaku;
		private int danmakuCount;

		static DanmakuPrefab() {
			hiddenParticle = new ParticleSystem.Particle();
			hiddenParticle.position = new Vector3(20000000f, 0f, 0f);
			hiddenParticle.startLifetime = float.PositiveInfinity;
			hiddenParticle.lifetime = float.PositiveInfinity;
			hiddenParticle.velocity = Vector3.zero;
			hiddenParticle.angularVelocity = 0;
			hiddenParticle.axisOfRotation = Vector3.forward;
		}

		[SerializeField]
		private IDanmakuController[] extraControllers;
		internal IDanmakuController[] ExtraControllers {
			get {
				return extraControllers;
			}
		}

		public void Add(Danmaku danmaku) {
//			Debug.Log("Add");
//			if (currentDanmaku == null) {
//				currentDanmaku = new Danmaku[128];
//			}
//			danmakuCount++;
//			if (danmakuCount >= currentDanmaku.Length) {
//				Danmaku[] temp = new Danmaku[Mathf.NextPowerOfTwo(danmakuCount + 1)];
//				System.Array.Copy(currentDanmaku, temp, currentDanmaku.Length);
//				currentDanmaku = temp;
//			}
//			danmaku.renderIndex = danmakuCount;
//			currentDanmaku [danmakuCount] = danmaku;
			currentDanmaku.Add(danmaku);
		}

		public void Remove(Danmaku danmaku) {
//			Debug.Log("remove");
//			int deadIndex = danmaku.poolIndex;;
//			Danmaku temp = currentDanmaku [danmakuCount];
//			currentDanmaku [danmakuCount] = danmaku;
//			currentDanmaku [deadIndex] = temp;
//			danmaku.renderIndex = danmakuCount;
//			danmaku.renderIndex = deadIndex;
//			danmakuCount--;
			currentDanmaku.Remove(danmaku);
		}

		void Update() {
			int danmakuCount = currentDanmaku.Count;
			Debug.Log(danmakuCount);
			int count = runtimeSystem.particleCount;
			if (danmakuCount > count) {
				//Debug.Log("hello");
				runtimeSystem.maxParticles = Mathf.NextPowerOfTwo(danmakuCount);
				runtimeSystem.Emit(danmakuCount - count);
				//Debug.Log(runtimeSystem.particleCount);
				count = danmakuCount;
			}
			if (danmakuCount > particles.Length) {
				particles = new ParticleSystem.Particle[Mathf.NextPowerOfTwo(danmakuCount + 1)];
			}

			int count2 = runtimeSystem.GetParticles(particles);
//			if(count > count2) {
//				Debug.Log(count + ", " + count2);
//			}
			Vector3 forward = Vector3.forward;
			bool done;
			IEnumerator<Danmaku> enumerator = currentDanmaku.GetEnumerator();
			for(int i = 0; i < danmakuCount; i++) {
				done = enumerator.MoveNext();
				if(done) {
					Danmaku danmaku = enumerator.Current;
					particles[i].position = danmaku.Position;
					particles[i].rotation = danmaku.Rotation;
					particles[i].size = danmaku.Scale;
					particles[i].axisOfRotation = forward;
					particles[i].lifetime = 1000;
					particles[i].color = danmaku.Color;
				} else {
					particles[i].size = 0f;
					particles[i].lifetime = -1;
				}
			}
//			for(; index < count; index++) {
//				particles[index].lifetime = 10000;
//				particles[index].position = Vector2.up * 20;
//				particles[index].size = 0f;
//			}
			runtimeSystem.SetParticles(particles, danmakuCount);
		}

		public override void Awake() {
			base.Awake();

			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<CircleCollider2D>().enabled = false;

			currentDanmaku = new HashSet<Danmaku>();
			//currentDanmaku = new Danmaku[128];

			particles = new ParticleSystem.Particle[128];

			//Disable all other components
			foreach (Behaviour comp in GetComponentsInChildren<Behaviour>()) {
				if(comp != this) {
					comp.enabled = false;
				}
			}
	
			runtimeSystem = Instantiate(danmakuParticlePrefab) as ParticleSystem;
			runtimeSystem.transform.position = Vector3.zero;
			runtimeRenderer = runtimeSystem.GetComponent<ParticleSystemRenderer> ();

			runtimeSystem.simulationSpace = ParticleSystemSimulationSpace.World;
			runtimeSystem.startColor = Color;
			runtimeSystem.startSize = 1;
			runtimeSystem.startLifetime = float.PositiveInfinity;
			runtimeSystem.gravityModifier = 0f;
			runtimeSystem.startSpeed = 0f;
			runtimeSystem.enableEmission = false;
			
			runtimeSystem.Emit(runtimeSystem.maxParticles);

			particles = new ParticleSystem.Particle[runtimeSystem.particleCount];

			Material renderMaterial = new Material(Material);
			renderMaterial.mainTexture = Sprite.texture;

			spriteMesh = new Mesh();

			var verts = Sprite.vertices;
			var tris = Sprite.triangles;
			
			Vector3[] vertexes = new Vector3[verts.Length];
			int[] triangles = new int[tris.Length];

			Matrix4x4 transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transform.localScale);
			
			for (int i = 0; i < verts.Length; i++) {
				vertexes[i] = transformMatrix * ((Vector3)verts[i]);
				//vertexes[i] = verts[i];
			}
			
			for (int i = 0; i < tris.Length; i++) {
				triangles[i] = (int)tris[i];
			}

			spriteMesh.vertices = vertexes;
			spriteMesh.uv = Sprite.uv;
			spriteMesh.triangles = triangles;

			runtimeRenderer.renderMode = ParticleSystemRenderMode.Mesh;
			runtimeRenderer.sharedMaterial = renderMaterial;
			runtimeRenderer.mesh = spriteMesh;
			runtimeRenderer.sortingLayerID = cachedSortingLayer;
			runtimeRenderer.sortingOrder = cachedSortingOrder;
			runtimeRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			runtimeRenderer.receiveShadows = false;
			runtimeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			runtimeRenderer.useLightProbes = false;

		}

		void OnDestroy() {
			if (runtimeSystem != null) {
				Destroy(runtimeSystem.gameObject);
			}
			if (spriteMesh != null) {
				Destroy(spriteMesh);
			}
		}

		internal DanmakuPrefab GetRuntime() {
			if(runtime == null)
				runtime = CreateRuntimeInstance(this);
			return runtime;
		}

		private static DanmakuPrefab CreateRuntimeInstance(DanmakuPrefab prefab) {
			DanmakuPrefab runtime = (DanmakuPrefab)Instantiate (prefab);
			//runtime.gameObject.hideFlags = HideFlags.HideInHierarchy;
			//runtime.gameObject.SetActive (false);
			return runtime;
		}
	}
}