//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif

namespace Hellgate
{
    public class HttpManager : MonoBehaviour
    {
#region Const

        protected const string httpManager = "HttpManager";

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
                    gObj.name = httpManager;

#if !UNITY_5_3_OR_NEWER
                    DontDestroyOnLoad (gObj);
#endif
                }

                return instance;
            }
        }

#endregion

#region Delegate

        /// <summary>
        /// Response delegate.
        /// </summary>
        public delegate TResult TResultDelegate<out TResult> (object obj);

#endregion

#region Static

        /// <summary>
        /// The pre reuqest.
        /// </summary>
        protected static TResultDelegate<HttpData> preReuqest;

        /// <summary>
        /// The response.
        /// </summary>
        protected static TResultDelegate<bool> response = null;

#endregion

#region SerializeField

        [SerializeField]
        protected bool debug = false;

#endregion

#if UNITY_5_4_OR_NEWER
        protected UnityWebRequest www;
#else
        protected WWW www;
#endif

        protected HttpData data;
        protected SSceneController popUp;
        protected float time;

        /// <summary>
        /// Sets the pre request.
        /// To request a callback just before.
        /// </summary>
        /// <value>The pre request.</value>
        public TResultDelegate<HttpData> PreRequest {
            set {
                preReuqest = value;
            }
        }

        /// <summary>
        /// Sets the response.
        /// Return false if the do not HttpData.finishedDelegate.
        /// </summary>
        /// <value>The response.</value>
        public TResultDelegate<bool> Response {
            set {
                response = value;
            }
        }

        protected virtual void Awake ()
        {
            if (instance == null) {
                instance = this;

#if !UNITY_5_3_OR_NEWER
                DontDestroyOnLoad (gameObject);
#endif
            }

            popUp = null;
        }

        /// <summary>
        /// Debug the specified data and www.
        /// warring is timeover
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected void Log ()
        {
            time = Time.time - time;

            if (!debug) {
                return;
            }

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder ();
            stringBuilder.AppendFormat ("[Request time] {0}\n[URL] {1}\n", time, data.url);

            if (data.headers != null) {
                stringBuilder.Append ("[WWWForm.headers]\n");
                foreach (KeyValuePair<string, string> pair in data.headers) {
                    stringBuilder.AppendFormat ("{0} : {1}\n", pair.Key, pair.Value);
                }
                stringBuilder.Append ("\n");
            }

            if (data.headers != null) {
                stringBuilder.Append ("[WWWForm.data]\n");
                foreach (KeyValuePair<string, string> pair in data.datas) {
                    stringBuilder.AppendFormat ("{0} : {1}\n", pair.Key, pair.Value);
                }
                stringBuilder.Append ("\n");
            }

            if (www == null) {
                HDebug.LogWarning (stringBuilder.ToString ());
            } else if (www.error != null) {
                stringBuilder.AppendFormat ("[WWW.error]\n{0}\n", www.error);
                HDebug.LogError (stringBuilder.ToString ());
            } else {
                HDebug.Log (stringBuilder.ToString ());
            }
        }

        /// <summary>
        /// Callbacks the request.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void CallbackRequest ()
        {
            Log ();

            Action innerCallback = () => {
                popUp = null;
                if (response != null) {
                    if (!response (www)) {
                        return;
                    }
                }

                if (data.finishedDelegate != null) {
                    data.finishedDelegate (www);
                }
            };

            if (popUp != null) {
                SceneManager.Instance.Close (delegate {
                    innerCallback ();
                });
            } else {
                innerCallback ();
            }
        }

        /// <summary>
        /// Raises the fail event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void OnFail ()
        {
            CallbackRequest ();
        }

        /// <summary>
        /// Raises the disposed event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void OnDisposed ()
        {
            CallbackRequest ();
        }

        /// <summary>
        /// Raises the done event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void OnDone ()
        {
            CallbackRequest ();
        }

        /// <summary>
        /// Request the specified data and post.
        /// </summary>
        /// <param name="data">HttpData.</param>
        /// <param name="post">If set to <c>true</c> post.</param>
        public void Request (HttpData data, Http.FinishedDelegate finish = null)
        {
            time = Time.time;

            Action innerRequest = () => {
                if (preReuqest != null) {
                    data = preReuqest (data);
                }

                this.data = data;
                Http http;

#if UNITY_5_4_OR_NEWER
                if (data.method == UnityWebRequest.kHttpVerbPOST) {
#else
                if (data.post) {
#endif

                    http = new Http (this, data.url);
                    http.AddRangeData (data.datas);
                } else {
                    http = new Http (this, data.url, data.datas);
                }

#if UNITY_5_4_OR_NEWER
                http.method = data.method;
                http.audioType = data.audioType;

                http.OnFail = (UnityWebRequest www) => {
                    this.www = www;
                    OnFail ();
                };

                http.OnDisposed = (UnityWebRequest www) => {
                    this.www = www;
                    OnDisposed ();
                };

                http.OnDone = (UnityWebRequest www) => {
                    this.www = www;
                    OnDone ();
                };
#else
                http.OnFail = (WWW www) => {
                    this.www = www;
                    OnFail ();
                };

                http.OnDisposed = (WWW www) => {
                    this.www = www;
                    OnDisposed ();
                };

                http.OnDone = (WWW www) => {
                    this.www = www;
                    OnDone ();
                };
#endif

                // set headers
                http.Headers = data.headers;

                // set timeout time
                http.Timeout = data.timeout;

                http.Request ();
            };

            if (finish != null) {
                data.finishedDelegate = finish;
            }

            if (data.popUp) {
                if (SceneManager.Instance.DefaultLoadingJobSceneName != "") {
                    if (SceneManager.Instance.IsLoadingJob) {
                        HDebug.LogWarning ("Now loading scene active");
                    }

                    SceneManager.Instance.PopUp (SceneManager.Instance.DefaultLoadingJobSceneName, null, delegate(SSceneController ctrl) {
                        popUp = ctrl;
                        innerRequest ();
                    });
                } else {
                    HDebug.LogWarning ("The default loading scene is not set");
                }
            } else {
                innerRequest ();
            }
        }

        /// <summary>
        /// GET request the specified data.
        /// </summary>
        /// <param name="data">HttpData.</param>
        public void Get (HttpData data, Http.FinishedDelegate finish = null)
        {
#if UNITY_5_4_OR_NEWER
            data.method = UnityWebRequest.kHttpVerbGET;
#else
            data.post = false;
#endif

            Request (data, finish);
        }

        /// <summary>
        /// POST request the specified data.
        /// </summary>
        /// <param name="data">HttpData.</param>
        public void Post (HttpData data, Http.FinishedDelegate finish = null)
        {
#if UNITY_5_4_OR_NEWER
            data.method = UnityWebRequest.kHttpVerbPOST;
#else
            data.post = true;
#endif

            Request (data, finish);
        }

#if UNITY_5_4_OR_NEWER
        public void GetTexture (HttpData data, Http.FinishedDelegate finish = null)
        {
            data.method = Http.kHttpVerbTexture;
            Request (data, finish);
        }

        public void GetAudioClip (HttpData data, AudioType audioType, Http.FinishedDelegate finish = null)
        {
            data.method = Http.kHttpVerbAudioClip;
            data.audioType = audioType;

            Request (data, finish);
        }
#endif
    }
}
