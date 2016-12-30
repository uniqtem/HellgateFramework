//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
    [RequireComponent (typeof(AutoSceneManager))]
    public class SceneManager : SSceneManager
    {
#region Singleton

        private static new SceneManager instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static new SceneManager Instance {
            get {
                return instance;
            }
        }

#endregion

#region Delegate

        public delegate void FinishedDelegate ();

#endregion

#region SerializeField

        /// <summary>
        /// The name of the loading job scene.
        /// </summary>
        [SerializeField]
        protected string LoadingJobSceneName;
        /// <summary>
        /// The name of the pop up scene.
        /// </summary>
        [SerializeField]
        protected string popUpSceneName;
        /// <summary>
        /// The name of the menu scene.
        /// </summary>
        [SerializeField]
        protected string menuSceneName;
        /// <summary>
        /// The editor local load asset bundle.
        /// </summary>
        [SerializeField]
        protected bool editorLocalLoadAssetBundle = true;
        /// <summary>
        /// The debug flag.
        /// </summary>
        [SerializeField]
        private bool showDebug = false;

#endregion

        /// <summary>
        /// The main menu.
        /// </summary>
        protected SSceneController mainMenu;
        /// <summary>
        /// The default shield alpha.
        /// </summary>
        protected float defaultShieldAlpha;
        /// <summary>
        /// The shield alpha.
        /// </summary>
        protected float shieldAlpha;
        /// <summary>
        /// The name of the now scene.
        /// </summary>
        protected string nowSceneName;
        /// <summary>
        /// The is loading job.
        /// </summary>
        protected bool isLoadingJob;

        /// <summary>
        /// Gets the default name of the loading job scene.
        /// </summary>
        /// <value>The default name of the loading job scene.</value>
        public string DefaultLoadingJobSceneName {
            get {
                return LoadingJobSceneName;
            }
        }

        /// <summary>
        /// Gets the default name of the pop up scene.
        /// </summary>
        /// <value>The default name of the pop up scene.</value>
        public string DefaultPopUpSceneName {
            get {
                return popUpSceneName;
            }
        }

        /// <summary>
        /// Gets the default name of the menu scene.
        /// </summary>
        /// <value>The default name of the menu scene.</value>
        public string DefaultMenuSceneName {
            get {
                return menuSceneName;
            }
        }

        /// <summary>
        /// Gets or sets the shield alpha.
        /// </summary>
        /// <value>The shield alpha.</value>
        public float ShieldAlpha {
            set {
                shieldAlpha = value;
            }
            get {
                return shieldAlpha;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hellgate.SceneManager"/> editor local load asset bundle.
        /// </summary>
        /// <value><c>true</c> if editor local load asset bundle; otherwise, <c>false</c>.</value>
        public bool EditorLocalLoadAssetBundle {
            get {
                return editorLocalLoadAssetBundle;
            }
        }

        /// <summary>
        /// Gets the now scene.
        /// </summary>
        /// <value>The now scene.</value>
        public string NowScene {
            get {
                return nowSceneName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is loading job.
        /// </summary>
        /// <value><c>true</c> if this instance is loading job; otherwise, <c>false</c>.</value>
        public bool IsLoadingJob {
            get {
                return isLoadingJob;
            }
        }

        protected override void Awake ()
        {
            base.Awake ();

            if (Application.isPlaying) {
                instance = this;

                defaultShieldAlpha = defaultShieldColor.a;
                shieldAlpha = defaultShieldAlpha;
            }
        }

        protected virtual void Start ()
        {
            if (showDebug) {
                if (!gameObject.GetComponent<HDebug> ()) {
                    gameObject.AddComponent<HDebug> ();
                }
            } else {
                Destroy (gameObject.GetComponent<HDebug> ());
            }
        }

        /// <summary>
        /// Screen the specified sceneName, data, active and deactive.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public override void Screen (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            nowSceneName = sceneName;

            base.Screen (sceneName, data, active, deactive);
        }

        /// <summary>
        /// Pops up.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public override void PopUp (string sceneName, object data = null, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            defaultShieldColor.a = shieldAlpha;

            base.PopUp (sceneName, data, active, deactive);

            shieldAlpha = defaultShieldAlpha;
        }

        /// <summary>
        /// Pops up.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="type">Type.</param>
        /// <param name="finished">Finished.</param>
        public virtual void PopUp (string text, PopUpType type, PopUpController.FinishedDelegate finished = null)
        {
            PopUp (popUpSceneName, new PopUpData (text, type), delegate(SSceneController ctrl) {
                PopUpController popUp = (PopUpController)ctrl;
                popUp.finishedDelegate = finished;
            });
        }

        /// <summary>
        /// Pops up.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="finished">Finished.</param>
        public virtual void PopUp (string text, PopUpController.FinishedDelegate finished)
        {
            PopUp (text, PopUpType.YesAndNo, finished);
        }

        /// <summary>
        /// Pops up.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="finished">Finished.</param>
        public virtual void PopUp (string text, CallbackDelegate finished)
        {
            PopUp (text, PopUpType.Ok, delegate(PopUpYNType type) {
                finished ();
            });
        }

        /// <summary>
        /// Loadings the job.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="popUp">If set to <c>true</c> pop up.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void LoadingJob (LoadingJobData data, bool popUp, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (LoadingJobSceneName == "") {
                HDebug.LogWarning ("The default loading job scene is not set");
                return;
            }

            data.popUp = popUp;
            if (popUp) {
                if (!data.active) {
                    shieldAlpha = 0f;
                }

                PopUp (LoadingJobSceneName, data, active, deactive);
            } else {
                Screen (LoadingJobSceneName, data, active, deactive);
            }
        }

        /// <summary>
        /// Loadings the job.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void LoadingJob (LoadingJobData data, SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            isLoadingJob = true;
            LoadingJob (data, data.popUp, active, delegate(SSceneController ctrl) {
                isLoadingJob = false;
                if (deactive != null) {
                    deactive (ctrl);
                }
            });
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void LoadMainMenu (SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (menuSceneName == "") {
//                HDebug.LogWarning ("The default menu scene is not set");
                return;
            }

            LoadMenu (menuSceneName, null, delegate(SSceneController ctrl) {
                mainMenu = ctrl;
                if (active != null) {
                    active (ctrl);
                }
            }, deactive);
        }

        /// <summary>
        /// Destories the main menu.
        /// </summary>
        public virtual void DestoryMainMenu ()
        {
            if (menuSceneName == "") {
                return;
            }

            if (mainMenu != null) {
                DestroyScenesFrom (menuSceneName);
                mainMenu = null;
            }
        }

        /// <summary>
        /// Hides the main menu.
        /// </summary>
        public virtual void HideMainMenu ()
        {
            if (mainMenu != null) {
                mainMenu.gameObject.SetActive (false);
            }
        }

        /// <summary>
        /// Shows the main menu.
        /// </summary>
        public virtual void ShowMainMenu ()
        {
            if (mainMenu != null) {
                mainMenu.gameObject.SetActive (true);
            }
        }

        /// <summary>
        /// wait.
        /// </summary>
        /// <returns>The wait.</returns>
        /// <param name="secondes">Secondes.</param>
        /// <param name="callback">Callback.</param>
        public IEnumerator CWait (float secondes, FinishedDelegate callback)
        {
            yield return new WaitForSeconds (secondes);
            callback ();
        }

        /// <summary>
        /// Wait the specified duration and callback.
        /// </summary>
        /// <param name="duration">Duration.</param>
        /// <param name="callback">Callback.</param>
        public void Wait (float duration, FinishedDelegate callback)
        {
            this.StartCoroutine (CWait (duration, callback));
        }
    }
}
