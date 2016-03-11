//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace Hellgate
{
    public class AssetBundleInitialDownloader
    {
#region delegate

        public delegate void EventDelegate (AssetBundleInitalStatus status);

        public delegate void ResponseDelegate (WWW www);

#endregion

        /// <summary>
        /// AssetBundleInitalDownloader status event.
        /// </summary>
        public EventDelegate aEvent = null;
        /// <summary>
        /// The response.
        /// </summary>
        public ResponseDelegate response = null;
        protected List<AssetBundleInitialData.AssetBundle> downloads;
        protected HttpData httpData;
        protected HttpManager httpManager;
        protected AssetBundleInitialData assetBundleData;
        protected AssetBundleManager assetBundleManager;
        protected int index;

        /// <summary>
        /// Gets the String progress.
        /// </summary>
        /// <value>The S progress.</value>
        public string SProgress {
            get {
                return (assetBundleManager.Progress * 100f).ToString ("N2") + "%";
            }
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public float Progress {
            get {
                return assetBundleManager.Progress;
            }
        }

        /// <summary>
        /// Gets the download count.
        /// </summary>
        /// <value>The download count.</value>
        public int DownloadCount {
            get {
                if (downloads == null) {
                    return 0;
                } else {
                    return downloads.Count;
                }
            }
        }

        /// <summary>
        /// Gets the download index of the curret.
        /// </summary>
        /// <value>The index of the curret.</value>
        public int CurretIndex {
            get {
                return index;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleInitalDownloader"/> class.
        /// Examples url : https://dl.dropboxusercontent.com/u/95277951/hellgate/fd3caab6ae1a959fd769a2a3ed9344e5adc98402/pc/resource.json
        /// </summary>
        /// <param name="baseUrl">Base URL.</param>
        /// <param name="name">Name.</param>
        /// <param name="assetExtension">Asset extension.</param>
        public AssetBundleInitialDownloader (string url, string baseUrl = "", string assetExtension = "unity3d")
        {
            AssetBundleData.BASE_URL = baseUrl;
            AssetBundleData.EXTENSION = assetExtension;

            assetBundleManager = AssetBundleManager.Instance;
            httpManager = HttpManager.Instance;

            httpData = new HttpData (url);
            httpData.popUp = false;
            httpData.finishedDelegate = Callback;
        }

        /// <summary>
        /// Http Callback the specified www.
        /// </summary>
        /// <param name="www">Www.</param>
        protected void Callback (WWW www)
        {
            if (www == null) {
                if (aEvent != null) {
                    aEvent (AssetBundleInitalStatus.HttpTimeover);
                }
                return;
            } else if (www.error != null) {
                if (aEvent != null) {
                    aEvent (AssetBundleInitalStatus.HttpError);
                }
                return;
            }

            if (response != null) {
                response (www);
            }

            assetBundleData = Reflection.Convert<AssetBundleInitialData> ((IDictionary)Json.Deserialize (www.text));

            if (assetBundleData._Resource.Major != Register.GetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MAJOR)) {

#if UNITY_EDITOR
                HDebug.Log ("Major : " + assetBundleData._Resource.Major);
#endif

                downloads = assetBundleData._AssetBundle;

                // Delete all AssetBundle.
                assetBundleManager.CleanCache ();
            } else if (assetBundleData._Resource.Minor != Register.GetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MINOR)) {

#if UNITY_EDITOR
                HDebug.Log ("Minor : " + assetBundleData._Resource.Minor);
#endif

                downloads = new List<AssetBundleInitialData.AssetBundle> ();
                for (int i = 0; i < assetBundleData._AssetBundle.Count; i++) {
                    if (assetBundleData._AssetBundle [i]._Version != Register.GetInt (assetBundleData._AssetBundle [i].Name)) {
                        downloads.Add (assetBundleData._AssetBundle [i]);
                    }
                }
            } else {
                if (aEvent != null) {
                    aEvent (AssetBundleInitalStatus.Over);
                }
                return;
            }

            if (aEvent != null) {
                aEvent (AssetBundleInitalStatus.Start);
            }

            if (downloads.Count > 0) {
                index = 0;
                DownloadAssetBundle ();
            } else {
                HDebug.LogWarning ("None of this has changed assetbundle version.");
                Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MAJOR, assetBundleData._Resource.Major);
                Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MINOR, assetBundleData._Resource.Minor);

                if (aEvent != null) {
                    aEvent (AssetBundleInitalStatus.Over);
                }
            }
        }

        /// <summary>
        /// Downloads the asset bundle.
        /// </summary>
        protected void DownloadAssetBundle ()
        {
            string name = downloads [index].Name;
            int version = downloads [index]._Version;

            AssetBundleData aBD = new AssetBundleData (name, version);
            assetBundleManager.DownloadAssetBundle (aBD, delegate(object obj) {
                Register.SetInt (name, version);
                index++;

                if (index < downloads.Count) {
                    DownloadAssetBundle ();
                } else {
                    Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MAJOR, assetBundleData._Resource.Major);
                    Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.MINOR, assetBundleData._Resource.Minor);

                    if (aEvent != null) {
                        aEvent (AssetBundleInitalStatus.DownloadOver);
                    }
                }
            });
        }

        /// <summary>
        /// Download this instance.
        /// </summary>
        public void Download ()
        {
            httpManager.GET (httpData);
        }
    }
}
