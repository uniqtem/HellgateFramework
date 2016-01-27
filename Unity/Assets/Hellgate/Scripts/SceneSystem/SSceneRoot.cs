//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
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
		/// Gets the cameras.
		/// </summary>
		/// <value>The cameras.</value>
		public Camera[] Cameras {
			get {
				return cameras;
			}
		}

		protected virtual void Awake ()
		{
			if (!Application.isPlaying) {
				if (cameras == null) {
					cameras = GameObject.FindObjectsOfType<Camera> ();
				}
#if UNITY_EDITOR
	#if UNITY_5_3
				UnityEditor.SceneManagement.SceneSetup sceneSetup = new UnityEditor.SceneManagement.SceneSetup ();
				gameObject.name = Path.GetFileNameWithoutExtension (sceneSetup.path);
	#elif
				gameObject.name = Path.GetFileNameWithoutExtension (EditorApplication.currentScene);
	#endif
#endif
			} else {
				if (SSceneManager.Instance != null) {
					SSceneApplication.Loaded (gameObject);
				}
			}
		}

//		protected virtual void Update ()
//		{
//#if UNITY_EDITOR
//			if (!Application.isPlaying) {
//					gameObject.name = Path.GetFileNameWithoutExtension (EditorApplication.currentScene);
//			}
//#endif
//		}
	}
}
