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
    public class WebViewManager : MonoBehaviour
    {
#region Const

        protected const string WEBVIEW_MANAGER = "WebViewManager";
        protected const string CLASS_NAME = "com.hellgate.UnityRegister";

#endregion

#region Static

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern IntPtr _WebViewLoadURL (string url, int left, int right, int top, int bottom);

        [DllImport ("__Internal")]
        private static extern IntPtr _WebViewDestroy ();
#endif

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

        protected virtual void Awake ()
        {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad (this.gameObject);
            }
        }

        protected virtual void OnDestory ()
        {
            instance = null;
        }

        protected virtual void OnProgressChanged (string percent)
        {
            if (ProgressReceivedEvent != null) {
                ProgressReceivedEvent (int.Parse (percent));
            }
        }

        public void LoadURL (string url, int leftMargin = 0, int rightMargin = 0, int topMargin = 0, int bottomMargin = 0)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android) {
                using (AndroidJavaClass android = new AndroidJavaClass (CLASS_NAME)) {
                    android.CallStatic ("webViewLoadURL", url, leftMargin, rightMargin, topMargin, bottomMargin);
                }
            }
#endif
        }

        public void Destroy ()
        {
            if (instance == null) {
                return;
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android) {
                using (AndroidJavaClass android = new AndroidJavaClass (CLASS_NAME)) {
                    android.CallStatic ("webViewDestroy");
                }
            }
#endif

            GameObject.Destroy (gameObject);
        }
    }
}
