//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hellgate
{
#region Enum

    public enum UIType
    {
        UGUI,
        NGUI
    }

#endregion

    /// <summary>
    /// Scene callback delegate.
    /// </summary>
	public delegate void SceneCallbackDelegate (SSceneController ctrl);

    /// <summary>
    /// Callback delegate.
    /// </summary>
	public delegate void CallbackDelegate ();

    public class SSceneManager : MonoBehaviour
    {
#region Enum

        protected enum SceneType
        {
            SCREEN,
            ADDSCREEN,
            POPUP,
            MENU
        }

#endregion

#region Class

        protected class LoadLevelData
        {
            public string sceneName;
            public object data;
            public SceneCallbackDelegate active;
            public SceneCallbackDelegate deactive;
            public SceneType type = SceneType.SCREEN;

            public LoadLevelData (string sceneName, object data, SceneCallbackDelegate active, SceneCallbackDelegate deactive)
            {
                this.sceneName = sceneName;
                this.data = data;
                this.active = active;
                this.deactive = deactive;
            }
        }

#endregion

#region Const

        /// <summary>
        /// Used for the calculation position.
        /// </summary>
        protected const int DISTANCE = 10;
        /// <summary>
        /// Used for the calculation popUp depth.
        /// </summary>
        protected const int POPUP_DEPTH = 1001;

#endregion

#region Delegate

        public delegate void ScreenStartChangeDelegate (string sceneName);

        public delegate void SceneActivedDelegate (string sceneName);

        /// <summary>
        /// The screen start change event.
        /// </summary>
        public ScreenStartChangeDelegate screenStartChange;
        /// <summary>
        /// The scene on active event.
        /// </summary>
        public SceneActivedDelegate sceneOnActive;

#endregion

#region Singleton

        protected static SSceneManager instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SSceneManager Instance {
            get {
                return instance;
            }
        }

#endregion

#region SerializeField

        /// <summary>
        /// The type of the UI.
        /// </summary>
        [SerializeField]
        protected UIType uIType = UIType.UGUI;
        /// <summary>
        /// The default color of the shield.
        /// </summary>
        [SerializeField]
        protected Color defaultShieldColor = new Color (0, 0, 0, 0.25f);
        /// <summary>
        /// The first name of the scene.
        /// </summary>
        [SerializeField]
        protected string firstSceneName;

#endregion

        /// <summary>
        /// The scenes.
        /// </summary>
        protected Dictionary<string, GameObject> scenes;
        /// <summary>
        /// The menus.
        /// </summary>
        protected Dictionary<string, GameObject> menus;
        /// <summary>
        /// The popups.
        /// </summary>
        protected Stack<string> popups;
        /// <summary>
        /// The screens.
        /// </summary>
        protected Stack<string> screens;
        /// <summary>
        /// The shields.
        /// </summary>
        protected List<GameObject> shields;
        /// <summary>
        /// The scene.
        /// </summary>
        protected GameObject scene;
        /// <summary>
        /// The shield.
        /// </summary>
        protected GameObject shield;
        /// <summary>
        /// The camera.
        /// </summary>
        protected GameObject gCamera;
        /// <summary>
        /// The menu.
        /// </summary>
        protected GameObject menu;
        /// <summary>
        /// The solid camera.
        /// </summary>
        protected GameObject solidCamera;
        /// <summary>
        /// The NGUI camera.
        /// </summary>
        protected GameObject nGUICamera;

        /// <summary>
        /// Gets the UI type
        /// </summary>
        /// <value>The UI type</value>
        public UIType _UIType {
            get {
                return uIType;
            }
        }

        /// <summary>
        /// Gets the NGUI camera.
        /// </summary>
        /// <value>The NGUI camera.</value>
        public Camera NGUICamera {
            get {
                return nGUICamera == null ? null : nGUICamera.GetComponent<Camera> ();
            }
        }

        protected virtual void Awake ()
        {
            instance = this;

            nGUICamera = null;

            scenes = new Dictionary<string, GameObject> ();
            menus = new Dictionary<string, GameObject> ();
            popups = new Stack<string> ();
            screens = new Stack<string> ();
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

#if !UNITY_5_3 
            DontDestroyOnLoad (gCamera);
            DontDestroyOnLoad (scene);
            DontDestroyOnLoad (shield);
#endif

            solidCamera = Instantiate (Resources.Load ("HellgateSolidCamera")) as GameObject;
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

        /// <summary>
        /// Firsts the scene load.
        /// </summary>
        protected virtual void FirstSceneLoad ()
        {
            if (firstSceneName == "") {
                return;
            }

            Screen (firstSceneName);
        }

        /// <summary>
        /// Loads the level.
        /// </summary>
        /// <param name="loadLevelData">Load level data.</param>
        protected virtual void LoadLevel (LoadLevelData loadLevelData)
        {
            bool isAddtive = false;
            switch (loadLevelData.type) {
            case SceneType.MENU:
            case SceneType.POPUP:
                isAddtive = true;
            break;
            case SceneType.SCREEN:
            case SceneType.ADDSCREEN:
                ClearPopUp ();
            break;
            }

            SSceneApplication.LoadLevel (loadLevelData.sceneName, delegate (GameObject root) {
                root.transform.parent = scene.transform;
                root.transform.localPosition = Vector3.zero;

                SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
                foreach (Camera cam in sRoot.Cameras) {
                    AudioListener audio = cam.GetComponent<AudioListener> ();
                    if (audio != null) {
                        audio.enabled = false;
                    }

                    // ngui
                    if (uIType == UIType.NGUI) {
                        if (nGUICamera == null) {
                            if (cam.GetComponent ("UICamera") != null) {
                                cam.clearFlags = CameraClearFlags.Depth;
                                nGUICamera = Instantiate (cam.gameObject) as GameObject;
                                nGUICamera.name = "UICamera";
                                nGUICamera.transform.parent = gCamera.transform;
                                nGUICamera.transform.localPosition = Vector3.zero;
                                nGUICamera.SetActive (true);

                                cam.gameObject.SetActive (false);
                            }
                        } else {
                            if (loadLevelData.type != SceneType.POPUP && cam.GetComponent ("UICamera") != null) {
                                cam.gameObject.SetActive (false);
                            }
                        }
                    }
                }

                SSceneController ctrl = root.GetComponent<SSceneController> ();
                ctrl.active = loadLevelData.active;
                ctrl.deactive = loadLevelData.deactive;

                switch (loadLevelData.type) {
                case SceneType.SCREEN:
                case SceneType.ADDSCREEN:
                    ctrl.OnSet (loadLevelData.data);
                    scenes.Add (loadLevelData.sceneName, root);
                    ClearScene (loadLevelData.sceneName);

                    if (screenStartChange != null) {
                        screenStartChange (loadLevelData.sceneName);
                    }

                    if (loadLevelData.type == SceneType.ADDSCREEN) {
                        screens.Push (loadLevelData.sceneName);
                    }
                break;
                case SceneType.POPUP:
                    scenes.Add (loadLevelData.sceneName, root);
                    DistancePopUp (root);
                    popups.Push (loadLevelData.sceneName);

                    ctrl.OnSet (loadLevelData.data);
                break;
                case SceneType.MENU:
                    ctrl.OnSet (loadLevelData.data);
                    menus.Add (loadLevelData.sceneName, root);
                break;
                }

                // ngui
                if (nGUICamera != null) {
                    MonoBehaviour uicam = nGUICamera.GetComponent<MonoBehaviour> ();
                    uicam.enabled = false;
                    uicam.enabled = true;
                }
            }, isAddtive);
        }

        /// <summary>
        /// Destroies the scene.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
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

        /// <summary>
        /// Clears the scene.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
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

        /// <summary>
        /// Clears the pop up.
        /// </summary>
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

        /// <summary>
        /// Clears the menu.
        /// </summary>
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

        /// <summary>
        /// Lasts the shield.
        /// </summary>
        /// <returns>The shield.</returns>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        protected virtual GameObject LastShield (bool flag)
        {
            float depth = 0;
            GameObject shield = null;
            for (int i = shields.Count - 1; i >= 0; i--) {
                if (shields [i].activeSelf == flag) {
                    if (uIType == UIType.NGUI) { // ngui
                        Camera cam = Util.FindChildObject (shields [i], "Camera").GetComponent<Camera> ();
                        if (cam.depth > depth) {
                            depth = cam.depth;
                            shield = shields [i];
                        }
                    } else { // ugui
                        Canvas canv = shields [i].GetComponent<Canvas> ();
                        if (canv.sortingOrder > depth) {
                            depth = canv.sortingOrder;
                            shield = shields [i];
                        }
                    }
                }
            }

            return shield;
        }

        /// <summary>
        /// Clears the shield.
        /// </summary>
        protected virtual void ClearShield ()
        {
            foreach (GameObject sh in shields) {
                sh.SetActive (false);
            }
        }

        /// <summary>
        /// Distances the pop up.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void DistancePopUp (GameObject root)
        {
            int depth = POPUP_DEPTH;
            if (popups.Count > 0) {
                SSceneRoot lRoot = scenes [popups.Peek ()].GetComponent<SSceneRoot> ();
                if (uIType == UIType.NGUI) { // ngui
                    Camera lCam = lRoot.Cameras [lRoot.Cameras.Length - 1];
                    depth = (int)lCam.depth + 1;
                } else { // ugui
                    Canvas[] canvas = lRoot.GetComponentsInChildren<Canvas> ();
                    depth = canvas [canvas.Length - 1].sortingOrder + 1;
                }
            }

            int d = depth;
            if (uIType == UIType.NGUI) { // ngui
                SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
                foreach (Camera cam in sRoot.Cameras) {
                    d++;
                    cam.depth = d;

                    if (cam.GetComponent ("UICamera") != null) {
                        cam.clearFlags = CameraClearFlags.Depth;
                    }
                }
            } else { // ugui
                Canvas[] canvas = root.GetComponentsInChildren<Canvas> ();
                foreach (Canvas canv in canvas) {
                    d++;
                    canv.sortingOrder = d;
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
                // ugui
                string resource = "HellgateUGUIShield";
                // ngui
                if (uIType == UIType.NGUI) { 
                    resource = "HellgateNGUIShield";
                }

                gShield = Instantiate (Resources.Load (resource)) as GameObject;
                gShield.name = "Shield" + shields.Count;
                gShield.transform.SetParent (shield.transform);
                gShield.transform.localPosition = Vector3.zero;
                shields.Add (gShield);
            } else {
                gShield = LastShield (false);
                if (gShield != null) {
                    gShield.SetActive (true);
                }
            }

            if (uIType == UIType.NGUI) { // ngui
                int x = (popups.Count + 1) * DISTANCE;
                root.transform.localPosition = new Vector3 (x, 0, 0);

                gShield.transform.localPosition = new Vector3 (x, -DISTANCE, 0);
                Camera sCam = gShield.GetComponentInChildren<Camera> ();
                sCam.depth = depth;

                MeshRenderer mesh = gShield.GetComponentInChildren<MeshRenderer> ();
                mesh.material.color = defaultShieldColor;
            } else { // ugui
                Canvas canv = gShield.GetComponentInChildren<Canvas> ();
                canv.sortingOrder = depth;

                Image image = gShield.GetComponentInChildren<Image> ();
                image.color = defaultShieldColor;
            }
        }

        /// <summary>
        /// Raises the active screen event.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void OnActiveScreen (GameObject root)
        {
            root.SetActive (true);

            if (sceneOnActive != null) {
                sceneOnActive (root.name);
            }
        }

        /// <summary>
        /// Raises the deative screen event.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void OnDeativeScreen (GameObject root)
        {
            root.SetActive (false);
        }

        /// <summary>
        /// Screen the specified sceneName, data, active and deactive.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void Screen (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (scenes.ContainsKey (sceneName)) {
                GameObject root = scenes [sceneName];
                if (!root.activeSelf) {
                    OnActiveScreen (root);
                    ClearScene (sceneName);
                    return;
                }
            }

            LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
            LoadLevel (loadLevel);
        }

        /// <summary>
        /// Adds the screen. management, stack and Back() shutdown function
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void AddScreen (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (scenes.ContainsKey (sceneName)) {
                GameObject root = scenes [sceneName];
                if (!root.activeSelf) {
                    OnActiveScreen (root);
                    ClearScene (sceneName);
                    screens.Push (sceneName);
                    return;
                }
            }

            LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
            loadLevel.type = SceneType.ADDSCREEN;
            LoadLevel (loadLevel);
        }

        /// <summary>
        /// Pops up. management, stack and Close() shutdown function
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
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

        /// <summary>
        /// Loads the menu.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
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

        /// <summary>
        /// Reboot this game.
        /// </summary>
        public virtual void Reboot ()
        {
            ClearMenu ();
            ClearScene ();
            FirstSceneLoad ();
        }

        /// <summary>
        /// Close this pop up only.
        /// </summary>
        public virtual void Close (CallbackDelegate callback = null)
        {
            if (popups.Count <= 0) {
                return;
            }

            string sceneName = popups.Peek ();
            GameObject root = scenes [sceneName];
            SSceneController ctrl = root.GetComponent<SSceneController> ();
            ctrl.callback = callback;

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

        /// <summary>
        /// Back the specified active and deactive. add screen only.
        /// </summary>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void Back (SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (screens.Count <= 1) {
                return;
            }

            screens.Pop ();
            string sceneName = screens.Peek ();

            Screen (sceneName, null, active, deactive);
        }

        /// <summary>
        /// Destroies the scenes from. (Except popup)
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public virtual void DestroyScenesFrom (string sceneName)
        {
            if (popups.Contains (sceneName)) {
                return;
            }

            DestroyScene (sceneName);
        }
    }
}
