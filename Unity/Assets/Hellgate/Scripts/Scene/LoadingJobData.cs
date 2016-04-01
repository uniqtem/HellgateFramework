//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    /// <summary>
    /// Loading job status.
    /// </summary>
    public enum LoadingJobStatus
    {
        DownloadOver = 1,
        HttpOver,
        HttpTimeover,
        HttpError
    }

    /// <summary>
    /// Main menu status.
    /// </summary>
    public enum MainMenuStatus
    {
        Show = 1,
        Hide
    }

    public class LoadingJobData
    {
#region Delegate

        public delegate void FinishedDelegate (List<object> obj,LoadingJobController job);

        public delegate void EventDelegate (LoadingJobStatus status,LoadingJobController job);

        /// <summary>
        /// The finished delegate.
        /// </summary>
        public FinishedDelegate finishedDelegate = null;
        /// <summary>
        /// Loading job status event.
        /// </summary>
        public EventDelegate lEvent = null;

#endregion

        /// <summary>
        /// Load to asset bundle list.
        /// </summary>
        public List<AssetBundleData> assetBundles;
        /// <summary>
        /// Request to http.
        /// </summary>
        public List<HttpData> https;
        /// <summary>
        /// Send to dictionary.
        /// </summary>
        public Dictionary<string, object> intent;
        /// <summary>
        /// Status main menu.
        /// </summary>
        public MainMenuStatus status = MainMenuStatus.Show;
        /// <summary>
        /// The name of the next scene.
        /// </summary>
        public string nextSceneName;
        /// <summary>
        /// The shield alpha.
        /// </summary>
        public float shieldAlpha = -1f;
        /// <summary>
        /// The loading job of pop up and screen.
        /// </summary>
        public bool popUp = true;
        /// <summary>
        /// The next scene of pop up and screen.
        /// </summary>
        public bool nextScenePopUp = false;
        /// <summary>
        /// The asset bundle async.
        /// </summary>
        public bool assetBundleasync = true;
        /// <summary>
        /// The asset bundle all unload.
        /// </summary>
        public bool assetBundleAllUnload = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.LoadingJobData"/> class.
        /// </summary>
        /// <param name="sceneName">Next scene name.</param>
        public LoadingJobData (string sceneName = "")
        {
            assetBundles = new List<AssetBundleData> ();
            https = new List<HttpData> ();
            intent = new Dictionary<string, object> ();

            nextSceneName = sceneName;
        }

        /// <summary>
        /// Puts the extra.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void PutExtra (string key, object value)
        {
            if (intent.ContainsKey (key)) {
                HDebug.LogWarning ("The key that already exists.");
            } else {
                intent.Add (key, value);
            }
        }
    }
}
