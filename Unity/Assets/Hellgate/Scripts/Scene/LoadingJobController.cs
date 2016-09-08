//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class LoadingJobController : SceneController
    {
        /// <summary>
        /// The name of the next scene.
        /// </summary>
        [HideInInspector]
        public string nextSceneName;
        protected List<object> datas;
        protected Dictionary<string, object> httpData;
        protected Dictionary<string, object> assetBundleData;
        protected LoadingJobData jobData;
        protected int index;

        public override void OnSet (object data)
        {
            base.OnSet (data);

            jobData = null;
            if ((jobData = data as LoadingJobData) != null) {
                if (jobData.assetBundleAllUnload) {
                    AssetBundleManager.Instance.AllUnload ();
                }

                if (!jobData.active) {
                    GameObject gObj = gameObject;
                    foreach (Transform trans in gObj.GetComponentInChildren<Transform> ()) {
                        if (trans == gObj.transform) {
                            continue;
                        }

                        trans.gameObject.SetActive (jobData.active);
                    }
                }

                nextSceneName = jobData.nextSceneName;

                datas = new List<object> ();
                httpData = new Dictionary<string, object> ();
                assetBundleData = new Dictionary<string, object> ();

                index = 0;
                Request ();
            }
        }

        public override void OnReset (object data)
        {
            base.OnReset (data);
            OnSet (data);
        }

        public override void OnKeyBack ()
        {
        }

        private void OnNextScene ()
        {
            if (jobData.nextScenePopUp) {
                if (jobData.shieldAlpha >= 0) {
                    SceneManager.Instance.ShieldAlpha = jobData.shieldAlpha;
                }

                SceneManager.Instance.PopUp (nextSceneName, datas, active, deactive);
            } else {
                SceneManager.Instance.Screen (nextSceneName, datas, active, deactive);

                if (jobData.status == MainMenuStatus.Show) {
                    if (MenuController.Instance == null) {
                        SceneManager.Instance.LoadMainMenu ();
                    } else {
                        SceneManager.Instance.ShowMainMenu ();
                    }
                } else if (jobData.status == MainMenuStatus.Hide) {
                    SceneManager.Instance.HideMainMenu ();
                } else {
                    SceneManager.Instance.DestoryMainMenu ();
                }
            }
        }

        /// <summary>
        /// Request the specified list.
        /// </summary>
        /// <param name="list">List<HttpData>.</param>
        public void Request (List<HttpData> list = null)
        {
            if (list != null) {
                index = 0;
                jobData.https = list;
            }

            if (jobData.https.Count <= 0 || jobData.https.Count <= index) {
                if (httpData.Count > 0) {
                    datas.AddRange (new List<object> (httpData.Values));
                    httpData = new Dictionary<string, object> ();

                    if (jobData.lEvent != null) {
                        jobData.lEvent (LoadingJobStatus.HttpOver, this);
                    }
                }

                if (list == null) {
                    index = 0;
                    LoadAssetBundle ();
                } else {
                    GoNextScene ();
                }

                return;
            }

            string key = jobData.https [index].url;
            if (jobData.https [index].post) {
                foreach (KeyValuePair<string, string> kVP in jobData.https [index].datas) {
                    key += kVP.Key + kVP.Value;
                }
            }

            if (httpData.ContainsKey (key)) {
                index++;
                Request ();
            } else {
                jobData.https [index].finishedDelegate = delegate (WWW www) {
                    if (www == null) {
                        if (jobData.lEvent != null) {
                            jobData.lEvent (LoadingJobStatus.HttpTimeover, this);
                        }
                    } else if (www.error != null) {
                        if (jobData.lEvent != null) {
                            jobData.lEvent (LoadingJobStatus.HttpError, this);
                        }
                    } else {
                        httpData.Add (key, www);

                        index++;
                        Request ();
                    }
                };

                jobData.https [index].popUp = false;
                HttpManager.Instance.Request (jobData.https [index]);
            }
        }

        /// <summary>
        /// Loads the asset bundle.
        /// </summary>
        /// <param name="list">List<AssetBundleData>.</param>
        public void LoadAssetBundle (List<AssetBundleData> list = null)
        {
            System.Action innerLoadAssetBundle = delegate() {
                index++;
                LoadAssetBundle ();
            };

            if (list != null) {
                index = 0;
                jobData.assetBundles = list;
            }

            if (jobData.assetBundles.Count <= 0 || jobData.assetBundles.Count <= index) {
                if (assetBundleData.Count > 0) {
                    datas.AddRange (new List<object> (assetBundleData.Values));
                    assetBundleData = new Dictionary<string, object> ();
                }

                GoNextScene ();
                return;
            }

            if (jobData.assetBundles [index] == null) {
                innerLoadAssetBundle ();
                return;
            }

            string key = jobData.assetBundles [index].url + jobData.assetBundles [index].objName + jobData.assetBundles [index].type.ToString ();
            if (assetBundleData.ContainsKey (key)) {
                innerLoadAssetBundle ();
            } else {
                AssetBundleManager.Instance.LoadAssetBundle (jobData.assetBundles [index], delegate(object obj) {
                    assetBundleData.Add (key, obj);

                    innerLoadAssetBundle ();
                });
            }
        }

        /// <summary>
        /// Puts the extra.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void PutExtra (string key, object value)
        {
            jobData.PutExtra (key, value);
        }

        private void GoNextScene ()
        {
            if (nextSceneName == "") {
                if (jobData.finishedDelegate != null) {
                    jobData.finishedDelegate (datas, this);
//                    jobData.finishedDelegate = null;

                    return;
                }
            }

            GoNextScene (nextSceneName);
        }

        /// <summary>
        /// Gos the next scene.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public void GoNextScene (string sceneName)
        {
            if (sceneName == "") {
                HDebug.LogWarning ("Setting the next scene name required");
                return;
            }

            nextSceneName = sceneName;
            if (jobData.intent.Count > 0) {
                datas.Add (jobData.intent);
            }

            if (jobData.popUp) {
                SceneManager.Instance.Close (delegate () {
                    OnNextScene ();
                });
            } else {
                OnNextScene ();
            }
        }
    }
}
