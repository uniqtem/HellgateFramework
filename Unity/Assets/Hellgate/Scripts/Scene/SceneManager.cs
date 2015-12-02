//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
	[RequireComponent(typeof(AutoSceneManager))]
	public class SceneManager : SSSceneManager
	{
#region Singleton
		private static SceneManager instance;

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

#region show Debug
		/// <summary>
		/// The debug flag.
		/// </summary>
		[SerializeField]
		private bool
			showDebug = false;

		void Start ()
		{
			GameObject gObj = gameObject;
			if (showDebug) {
				if (!gObj.GetComponent<Logger> ()) {
					gObj.AddComponent<Logger> ();
				}
			} else {
				Destroy (gObj.GetComponent<Logger> ());
			}
		}
#endregion

#region SerializeField
		/// <summary>
		/// The name of the loading job scene.
		/// </summary>
		[SerializeField]
		protected string
			LoadingJobSceneName;
		/// <summary>
		/// The name of the pop up scene.
		/// </summary>
		[SerializeField]
		protected string
			popUpSceneName;
		/// <summary>
		/// The name of the menu scene.
		/// </summary>
		[SerializeField]
		protected string
			menuSceneName;
#endregion

		protected SSController mainMenu;
		protected float defaultShieldAlpha;
		protected float shieldAlpha;
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

//				m_UIType = UIType.nGUI;
				defaultShieldAlpha = m_DefaultShieldColor.a;
				shieldAlpha = defaultShieldAlpha;
			}
		}

		private IEnumerator CWait (float secondes, FinishedDelegate callback)
		{
			yield return new WaitForSeconds (secondes);
			callback ();
		}

		/// <summary>
		/// Screen the specified sceneName, data, onActive and onDeactive.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="data">Data.</param>
		/// <param name="onActive">On active.</param>
		/// <param name="onDeactive">On deactive.</param>
		public override void Screen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
		{
			if (nowSceneName == sceneName) {
				base.Screen ("HellgateEmpty", null, delegate(SSController ctrl) {
					nowSceneName = "";
					Screen (sceneName, data, onActive, onDeactive);
				});

				return;
			}
			nowSceneName = sceneName;

			base.Screen (sceneName, data, onActive, onDeactive);
		}

		/// <summary>
		/// Pops up.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="data">Data.</param>
		/// <param name="onActive">On active.</param>
		/// <param name="onDeactive">On deactive.</param>
		public override void PopUp (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
		{
			m_DefaultShieldColor.a = shieldAlpha;

			base.PopUp (sceneName, data, onActive, onDeactive);

			shieldAlpha = defaultShieldAlpha;
		}

		/// <summary>
		/// Close the specified callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public virtual void Close (NoParamCallback callback = null)
		{
			Close (false, callback);
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
				Debug.LogWarning ("The default popup scene is not set");
				if (finished != null) {
					finished (PopUpYNType.Yes);
				}
				return;
			}

			PopUp (popUpSceneName, new PopUpData (text, type), delegate(SSController ctrl) {
				PopUpController popUp = (PopUpController)ctrl;
				popUp.finishedDelegate = finished;
			});
		}

		/// <summary>
		/// Loadings the job.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="popUp">If set to <c>true</c> pop up.</param>
		/// <param name="onActive">On active.</param>
		/// <param name="onDeactive">On deactive.</param>
		public virtual void LoadingJob (LoadingJobData data, bool popUp, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
		{
			if (LoadingJobSceneName == "") {
				Debug.LogWarning ("The default loading job scene is not set");
				return;
			}

			data.popUp = popUp;
			if (popUp) {
				PopUp (LoadingJobSceneName, data, onActive, onDeactive);
			} else {
				HideMainMenu ();
				Screen (LoadingJobSceneName, data, onActive, onDeactive);
			}
		}

		/// <summary>
		/// Loadings the job.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="onActive">On active.</param>
		/// <param name="onDeactive">On deactive.</param>
		public virtual void LoadingJob (LoadingJobData data, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
		{
			LoadingJob (data, data.popUp, onActive, onDeactive);
		}

		/// <summary>
		/// Loads the main menu.
		/// </summary>
		/// <param name="onActive">On active.</param>
		/// <param name="onDeactive">On deactive.</param>
		public virtual void LoadMainMenu (SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
		{
			if (menuSceneName == "") {
				Debug.LogWarning ("The default menu scene is not set");
				return;
			}

			LoadMenu (menuSceneName, null, delegate(SSController ctrl) {
				mainMenu = ctrl;
				if (onActive != null) {
					onActive (ctrl);
				}
			}, delegate(SSController ctrl) {
				mainMenu = null;
				if (onDeactive != null) {
					onDeactive (ctrl);
				}
			});
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
		/// Restert this instance.
		/// </summary>
		public virtual void Restert ()
		{
			DestoryMainMenu ();
			OnFirstSceneLoad ();
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
