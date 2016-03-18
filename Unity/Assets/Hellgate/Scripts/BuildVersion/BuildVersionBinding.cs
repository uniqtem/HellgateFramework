//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Runtime.InteropServices;

namespace Hellgate
{
    public class BuildVersionBindings
    {
#if UNITY_ANDROID
        public static string AndroidBundleVersion {
            get {
                if (AbundleVersion == null) {
                    GetVersionInfo ();
                }

                return AbundleVersion;
            }
        }

        private static string AbundleVersion;

        private static void GetVersionInfo ()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity").Call<AndroidJavaObject> ("getApplicationContext");
            AndroidJavaObject pManager = context.Call<AndroidJavaObject> ("getPackageManager");
            AndroidJavaObject pInfo = pManager.Call<AndroidJavaObject> ("getPackageInfo",
                                                               context.Call<string> ("getPackageName"),
                                                               pManager.GetStatic<int> ("GET_ACTIVITIES"));

            AbundleVersion = pInfo.Get<string> ("versionName");
        }
#endif

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern string _GetCFBundleVersion ();
        [DllImport ("__Internal")]
        private static extern string _GetCFBundleShortVersionString ();

        public static string IOSBundleVersion {
            get {
                if (IbundleVersion == null) {
                    GetVersionInfo ();
                }
                return IbundleVersion;
            }
        }

        private static string IbundleVersion;

        private static void GetVersionInfo ()
        {
            IbundleVersion = _GetCFBundleShortVersionString ();
        }
#endif

        /// <summary>
        /// Gets the build version.
        /// </summary>
        /// <returns>The build version.</returns>
        public static string GetBuildVersion ()
        {
#if UNITY_EDITOR
            return UnityEditor.PlayerSettings.bundleVersion;
#elif UNITY_ANDROID
            return AndroidBundleVersion;
#elif UNITY_IOS
            return IOSBundleVersion;
#endif
        }
    }
}
