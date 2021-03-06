// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;

/// <summary>
/// A utilty library of random useful and portable scripts for Unity
/// </summary>
namespace UnityUtilLib {

	/// <summary>
	/// An abstract class for all <a href="http://en.wikipedia.org/wiki/Singleton_pattern">Singleton</a> MonoBehaviours.
	/// Useful for creating managers where there should be only one instance at any given time.
	/// To use, simply subclass like so:
	/// <c>public class Foo : Singleton<Foo></c>
	/// </summary>
	[RequireComponent(typeof(StaticGameObject))]
	public abstract class Singleton<T> : CachedObject where T : Singleton<T> {

		private static T instance;

		public static T Instance {
			get { 
				if(instance == null) {
					instance = FindObjectOfType<T>();
					if(instance == null) {
						instance = new GameObject(typeof(T).Name).AddComponent<T>();
					}
				}
				return instance; 
			}
		}

		public bool destroyNewInstances;

		public override void Awake () {
			base.Awake ();
			if(instance != null) {
				if(instance.destroyNewInstances) {
					Destroy (this);
					return;
				} else {
					Destroy (instance);
				}
			}
			
			instance = (T)this;
		}
	}
}