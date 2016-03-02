using UnityEngine;
using System;
using System.Collections;

#if UNITY_IOS
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


        protected virtual void Awake ()
        {
            _WebViewInit ();
        }

        protected virtual void OnDestory ()
        {
            Destroy ();
        }

        public virtual void LoadURL (string url)
        {
            _WebViewLoadURL (url);
        }

        public virtual void Destroy ()
        {
            _WebViewDestroy ();
        }

        public virtual void SetMargin (int left, int top, int right, int bottom)
        {
            _WebViewSetMargin (left, top, right, bottom);
        }

        public virtual void SetVisibility (bool flag)
        {
            _WebViewSetVisibility (flag);
        }

        public virtual void SetBackground (bool flag)
        {
            _WebViewSetBackground (flag);
        }
#endif
    }
}
