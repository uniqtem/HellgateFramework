//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Hellgate
{
    public partial class WebView
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void _WebViewInit ();

        [DllImport ("__Internal")]
        private static extern void _WebViewLoadURL (string url);

        [DllImport ("__Internal")]
        private static extern void _WebViewDestroy ();

        [DllImport ("__Internal")]
        private static extern void _WebViewSetMargin (int left, int top, int right, int bottom);

        [DllImport ("__Internal")]
        private static extern void _WebViewSetVisibility (bool flag);

        [DllImport ("__Internal")]
        private static extern void _WebViewSetBackground (bool flag);

        [DllImport ("__Internal")]
        private static extern void _WebViewGoBack ();

        [DllImport ("__Internal")]
        private static extern void _WebViewGoForward ();

        [DllImport ("__Internal")]
        private static extern bool _WebViewCanGoBack ();

        [DllImport ("__Internal")]
        private static extern bool _WebViewCanGoForward ();

        [DllImport ("__Internal")]
        private static extern void _WebViewReload ();

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake ()
        {
            _WebViewInit ();
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
            _WebViewLoadURL (url);
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public virtual void Destroy ()
        {
            _WebViewDestroy ();
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
            _WebViewSetMargin (left, top, right, bottom);
        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public virtual void SetVisibility (bool flag)
        {
            _WebViewSetVisibility (flag);
        }

        /// <summary>
        /// Sets the background.
        /// true : white background.
        /// false : none background.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public virtual void SetBackground (bool flag)
        {
            _WebViewSetBackground (flag);
        }

        /// <summary>
        /// Gos the back.
        /// </summary>
        public virtual void GoBack ()
        {
            _WebViewGoBack ();
        }

        /// <summary>
        /// Gos the forward.
        /// </summary>
        public virtual void GoForward ()
        {
            _WebViewGoForward ();
        }

        /// <summary>
        /// Determines whether this instance can go back.
        /// </summary>
        /// <returns><c>true</c> if this instance can go back; otherwise, <c>false</c>.</returns>
        public virtual bool CanGoBack ()
        {
            return _WebViewCanGoBack ();
        }

        /// <summary>
        /// Determines whether this instance can go forward.
        /// </summary>
        /// <returns><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</returns>
        public virtual bool CanGoForward ()
        {
            return _WebViewCanGoForward ();
        }

        /// <summary>
        /// Reload.
        /// </summary>
        public virtual void Reload ()
        {
            _WebViewReload ();
        }
#endif
    }
}
