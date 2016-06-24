//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Hellgate
{
    [ExecuteInEditMode]
    public class SSceneRoot : MonoBehaviour
    {
        /// <summary>
        /// The cameras.
        /// </summary>
        [SerializeField]
        protected Camera[] cameras;
        /// <summary>
        /// The event system.
        /// </summary>
        [SerializeField]
        protected EventSystem eventSystem;

        /// <summary>
        /// Gets the cameras.
        /// </summary>
        /// <value>The cameras.</value>
        public Camera[] Cameras {
            get {
                return cameras;
            }
        }

        /// <summary>
        /// Gets the event system.
        /// </summary>
        /// <value>The event system.</value>
        public EventSystem EventSystem {
            get {
                return eventSystem;
            }
        }

        protected virtual void Awake ()
        {
            if (!Application.isPlaying) {
                // ngui
                if (cameras == null) {
                    cameras = GameObject.FindObjectsOfType<Camera> ();
                }

                // ugui
                if (eventSystem == null) {
                    eventSystem = GameObject.FindObjectOfType<EventSystem> ();
                }

#if UNITY_EDITOR
                #if UNITY_5_3
                gameObject.name = Path.GetFileNameWithoutExtension (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().name);
                #else
                gameObject.name = Path.GetFileNameWithoutExtension (EditorApplication.currentScene);
                #endif
#endif
            } else {
                if (SSceneManager.Instance != null) {
                    SSceneApplication.Loaded (gameObject);
                }
            }
        }
    }
}
