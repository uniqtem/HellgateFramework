using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hellgate
{
	public delegate void SceneCallbackDelegate (SSceneController ctrl);

	public delegate void CallbackDelegate ();

	public class SSceneManager : MonoBehaviour
	{
		protected enum SceneType
		{
			SCENE,
			POPUP,
			MENU
		}

		protected class LoadLevelData
		{
			public string sceneName;
			public object data;
			public SceneCallbackDelegate active;
			public SceneCallbackDelegate deactive;
			public CallbackDelegate callback = null;
			public SceneType type = SceneType.SCENE;

			public LoadLevelData (string sceneName, object data, SceneCallbackDelegate active, SceneCallbackDelegate deactive)
			{
				this.sceneName = sceneName;
				this.data = data;
				this.active = active;
				this.deactive = deactive;
			}
		}

		protected const int DISTANCE = 10;
		protected const int POPUP_DEPTH = 100;

		public delegate void ScreenStartChangeDelegate (string sceneName);

		public delegate void SceneActivedDelegate (string sceneName);

		protected static SSceneManager instance;

		public static SSceneManager Instance {
			get {
				return instance;
			}
		}

		public ScreenStartChangeDelegate screenStartChange;
		public SceneActivedDelegate sceneOnActive;
		[SerializeField]
		protected Color
			defaultShieldColor = new Color (0, 0, 0, 0.25f);
		[SerializeField]
		protected string
			firstSceneName;
		protected Dictionary<string, GameObject> scenes;
		protected Dictionary<string, GameObject> menus;
		protected Stack<string> popups;
		protected List<GameObject> shields;
		protected GameObject scene;
		protected GameObject shield;
		protected GameObject gCamera;
		protected GameObject menu;
		protected GameObject solidCamera;
		protected GameObject uiCamera;

		protected virtual void Awake ()
		{
			instance = this;

			scenes = new Dictionary<string, GameObject> ();
			menus = new Dictionary<string, GameObject> ();
			popups = new Stack<string> ();
			shields = new List<GameObject> ();

			gCamera = new GameObject ("Cameras");
			scene = new GameObject ("Scenes");
			shield = new GameObject ("Shields");

			gCamera.transform.parent = instance.transform;
			gCamera.transform.localPosition = Vector3.zero;

			scene.transform.parent = instance.transform;
			scene.transform.localPosition = Vector3.zero;

			shield.transform.parent = instance.transform;
			shield.transform.localPosition = Vector3.zero;

			DontDestroyOnLoad (instance.gameObject);
			DontDestroyOnLoad (gCamera);
			DontDestroyOnLoad (scene);
			DontDestroyOnLoad (shield);

			solidCamera = Instantiate (Resources.Load ("SolidCamera")) as GameObject;
			solidCamera.name = "SolidCamera";
			solidCamera.transform.parent = gCamera.transform;
			solidCamera.transform.position = Vector3.zero;

			FirstSceneLoad ();
		}

#if UNITY_EDITOR || UNITY_ANDROID
		protected virtual void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Escape)) {
				SSceneController ctrl = null;
				if (popups.Count > 0) {
					ctrl = scenes [popups.Peek ()].GetComponent<SSceneController> ();
				} else {
					foreach (var pair in scenes) {
						if (pair.Value.activeSelf) {
							ctrl = pair.Value.GetComponent<SSceneController> ();
							break;
						}
					}
				}

				if (ctrl == null) {
					Close ();
				} else {
					ctrl.OnKeyBack ();
				}
			}
		}
#endif

		protected virtual void FirstSceneLoad ()
		{
			if (firstSceneName == "") {
				return;
			}

			Screen (firstSceneName);
		}

		protected virtual void LoadLevel (LoadLevelData loadLevelData)
		{
			bool isAddtive = false;
			switch (loadLevelData.type) {
			case SceneType.MENU:
			case SceneType.POPUP:
				isAddtive = true;
				break;
			}

			SSceneApplication.LoadLevel (loadLevelData.sceneName, delegate (GameObject root) {
				if (loadLevelData.type == SceneType.MENU) {
					menus.Add (loadLevelData.sceneName, root);
				} else {
					scenes.Add (loadLevelData.sceneName, root);
				}

				root.transform.parent = scene.transform;
				root.transform.localPosition = Vector3.zero;
				
				SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
				foreach (Camera cam in sRoot.Cameras) {
					AudioListener audio = cam.GetComponent<AudioListener> ();
					if (audio != null) {
						audio.enabled = false;
					}

					if (uiCamera == null) {
						if (cam.GetComponent<UICamera> () != null) {
							cam.clearFlags = CameraClearFlags.Depth;
							uiCamera = Instantiate (cam.gameObject) as GameObject;
							uiCamera.name = "UICamera";
							uiCamera.transform.parent = gCamera.transform;
							uiCamera.transform.localPosition = Vector3.zero;
							uiCamera.SetActive (true);

							cam.gameObject.SetActive (false);
						}
					} else {
						if (loadLevelData.type != SceneType.POPUP && cam.GetComponent<UICamera> () != null) {
							cam.gameObject.SetActive (false);
						}
					}
				}

				if (loadLevelData.type == SceneType.POPUP) {
					DistancePopUp (root);
					popups.Push (loadLevelData.sceneName);
				}

				SSceneController ctrl = root.GetComponent<SSceneController> ();
				ctrl.OnSet (loadLevelData.data);
				ctrl.active = loadLevelData.active;
				ctrl.deactive = loadLevelData.deactive;

				if (loadLevelData.callback != null) {
					loadLevelData.callback ();
				}

				if (screenStartChange != null) {
					screenStartChange (loadLevelData.sceneName);
				}

				MonoBehaviour uicam = uiCamera.GetComponent<MonoBehaviour> ();
				uicam.enabled = false;
				uicam.enabled = true;
			}, isAddtive);
		}

		protected virtual void DestroyScene (string sceneName)
		{
			GameObject scene = null;
			System.Action innerDestoryScene = () => {
				SSceneApplication.Unloaded (scene);
				Destroy (scene);
			};

			if (scenes.ContainsKey (sceneName)) {
				scene = scenes [sceneName];
				innerDestoryScene ();
				scenes.Remove (sceneName);
			}

			if (menus.ContainsKey (sceneName)) {
				scene = menus [sceneName];
				innerDestoryScene ();
				menus.Remove (sceneName);
			}
		}

		protected virtual void ClearScene (string sceneName = "")
		{
			List<string> keys = new List<string> ();
			foreach (var pair in scenes) {
				if (sceneName == pair.Key) {
					continue;
				}

				SSceneController ctrl = pair.Value.GetComponent<SSceneController> ();
				if (ctrl.IsCache) {
					OnDeativeScreen (pair.Value);
				} else {
					keys.Add (pair.Key);
				}
			}

			foreach (string key in keys) {
				DestroyScene (key);
			}

			popups.Clear ();
			ClearShield ();
		}

		protected virtual void ClearPopUp ()
		{
			foreach (string popup in popups) {
				SSceneController ctrl = scenes [popup].GetComponent<SSceneController> ();
				if (ctrl.IsCache) {
					OnDeativeScreen (scenes [popup]);
				} else {
					DestroyScene (popup);
				}
			}

			popups.Clear ();
			ClearShield ();
		}

		protected virtual void ClearMenu ()
		{
			List<string> keys = new List<string> ();
			foreach (var pair in menus) {
				SSceneController ctrl = pair.Value.GetComponent<SSceneController> ();
				if (ctrl.IsCache) {
					OnDeativeScreen (pair.Value);
				} else {
					keys.Add (pair.Key);
				}
			}
			
			foreach (string key in keys) {
				DestroyScene (key);
			}
		}

		protected virtual GameObject LastShield (bool flag)
		{
			float depth = 0;
			GameObject shield = null;
			for (int i = shields.Count - 1; i >= 0; i--) {
				if (shields [i].activeSelf == flag) {
					Camera cam = Util.FindChildObject (shields [i], "Camera").GetComponent<Camera> ();
					if (cam.depth > depth) {
						depth = cam.depth;
						shield = shields [i];
					}
				}
			}
			
			return shield;
		}
		
		protected virtual void ClearShield ()
		{
			foreach (GameObject sh in shields) {
				sh.SetActive (false);
			}
		}

		protected virtual void DistancePopUp (GameObject root)
		{
			int x = (popups.Count + 1) * DISTANCE;
			root.transform.localPosition = new Vector3 (x, 0, 0);

			int depth = POPUP_DEPTH;
			if (popups.Count > 0) {
				SSceneRoot lRoot = scenes [popups.Peek ()].GetComponent<SSceneRoot> ();
				Camera lCam = lRoot.Cameras.Last ();
				depth = (int)lCam.depth + 1;
			}

			SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
			int d = depth;
			foreach (Camera cam in sRoot.Cameras) {
				d++;
				cam.depth = d;
				if (cam.GetComponent<UICamera> () != null) {
					cam.clearFlags = CameraClearFlags.Depth;
				}
			}

			bool createShield = true;
			foreach (GameObject sh in shields) {
				if (!sh.activeSelf) {
					createShield = false;
					break;
				}
			}

			GameObject gShield = null;
			if (createShield) {
				gShield = Instantiate (Resources.Load ("nGUIShield")) as GameObject;
				gShield.name = "Shield" + shields.Count;
				gShield.transform.parent = shield.transform;
				gShield.GetComponentInChildren<Camera> ().gameObject.AddComponent<UICamera> ();
				shields.Add (gShield);
			} else {
				gShield = LastShield (false);
				if (gShield != null) {
					gShield.SetActive (true);
				}
			}
			
			gShield.transform.localPosition = new Vector3 (x, -DISTANCE, 0);
			Camera sCam = gShield.GetComponentInChildren<Camera> ();
			sCam.depth = depth;

			MeshRenderer mesh = gShield.GetComponentInChildren<MeshRenderer> ();
			mesh.material.color = defaultShieldColor;
		}

		protected virtual void OnActiveScreen (GameObject root)
		{
			root.SetActive (true);

			if (sceneOnActive != null) {
				sceneOnActive (root.name);
			}
		}

		protected virtual void OnDeativeScreen (GameObject root)
		{
			root.SetActive (false);
		}

		public virtual void Screen (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
		{
			if (scenes.ContainsKey (sceneName)) {
				GameObject root = scenes [sceneName];
				if (!root.activeSelf) {
					OnActiveScreen (root);

					return;
				}
			}

			LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
			loadLevel.callback = delegate() {
				ClearScene (sceneName);
			};

			LoadLevel (loadLevel);
		}

		public virtual void PopUp (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
		{
			if (scenes.ContainsKey (sceneName)) {
				GameObject root = scenes [sceneName];
				if (root.activeSelf) {
					if (popups.Peek () == sceneName) {
						Close ();
					}
				} else {
					DistancePopUp (root);
					popups.Push (sceneName);
					OnActiveScreen (root);
					
					return;
				}
			}

			LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
			loadLevel.type = SceneType.POPUP;

			LoadLevel (loadLevel);
		}

		public virtual void LoadMenu (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
		{
			if (menus.ContainsKey (sceneName)) {
				GameObject root = menus [sceneName];
				if (root.activeSelf) {
					DestroyScene (sceneName);
				} else {
					OnActiveScreen (root);
					return;
				}
			}
		
			LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
			loadLevel.type = SceneType.MENU;

			LoadLevel (loadLevel);
		}

		public virtual void Reboot ()
		{
			ClearMenu ();
			ClearScene ();
			FirstSceneLoad ();
		}

		public virtual void Close ()
		{
			if (popups.Count <= 0) {
				return;
			}

			string sceneName = popups.Peek ();
			GameObject root = scenes [sceneName];
			SSceneController ctrl = root.GetComponent<SSceneController> ();
			if (ctrl.IsCache) {
				OnDeativeScreen (root);
			} else {
				DestroyScene (sceneName);
			}
			popups.Pop ();

			GameObject shield = LastShield (true);
			if (shield != null) {
				shield.SetActive (false);
			}
		}

		public virtual void DestroyScenesFrom (string sceneName)
		{
			if (popups.Contains (sceneName)) {
				return;
			}

			DestroyScene (sceneName);
		}
	}
}
