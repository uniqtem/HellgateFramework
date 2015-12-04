using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
	public class SSceneApplication
	{
		public delegate void OnLoadDelegate(GameObject root);

		private static Dictionary<string, OnLoadDelegate> onLoadeds = new Dictionary<string, OnLoadDelegate> ();

		private static bool AddScene (string sceneName, OnLoadDelegate onLoaded)
		{
			if (onLoadeds.ContainsKey (sceneName)) {
				return false;
			}

			onLoadeds.Add (sceneName, onLoaded);
			return true;
		}

		public static void LoadLevel (string sceneName, OnLoadDelegate onLoaded = null, bool isAdditive = false)
		{
			if (AddScene (sceneName, onLoaded)) {
				if (isAdditive) {
					Application.LoadLevelAdditive (sceneName);
				} else {
					Application.LoadLevel (sceneName);
				}
			}
		}

		public static void LoadLevelAsync (string sceneName, OnLoadDelegate onLoaded = null, bool isAdditive = false)
		{
			if (AddScene (sceneName, onLoaded)) {
				if (isAdditive) {
					Application.LoadLevelAdditive (sceneName);
				} else {
					Application.LoadLevel (sceneName);
				}
			}
		}

		public static void Loaded (GameObject root)
		{
			if (onLoadeds [root.name] != null) {
				onLoadeds [root.name] (root);
			}
		}

		public static void Unloaded (GameObject root)
		{
			if (onLoadeds.ContainsKey (root.name)) {
				onLoadeds.Remove (root.name);
			}
		}
	}
}
