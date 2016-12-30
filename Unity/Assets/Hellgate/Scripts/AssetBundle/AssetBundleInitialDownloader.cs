//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif

namespace Hellgate
{
    public class AssetBundleInitialDownloader
    {
#region delegate

        public delegate void EventDelegate (AssetBundleInitalStatus status);

#if UNITY_5_4_OR_NEWER
        public delegate void ResponseDelegate (UnityWebRequest www);
#else
        public delegate void ResponseDelegate (WWW www);
#endif

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
        /// Examples url : http://www.uniqtem.net/hellgate/resource/pc/resource.json
        /// </summary>
        /// <param name="baseUrl">Base URL.</param>
        /// <param name="name">Name.</param>
        /// <param name="assetExtension">Asset extension.</param>
        public AssetBundleInitialDownloader (string url, string baseUrl = "", string assetExtension = "unity3d")
        {
            AssetBundleData.baseUrl = baseUrl;
            AssetBundleData.extension = assetExtension;

            assetBundleManager = AssetBundleManager.Instance;
            httpManager = HttpManager.Instance;

            httpData = new HttpData (url);
            httpData.popUp = false;

#if UNITY_5_4_OR_NEWER
            httpData.finishedDelegate = delegate(UnityWebRequest www) {
#else
            httpData.finishedDelegate = delegate(WWW www) {
#endif
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

#if UNITY_5_4_OR_NEWER
                DownloadAssetBundle (www.downloadHandler.text);
#else
                DownloadAssetBundle (www.text);
#endif
            };
        }

        /// <summary>
        /// Downloads the asset bundle.
        /// </summary>
        /// <param name="json">Json.</param>
        protected void DownloadAssetBundle (string json)
        {
            assetBundleData = JsonUtil.FromJson<AssetBundleInitialData> (json);
            if (assetBundleData._Resource.Major != Register.GetInt (assetBundleData._Resource.Name + AssetBundleInitialData.major)) {

#if UNITY_EDITOR
                HDebug.Log ("Major : " + assetBundleData._Resource.Major);
#endif

                downloads = assetBundleData._AssetBundle;

                // Delete all AssetBundle.
                assetBundleManager.CleanCache ();
            } else if (assetBundleData._Resource.Minor != Register.GetInt (assetBundleData._Resource.Name + AssetBundleInitialData.minor)) {

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
                Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.major, assetBundleData._Resource.Major);
                Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.minor, assetBundleData._Resource.Minor);

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
                    Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.major, assetBundleData._Resource.Major);
                    Register.SetInt (assetBundleData._Resource.Name + AssetBundleInitialData.minor, assetBundleData._Resource.Minor);

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
            httpManager.Get (httpData);
        }
    }
}
