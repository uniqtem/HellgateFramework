//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
	public class HttpManager : MonoBehaviour
	{
#region Const
		private const string HTTP_MANAGER = "HttpManager";
#endregion

#region Singleton
		private static HttpManager instance = null;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static HttpManager Instance {
			get {
				if (instance == null) {
					GameObject gObj = new GameObject ();
					instance = gObj.AddComponent<HttpManager> ();
					gObj.name = HTTP_MANAGER;
					
					DontDestroyOnLoad (gObj);
				}
				
				return instance;
			}
		}
#endregion

		protected SSceneController popUp;

		protected virtual void Awake ()
		{
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad (this.gameObject);
			}

			popUp = null;
		}

		/// <summary>
		/// Callbacks the request.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="www">Www.</param>
		protected virtual void CallbackRequest (HttpData data, WWW www)
		{
			if (popUp != null) {
				SceneManager.Instance.Close ();

				if (data.finishedDelegate != null) {
					data.finishedDelegate (www);
				}
			} else {
				if (data.finishedDelegate != null) {
					data.finishedDelegate (www);
				}
			}

			popUp = null;
		}

		/// <summary>
		/// Raises the fail event.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="www">Www.</param>
		protected virtual void OnFail (HttpData data, WWW www)
		{
			Debug.Log ("Request OnFail " + www.error);
			CallbackRequest (data, www);
		}

		/// <summary>
		/// Raises the disposed event.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="www">Www.</param>
		protected virtual void OnDisposed (HttpData data, WWW www)
		{
			Debug.Log ("Reuqest timeover");
			CallbackRequest (data, www);
		}

		/// <summary>
		/// Raises the done event.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="www">Www.</param>
		protected virtual void OnDone (HttpData data, WWW www)
		{
			Debug.Log ("Requst good!!");
			CallbackRequest (data, www);
		}

		/// <summary>
		/// Request the specified data and post.
		/// </summary>
		/// <param name="data">HttpData.</param>
		/// <param name="post">If set to <c>true</c> post.</param>
		public void Request (HttpData data, bool post)
		{
			System.Action innerRequest = () => {
				Http http;
				if (post) { // post reuqest
					http = new Http (this, data.url);
					foreach (KeyValuePair<string, string> kVP in data.data) {
						http.AddData (kVP.Key, kVP.Value);
					}
				} else { // get request
					http = new Http (this, data.url, data.data);
				}

				// Set timeout time.
				if (data.timeout != 0) {
					http.Timeout = data.timeout;
				}
				
				http.OnFail = (WWW www) => {
					OnFail (data, www);
				};
				
				http.OnDisposed = (WWW www) => {
					OnDisposed (data, www);
				};
				
				http.OnDone = (WWW www) => {
					OnDone (data, www);
				};
				
				http.Request ();
			};

			if (data.popUp) {
				if (SceneManager.Instance.DefaultLoadingJobSceneName != "") {
					SceneManager.Instance.PopUp (SceneManager.Instance.DefaultLoadingJobSceneName, null, delegate(SSceneController ctrl) {
						popUp = ctrl;
						innerRequest ();
					});
				} else {
					Debug.LogWarning ("The default loading scene is not set");
					innerRequest ();
				}
			} else {
				innerRequest ();
			}
		}

		/// <summary>
		/// Request the specified data.
		/// </summary>
		/// <param name="data">HttpData.</param>
		public void Request (HttpData data)
		{
			Request (data, data.post);
		}

		/// <summary>
		/// GET request the specified data.
		/// </summary>
		/// <param name="data">HttpData.</param>
		public void GET (HttpData data)
		{
			Request (data, false);
		}

		/// <summary>
		/// POST request the specified data.
		/// </summary>
		/// <param name="data">HttpData.</param>
		public void POST (HttpData data)
		{
			Request (data, true);
		}
	}
}

