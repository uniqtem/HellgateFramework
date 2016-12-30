//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Hellgate
{
    public class Clipboard
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void _SetText (string text);
#endif

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">Text.</param>
        public static void SetText (string text)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
            activity.Call ("runOnUiThread", new AndroidJavaRunnable (delegate() {
            AndroidJavaObject clipboard = activity.Call<AndroidJavaObject> ("getSystemService", "clipboard");
            AndroidJavaClass clipData = new AndroidJavaClass ("android.content.ClipData");
            AndroidJavaObject clip = clipData.CallStatic<AndroidJavaObject> ("newPlainText", "simple text", text);
            clipboard.Call ("setPrimaryClip", clip);
            }));
#elif !UNITY_EDITOR && UNITY_IOS
            _SetText (text);
#else
            #if !UNITY_5_0 && !UNITY_5_1
            GUIUtility.systemCopyBuffer = text;
            #endif
#endif
        }
    }
}
