//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

namespace Hellgate
{
    public partial class WebView : MonoBehaviour
    {
#if UNITY_EDITOR || UNITY_ANDROID
//#if !UNITY_EDITOR
        /// <summary>
        /// Android class name.
        /// </summary>
        protected const string CLASS_NAME = "com.hellgate.UnityWebViewActivity";

        /// <summary>
        /// The AndroidJavaObject.
        /// </summary>
        protected AndroidJavaObject android;

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake ()
        {
            if (Application.platform == RuntimePlatform.Android) {
                android = new AndroidJavaObject (CLASS_NAME);
                android.Call ("init");
            }
        }

        /// <summary>
        /// Raises the destory event.
        /// </summary>
        protected virtual void OnDestory ()
        {
            Destroy ();
        }

        /// <summary>
        /// Loads the URL.
        /// </summary>
        /// <param name="url">URL.</param>
        public virtual void LoadURL (string url)
        {
            if (android == null) {
                return;
            }

            android.Call ("loadURL", url);
        }
            
        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public virtual void Destroy ()
        {
            if (android == null) {
                return;
            }

            android.Call ("destroy");
        }

        /// <summary>
        /// Sets the margin.
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="top">Top.</param>
        /// <param name="right">Right.</param>
        /// <param name="bottom">Bottom.</param>
        public virtual void SetMargin (int left, int top, int right, int bottom)
        {
            if (android == null) {
                return;
            }

            android.Call ("setMargin", left, top, right, bottom);
        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public virtual void SetVisibility (bool flag)
        {
            if (android == null) {
                return;
            }

            android.Call ("setVisibility", flag);
        }

        /// <summary>
        /// Sets the background.
        /// true : white background.
        /// false : none background.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public virtual void SetBackground (bool flag)
        {
            if (android == null) {
                return;
            }

            android.Call ("setBackground", flag);
        }

        /// <summary>
        /// Gos the back.
        /// </summary>
        public virtual void GoBack ()
        {
            if (android == null) {
                return;
            }

            android.Call ("goBack");
        }

        /// <summary>
        /// Gos the forward.
        /// </summary>
        public virtual void GoForward ()
        {
            if (android == null) {
                return;
            }

            android.Call ("goForward");
        }

        /// <summary>
        /// Determines whether this instance can go back.
        /// </summary>
        /// <returns><c>true</c> if this instance can go back; otherwise, <c>false</c>.</returns>
        public virtual bool CanGoBack ()
        {
            if (android == null) {
                return false;
            }

            return android.Call<bool> ("canGoBack");
        }

        /// <summary>
        /// Determines whether this instance can go forward.
        /// </summary>
        /// <returns><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</returns>
        public virtual bool CanGoForward ()
        {
            if (android == null) {
                return false;
            }

            return android.Call<bool> ("canGoForward");
        }

        /// <summary>
        /// Reload.
        /// </summary>
        public virtual void Reload ()
        {
            if (android == null) {
                return;
            }

            android.Call ("reload");
        }
#endif
    }
}
