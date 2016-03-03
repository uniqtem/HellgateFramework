//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
    [RequireComponent (typeof(SSceneRoot))]
    public class SSceneController : MonoBehaviour
    {
        /// <summary>
        /// The active event.
        /// </summary>
        public SceneCallbackDelegate active;
        /// <summary>
        /// The deactive event.
        /// </summary>
        public SceneCallbackDelegate deactive;
        /// <summary>
        /// Close pop up callback .
        /// </summary>
        public CallbackDelegate callback;
        /// <summary>
        /// The is cache.
        /// </summary>
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
            if (isCache) {
                if (deactive != null) {
                    deactive (ctrl);
                }

                if (callback != null) {
                    callback ();
                }
            }
        }

        public virtual void OnDestroy ()
        {
            if (deactive != null) {
                deactive (ctrl);
            }

            if (callback != null) {
                callback ();
            }
        }

        /// <summary>
        /// Raises the set event.
        /// </summary>
        /// <param name="data">Data.</param>
        public virtual void OnSet (object data)
        {
        }

        /// <summary>
        /// Raises the key back event.
        /// </summary>
        public virtual void OnKeyBack ()
        {
            SSceneManager.Instance.Close ();
        }
    }
}
