//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;

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
            Screen,
            AddScreen,
            Popup,
            Menu
        }

#endregion

#region Class

        protected class LoadLevelData
        {
            public string sceneName;
            public object data;
            public SceneCallbackDelegate active;
            public SceneCallbackDelegate deactive;
            public SceneType type = SceneType.Screen;

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
        protected const int distance = 10;
        /// <summary>
        /// Used for the calculation popUp depth.
        /// </summary>
        protected const int popupDepth = 1001;

#endregion

#region Delegate

        public delegate void SceneStartDelegate (string sceneName);

        public delegate void SceneActivedDelegate (string sceneName);

        public delegate void SceneDeactivedDelegate (string sceneName);

        /// <summary>
        /// The screen start change event.
        /// </summary>
        public SceneStartDelegate screenStartChange;
        /// <summary>
        /// The pop up start event.
        /// </summary>
        public SceneStartDelegate popUpStart;
        /// <summary>
        /// The menu start event.
        /// </summary>
        public SceneStartDelegate menuStart;
        /// <summary>
        /// The scene on active event.
        /// </summary>
        public SceneActivedDelegate sceneOnActive;
        /// <summary>
        /// The scene on deactive event.
        /// </summary>
        public SceneDeactivedDelegate sceneOnDeactive;

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
        /// The event system.
        /// </summary>
        protected GameObject eventSystem;

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

        /// <summary>
        /// Gets the event system.
        /// </summary>
        /// <value>The event system.</value>
        public UnityEngine.EventSystems.EventSystem EventSystem {
            get {
                return eventSystem == null ? null : eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ();
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

#if !UNITY_5_3_OR_NEWER
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
            switch (loadLevelData.type) {
            case SceneType.Popup:
            case SceneType.Screen:
            case SceneType.AddScreen:
                if (scenes.ContainsKey (loadLevelData.sceneName)) {
                    GameObject root = scenes [loadLevelData.sceneName];
                    if (!root.activeSelf) {
                        if (loadLevelData.type == SceneType.Popup) {
                            DistancePopUp (root);
                            popups.Push (loadLevelData.sceneName);
                        }

                        if (loadLevelData.type != SceneType.Popup) {
                            ClearScene (loadLevelData.sceneName);
                        }

                        if (loadLevelData.type == SceneType.AddScreen) {
                            screens.Push (loadLevelData.sceneName);
                        }
                    }

                    OnActiveScreen (root);

                    SSceneController ctrl = root.GetComponent<SSceneController> ();
                    ctrl.OnReset (loadLevelData.data);
                    return;
                }

                // ugui
                if (uIType == UIType.UGUI) {
                    if (loadLevelData.type != SceneType.Popup) {
                        ClearEventSystem (loadLevelData.sceneName);
                    }
                }
            break;
            case SceneType.Menu:
                if (menus.ContainsKey (loadLevelData.sceneName)) {
                    GameObject root = menus [loadLevelData.sceneName];
                    OnActiveScreen (root);
                    return;
                }
            break;
            }

            bool isAddtive = false;
            if (loadLevelData.type == SceneType.Menu || loadLevelData.type == SceneType.Popup) {
                isAddtive = true;
            }

            SSceneApplication.LoadLevel (loadLevelData.sceneName, delegate(GameObject root) {
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
                            if (loadLevelData.type != SceneType.Popup && cam.GetComponent ("UICamera") != null) {
                                cam.gameObject.SetActive (false);
                            }
                        }
                    }
                }

                if (sRoot.EventSystem != null) {
                    eventSystem = sRoot.EventSystem.gameObject;
                }

                SSceneController ctrl = root.GetComponent<SSceneController> ();
                if (ctrl == null) {
                    HDebug.LogError ("No SceneController.");
                    return;
                }

                ctrl.active = loadLevelData.active;
                ctrl.deactive = loadLevelData.deactive;

                switch (loadLevelData.type) {
                case SceneType.Screen:
                case SceneType.AddScreen:
                    ctrl.OnSet (loadLevelData.data);
                    scenes.Add (loadLevelData.sceneName, root);
                    ClearScene (loadLevelData.sceneName);

                    if (screenStartChange != null) {
                        screenStartChange (loadLevelData.sceneName);
                    }

                    if (loadLevelData.type == SceneType.AddScreen) {
                        screens.Push (loadLevelData.sceneName);
                    }
                break;
                case SceneType.Popup:
                    scenes.Add (loadLevelData.sceneName, root);
                    DistancePopUp (root);
                    popups.Push (loadLevelData.sceneName);

                    ctrl.OnSet (loadLevelData.data);

                    if (popUpStart != null) {
                        popUpStart (loadLevelData.sceneName);
                    }
                break;
                case SceneType.Menu:
                    ctrl.OnSet (loadLevelData.data);
                    menus.Add (loadLevelData.sceneName, root);

                    if (menuStart != null) {
                        menuStart (loadLevelData.sceneName);
                    }
                break;
                }

                if (uIType == UIType.NGUI) { // ngui
                    if (nGUICamera != null) {
                        MonoBehaviour uicam = nGUICamera.GetComponent<MonoBehaviour> ();
                        uicam.enabled = false;
                        uicam.enabled = true;
                    }
                }
            }, isAddtive);
        }

        /// <summary>
        /// Clears the event system.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        protected virtual void ClearEventSystem (string sceneName)
        {
            GameObject root = null;
            Action innerClear = () => {
                SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
                if (sRoot.EventSystem != null) {
                    sRoot.EventSystem.gameObject.SetActive (false);
                }
            };

            foreach (var pair in scenes) {
                if (pair.Key == sceneName) {
                    continue;
                }

                root = pair.Value;
                innerClear ();
            }
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
                if (pair.Key == sceneName) {
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
        protected virtual void ClearPopUp (CallbackDelegate callback = null)
        {
            if (popups.Count <= 0) {
                if (callback != null) {
                    callback ();
                }
                return;
            }

            Close (delegate() {
                ClearPopUp (callback);
            });
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
                Destroy (sh);
            }

            shields.Clear ();
        }

        /// <summary>
        /// Distances the pop up.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void DistancePopUp (GameObject root)
        {
            int depth = popupDepth;
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

            // ugui
            string resource = "HellgateUGUIShield";
            // ngui
            if (uIType == UIType.NGUI) { 
                resource = "HellgateNGUIShield";
            }
            GameObject gShield = Instantiate (Resources.Load (resource)) as GameObject;
            gShield.name = "Shield" + shields.Count;
            gShield.transform.SetParent (shield.transform);
            gShield.transform.localPosition = Vector3.zero;
            shields.Add (gShield);

            if (uIType == UIType.NGUI) { // ngui
                int x = (popups.Count + 1) * distance;
                root.transform.localPosition = new Vector3 (x, 0, 0);

                gShield.transform.localPosition = new Vector3 (x, -distance, 0);
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
        /// Sets the active.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        protected virtual void SetActive (GameObject root, bool flag)
        {
            root.SetActive (flag);

            if (flag) {
                if (sceneOnActive != null) {
                    sceneOnActive (root.name);
                }
            } else {
                if (sceneOnDeactive != null) {
                    sceneOnDeactive (root.name);
                }
            }

            // ugui
            if (uIType == UIType.UGUI) {
                SSceneRoot sRoot = root.GetComponent<SSceneRoot> ();
                if (sRoot.EventSystem != null) {
                    sRoot.EventSystem.gameObject.SetActive (flag);
                }
            }
        }

        /// <summary>
        /// Raises the active screen event.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void OnActiveScreen (GameObject root)
        {
            SetActive (root, true);
        }

        /// <summary>
        /// Raises the deative screen event.
        /// </summary>
        /// <param name="root">Root.</param>
        protected virtual void OnDeativeScreen (GameObject root)
        {
            SetActive (root, false);
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
            ClearPopUp (delegate() {
                LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
                LoadLevel (loadLevel);
            });
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
            ClearPopUp (delegate() {
                LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
                loadLevel.type = SceneType.AddScreen;
                LoadLevel (loadLevel);
            });
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
            LoadLevelData loadLevel = new LoadLevelData (sceneName, data, active, deactive);
            loadLevel.type = SceneType.Popup;
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
            loadLevel.type = SceneType.Menu;
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
            if (scenes.ContainsKey (sceneName)) {
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
                    shields.Remove (shield);
                    Destroy (shield);
                }
            } else {
                if (callback != null) {
                    callback ();
                }
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
