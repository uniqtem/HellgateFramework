//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

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

        public event Action<int> ProgressReceivedEvent;

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

        public override void Destroy ()
        {
            base.Destroy ();

            if (instance == null) {
                return;
            }

            GameObject.Destroy (gameObject);
        }

        public void LoadURL (string url, int leftMargin, int topMargin, int rightMargin, int bottomMargin)
        {
            LoadURL (url);
            SetMargin (leftMargin, topMargin, rightMargin, bottomMargin);
            SetVisibility (true);
        }
    }
}
