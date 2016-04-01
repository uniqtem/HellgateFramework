//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Framework namespace. using Hellgate;
/// </summary>
namespace Hellgate
{
    public class AssetBundleClient
    {
#region AssetBundleRef

        private class AssetBundleRef
        {
            public AssetBundle assetBundle = null;
            public string url;
            public int version;

            public AssetBundleRef (string u, int v)
            {
                url = u;
                version = v;
            }
        }

#endregion

#region Delegate

        public delegate void FinishedDelegate (object obj);

#endregion

#region Static

        private static Dictionary<string, AssetBundleRef> dictionaryAssetBundleRef;

#endregion

        private float progress;

        /// <summary>
        /// Gets the progress. Assetbundle download progress.
        /// 0 to 1.
        /// </summary>
        /// <value>The progress.</value>
        public float Progress {
            get {
                return progress;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleClient"/> class.
        /// </summary>
        public AssetBundleClient ()
        {
            dictionaryAssetBundleRef = new Dictionary<string, AssetBundleRef> ();
        }

        /// <summary>
        /// Gets the asset bundle.
        /// </summary>
        /// <returns>The asset bundle.</returns>
        /// <param name="url">URL.</param>
        /// <param name="version">Version.</param>
        public AssetBundle GetAssetBundle (string url, int version)
        {
            string keyName = url + version.ToString ();
            AssetBundleRef assetBundleRef;
            if (dictionaryAssetBundleRef.TryGetValue (keyName, out assetBundleRef)) {
                return assetBundleRef.assetBundle;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Downloads the asset bundle.
        /// </summary>
        /// <returns>The asset bundle.</returns>
        /// <param name="url">URL.</param>
        /// <param name="version">Version.</param>
        /// <param name="finished">Finished.</param>
        public IEnumerator DownloadAssetBundle (string url, int version, FinishedDelegate finished)
        {
            string keyName = url + version.ToString ();
            if (dictionaryAssetBundleRef.ContainsKey (keyName)) {
                yield return null;
            } else {
                while (!Caching.ready) {
                    yield return null;
                }

                using (WWW www = WWW.LoadFromCacheOrDownload (url, version)) {
                    while (!www.isDone) {
                        progress = www.progress;
                        yield return null;
                    }
                    if (www.error != null) {
                    } else {
                        AssetBundleRef assetBundleRef = new AssetBundleRef (url, version);
                        assetBundleRef.assetBundle = www.assetBundle;
                        dictionaryAssetBundleRef.Add (keyName, assetBundleRef);

                        yield return null;
                    }

                    if (finished != null) {
                        finished (www);
                    }

                    progress = 1f;
                    www.Dispose ();
                }
            }
        }

        /// <summary>
        /// Unload the specified url, version and allFlag.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="version">Version.</param>
        /// <param name="allFlag">If set to <c>true</c> all flag.</param>
        public void Unload (string url, int version, bool allFlag)
        {
            string keyName = url + version.ToString ();
            AssetBundleRef assetBundleRef;
            if (dictionaryAssetBundleRef.TryGetValue (keyName, out assetBundleRef)) {
                assetBundleRef.assetBundle.Unload (allFlag);
                assetBundleRef.assetBundle = null;
                dictionaryAssetBundleRef.Remove (keyName);
            }
        }

        /// <summary>
        /// Alls the unload.
        /// </summary>
        public void AllUnload ()
        {
            foreach (KeyValuePair<string, AssetBundleRef> kVP in dictionaryAssetBundleRef) {
                kVP.Value.assetBundle.Unload (true);
                kVP.Value.assetBundle = null;
            }

            dictionaryAssetBundleRef = new Dictionary<string, AssetBundleRef> ();
        }
    }
}
