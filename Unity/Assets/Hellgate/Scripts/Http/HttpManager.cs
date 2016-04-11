//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class HttpManager : MonoBehaviour
    {
#region Const

        protected const string HTTP_MANAGER = "HttpManager";

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

        protected SSceneController popUp;

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
        protected virtual void OnFail (HttpData data, WWW www)
        {
//            HDebug.Log ("Request OnFail " + www.error);
            CallbackRequest (data, www);
        }

        /// <summary>
        /// Raises the disposed event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void OnDisposed (HttpData data, WWW www)
        {
//            HDebug.Log ("Reuqest timeover");
            CallbackRequest (data, www);
        }

        /// <summary>
        /// Raises the done event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="www">Www.</param>
        protected virtual void OnDone (HttpData data, WWW www)
        {
//            HDebug.Log (data.post == true ? "post : " + www.url : "get : " + www.url);
//            if (data.post) {
//                string log = "";
//                foreach (KeyValuePair<string, string> kVP in data.datas) {
//                    log += kVP.Key + " : " + kVP.Value + " \n";
//                }
//                HDebug.Log (log);
//            }
//            HDebug.Log ("Requst good!!");
            CallbackRequest (data, www);
        }

        /// <summary>
        /// Request the specified data and post.
        /// </summary>
        /// <param name="data">HttpData.</param>
        /// <param name="post">If set to <c>true</c> post.</param>
        public void Request (HttpData data, bool post)
        {
            Action innerRequest = () => {
                if (preReuqest != null) {
                    data = preReuqest (data);
                }

                Http http;
                if (post) { // post reuqest
                    http = new Http (this, data.url);
                    http.Headers = data.headers;
                    foreach (KeyValuePair<string, string> kVP in data.datas) {
                        http.AddData (kVP.Key, kVP.Value);
                    }
                } else { // get request
                    http = new Http (this, data.url, data.datas);
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
                    HDebug.LogWarning ("The default loading scene is not set");
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
            Request (data);
        }

        /// <summary>
        /// POST request the specified data.
        /// </summary>
        /// <param name="data">HttpData.</param>
        public void POST (HttpData data)
        {
            data.post = true;
            Request (data);
        }
    }
}
