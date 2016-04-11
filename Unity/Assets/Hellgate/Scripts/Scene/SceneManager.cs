//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
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

#endregion

#region show Debug

        /// <summary>
        /// The debug flag.
        /// </summary>
        [SerializeField]
        private bool showDebug = false;

        void Start ()
        {
            GameObject gObj = gameObject;
            if (showDebug) {
                if (!gObj.GetComponent<HDebug> ()) {
                    gObj.AddComponent<HDebug> ();
                }
            } else {
                Destroy (gObj.GetComponent<HDebug> ());
            }
        }

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

        protected override void Awake ()
        {
            base.Awake ();

            if (Application.isPlaying) {
                instance = this;

                defaultShieldAlpha = defaultShieldColor.a;
                shieldAlpha = defaultShieldAlpha;
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
            if (nowSceneName == sceneName) {
                base.Screen ("HellgateEmpty", null, delegate(SSceneController ctrl) {
                    nowSceneName = "";
                    base.Screen (sceneName, data, active, deactive);
                });

                return;
            }
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
            if (popUpSceneName == "") {
                HDebug.LogWarning ("The default popup scene is not set");
                if (finished != null) {
                    finished (PopUpYNType.Yes);
                }
                return;
            }

            PopUp (popUpSceneName, new PopUpData (text, type), delegate(SSceneController ctrl) {
                PopUpController popUp = (PopUpController)ctrl;
                popUp.finishedDelegate = finished;
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
            LoadingJob (data, data.popUp, active, deactive);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        /// <param name="active">Active.</param>
        /// <param name="deactive">Deactive.</param>
        public virtual void LoadMainMenu (SceneCallbackDelegate active = null, SceneCallbackDelegate deactive = null)
        {
            if (menuSceneName == "") {
                HDebug.LogWarning ("The default menu scene is not set");
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
