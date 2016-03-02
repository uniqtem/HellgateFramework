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
        protected const string CLASS_NAME = "com.hellgate.UnityWebViewActivity";

        protected AndroidJavaObject android;

        protected virtual void Awake ()
        {
            if (Application.platform == RuntimePlatform.Android) {
                android = new AndroidJavaObject (CLASS_NAME);
                android.Call ("init");
            }
        }

        protected virtual void OnDestory ()
        {
            Destroy ();
        }

        public virtual void LoadURL (string url)
        {
            if (android == null) {
                return;
            }

            android.Call ("loadURL", url);
        }

        public virtual void Destroy ()
        {
            if (android == null) {
                return;
            }

            android.Call ("destroy");
        }

        public virtual void SetMargin (int left, int top, int right, int bottom)
        {
            if (android == null) {
                return;
            }

            android.Call ("setMargin", left, top, right, bottom);
        }

        public virtual void SetVisibility (bool flag)
        {
            if (android == null) {
                return;
            }

            android.Call ("setVisibility", flag);
        }

        public virtual void SetBackground (bool flag)
        {
            if (android == null) {
                return;
            }

            android.Call ("setBackground", flag);
        }
    }
}
