//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

namespace Hellgate
{
    public class WebViewManager : WebView
    {
#region Const

        protected const string WEBVIEW_MANAGER = "WebViewManager";

#endregion

#region Singleton

        private static WebViewManager instance = null;

        public static WebViewManager Instance {
            get {
                if (instance == null) {
                    GameObject gObj = new GameObject ();
                    instance = gObj.AddComponent<WebViewManager> ();
                    instance.Awake ();

                    gObj.name = WEBVIEW_MANAGER;
                    DontDestroyOnLoad (gObj);
                }

                return instance;
            }
        }

#endregion

        /// <summary>
        /// Occurs when progress received event.
        /// </summary>
        public event Action<int> ProgressReceivedEvent;

        /// <summary>
        /// Occurs when error received event.
        /// </summary>
        public event Action<string> ErrorReceivedEvent;

        protected override void Awake ()
        {
            if (instance == null) {
                base.Awake ();

                instance = this;
            }
        }

        protected override void OnDestory ()
        {
            base.OnDestory ();

            instance = null;
        }

        protected virtual void OnProgressChanged (string percent)
        {
            if (ProgressReceivedEvent != null) {
                ProgressReceivedEvent (int.Parse (percent));
            }
        }

        protected virtual void OnError (string message)
        {
            if (ErrorReceivedEvent != null) {
                ErrorReceivedEvent (message);
            }
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy ()
        {
            base.Destroy ();

            if (instance == null) {
                return;
            }

            GameObject.Destroy (gameObject);
        }

        /// <summary>
        /// Loads the URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="leftMargin">Left margin.</param>
        /// <param name="topMargin">Top margin.</param>
        /// <param name="rightMargin">Right margin.</param>
        /// <param name="bottomMargin">Bottom margin.</param>
        public void LoadURL (string url, int leftMargin, int topMargin, int rightMargin, int bottomMargin)
        {
            LoadURL (url);
            SetMargin (leftMargin, topMargin, rightMargin, bottomMargin);
            SetVisibility (true);
        }
    }
}
