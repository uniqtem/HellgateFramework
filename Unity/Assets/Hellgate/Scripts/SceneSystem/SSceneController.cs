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
		private SSceneController ctrl;

		public bool IsCache {
			get {
				return isCache;
			}
		}

		public virtual void Awake ()
		{
			isCache = false;
			ctrl = this;
		}

		public virtual void Start ()
		{
			if (active != null) {
				active (ctrl);
			}
		}

		public virtual void OnEnable ()
		{
			if (active != null) {
				active (ctrl);
			}
		}

		public virtual void OnDisable ()
		{
			if (deactive != null) {
				deactive (ctrl);
			}
		}

		public virtual void OnDestroy ()
		{
			if (deactive != null) {
				deactive (ctrl);
			}
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
