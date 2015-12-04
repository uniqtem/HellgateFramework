using UnityEngine;
using System.Collections;

namespace Hellgate
{
	[RequireComponent(typeof(SSceneRoot))]
	public class SSceneController : MonoBehaviour
	{
		public SceneCallbackDelegate active;
		public SceneCallbackDelegate deactive;
		protected bool isCache;

		public bool IsCache {
			get {
				return isCache;
			}
		}

		public virtual void Awake ()
		{
			isCache = false;
		}

		public virtual void Start ()
		{
		}

		public virtual void OnEnable ()
		{
		}

		public virtual void OnDisable ()
		{
		}

		public virtual void OnDestroy ()
		{
		}

		public virtual void OnSet (object data)
		{
		}

		public virtual void OnKeyBack ()
		{
			SSceneManager.Instance.Close ();
		}
	}
}
