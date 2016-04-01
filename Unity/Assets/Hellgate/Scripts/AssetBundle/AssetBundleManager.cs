//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Hellgate
{
    public class AssetBundleManager : MonoBehaviour
    {
#region Const

        protected const string ASSET_BUNDLE_MANAGER = "AssetBundleManager";

#endregion

#region Singleton

        private static AssetBundleManager instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static AssetBundleManager Instance {
            get {
                if (instance == null) {
                    GameObject gObj = new GameObject ();
                    instance = gObj.AddComponent<AssetBundleManager> ();
                    gObj.name = ASSET_BUNDLE_MANAGER;

                    DontDestroyOnLoad (gObj);
                }
				
                return instance;
            }
        }

#endregion

        protected AssetBundleClient assetBundleClient;
        protected Dictionary<string, object> assetBundleData;

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public float Progress {
            get {
                return assetBundleClient.Progress;
            }
        }

        protected virtual void Awake ()
        {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad (this.gameObject);
            }

            assetBundleClient = new AssetBundleClient ();
        }

        /// <summary>
        /// Downloads the error.
        /// </summary>
        /// <param name="www">Www.</param>
        protected virtual void DownloadError (WWW www)
        {
            HDebug.LogError (www.url + " download error : " + www.error);
        }

        /// <summary>
        /// Textures the convert sprite.
        /// </summary>
        /// <returns>The convert sprite.</returns>
        /// <param name="obj">Object.</param>
        protected object TextureConvertSprite (object obj)
        {
            if (obj is Texture2D) {
                Texture2D tempTexture2D = obj as Texture2D;
                Sprite tempSprite = Sprite.Create (tempTexture2D, new Rect (0, 0, tempTexture2D.width, tempTexture2D.height), Vector2.zero);
                tempSprite.name = tempTexture2D.name;
                obj = tempSprite;
            }

            return obj;
        }

        /// <summary>
        /// Loads the asset async.
        /// </summary>
        /// <returns>The asset async.</returns>
        /// <param name="data">Data.</param>
        /// <param name="finished">Finished.</param>
        protected IEnumerator LoadAssetAsync (AssetBundleData data, AssetBundleClient.FinishedDelegate finished)
        {
            AssetBundleRequest request = data.assetBundle.LoadAssetAsync (data.objName, data.type);
            yield return request;

            object obj = request.asset as object;
            if (data.type == typeof(Sprite)) {
                obj = TextureConvertSprite (obj);
            }

            finished (obj);
        }

        /// <summary>
        /// Loads the asset.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="finished">Finished.</param>
        protected void LoadAsset (AssetBundleData data, AssetBundleClient.FinishedDelegate finished)
        {
            object obj = data.assetBundle.LoadAsset (data.objName, data.type) as object;
            if (data.type == typeof(Sprite)) {
                obj = TextureConvertSprite (obj);
            }

            finished (obj);
        }

        /// <summary>
        /// Loads the asset bundle.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="finished">Finished.</param>
        public void LoadAssetBundle (AssetBundleData data, AssetBundleClient.FinishedDelegate finished)
        {
            System.Action InnerLoadAssetBundle = () => {
                System.Action InnerLoadAsset = () => {
                    if (data.objName == string.Empty) {
                        HDebug.LogWarning ("Set the object name in the assetbundle.");
                        finished (null);
                        return;
                    }

                    if (data.async) {
                        StartCoroutine (LoadAssetAsync (data, finished));
                    } else {
                        LoadAsset (data, finished);
                    }
                };

                if (data.assetBundle == null) {
                    data.assetBundle = assetBundleClient.GetAssetBundle (data.url, data.version);
                    if (data.assetBundle == null) {
                        DownloadAssetBundle (data, delegate(object obj) {
                            WWW www = obj as WWW;
                            if (www.error != null) {
                                DownloadError (www);
                                return;
                            } else {
                                data.assetBundle = assetBundleClient.GetAssetBundle (data.url, data.version);
                            }

                            InnerLoadAsset ();
                        });
                    } else {
                        InnerLoadAsset ();
                    }
                } else {
                    InnerLoadAsset ();
                }
            };

#if UNITY_EDITOR
//          HDebug.Log (data.assetBundleName + " / " + data.objName);
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (data.assetBundleName, data.objName);
            string path = "";
            if (paths.Length <= 0) {
                paths = AssetDatabase.GetAssetPathsFromAssetBundle (data.assetBundleName);
                if (paths.Length <= 0) {
                    InnerLoadAssetBundle ();
                    return;
                }

                string[] files = Directory.GetFiles (paths [0], data.objName + ".*");
                if (files.Length <= 0) {
                    InnerLoadAssetBundle ();
                    return;
                }
                path = files [0];
            } else {
                path = paths [0];
            }

            object temp = AssetDatabase.LoadMainAssetAtPath (path);
            if (data.type == typeof(Sprite)) {
                temp = TextureConvertSprite (temp);
            }

            finished (temp);
#else
            InnerLoadAssetBundle ();
#endif
        }

        /// <summary>
        /// Loads the asset bundle.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="finished">Finished.</param>
        public void LoadAssetBundle (AssetBundleData[] list, AssetBundleClient.FinishedDelegate finished)
        {
            LoadAssetBundle (new List<AssetBundleData> (list), finished);
        }

        /// <summary>
        /// Loads the asset bundle.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="finished">Finished.</param>
        public void LoadAssetBundle (List<AssetBundleData> list, AssetBundleClient.FinishedDelegate finished)
        {
            if (assetBundleData == null) {
                assetBundleData = new Dictionary<string, object> ();
            }

            System.Action innerAssetBundle = () => {
                list.RemoveAt (0);
                if (list.Count <= 0) {
                    finished (new List<object> (assetBundleData.Values));
                    assetBundleData = null;
                } else {
                    LoadAssetBundle (list, finished);
                }
            };

            string key = list [0].url + list [0].objName + list [0].type.ToString ();
            if (assetBundleData.ContainsKey (key)) {
                innerAssetBundle ();
                return;
            }

            LoadAssetBundle (list [0], delegate(object obj) {
                if (assetBundleData == null) {
                    assetBundleData = new Dictionary<string, object> ();
                }

                assetBundleData.Add (key, obj);
                innerAssetBundle ();
            });
        }

        /// <summary>
        /// Downloads the asset bundle.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="finished">Finished.</param>
        public void DownloadAssetBundle (AssetBundleData data, AssetBundleClient.FinishedDelegate finished)
        {
            StartCoroutine (assetBundleClient.DownloadAssetBundle (data.url, data.version, finished));
        }

        /// <summary>
        /// Unload the specified url, version and flag.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="version">Version.</param>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public void Unload (string url, int version, bool flag = true)
        {
            assetBundleClient.Unload (url, version, flag);
        }

        /// <summary>
        /// Alls the unload.
        /// </summary>
        public void AllUnload ()
        {
            assetBundleClient.AllUnload ();
        }

        /// <summary>
        /// Cleans the cache.
        /// </summary>
        public void CleanCache ()
        {
            Caching.CleanCache ();
        }
    }
}
