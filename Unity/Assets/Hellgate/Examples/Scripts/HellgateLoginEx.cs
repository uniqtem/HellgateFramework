//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;
using MiniJSON;

public class HellgateLoginEx : HellgateSceneControllerEx
{
    // const
    private const string BASE_URL = "https://dl.dropboxusercontent.com/u/95277951/hellgate/";
    [SerializeField]
    private GameObject status;
    [SerializeField]
    private GameObject progress;
    [SerializeField]
    private GameObject count;
    [SerializeField]
    private GameObject percent;
    [SerializeField]
    private GameObject button;
    private AssetBundleInitialDownloader aDownloader;
    private AssetBundleInitalStatus aStatus;
    private bool goMainFlag;

    public override void OnSet (object data)
    {
        base.OnSet (data);

        string encrypt = Encrypt.SHA1Key (BuildVersionBindings.GetBuildVersion () + "Hellgate");
//		HDebug.Log (encrypt);
        List<string> param = new List<string> ();
        param.Add (BASE_URL);
        param.Add (encrypt);
        param.Add (Util.GetDevice ());
        param.Add ("manifest");

        string url = Http.CreateURL (param, "json");
        HttpData hD = new HttpData (url);
        hD.popUp = false;
        hD.finishedDelegate = CallbackManifest;
        HttpManager.Instance.GET (hD);

        progress.SetActive (false);
        percent.SetActive (false);
        count.SetActive (false);

        goMainFlag = false;

        SetLabelTextValue (status, "Checking maifest");
    }

    void Update ()
    {
        // AssetBundle status.
        if (aStatus == AssetBundleInitalStatus.Start && aDownloader != null) {
            SetScrollValue (progress, aDownloader.Progress);
            SetLabelTextValue (percent, aDownloader.SProgress);
            SetLabelTextValue (count, aDownloader.CurretIndex + " / " + aDownloader.DownloadCount);
        }
    }

    public override void OnKeyBack ()
    {
        base.Quit ("Exit ?");
    }

    private void CallbackManifest (WWW www)
    {
        if (www == null) {
            SetLabelTextValue (status, "Time over");
//            sLabel.text = "Time over";
        } else if (www.error != null) {
            SetLabelTextValue (status, "Server error");
//            sLabel.text = "Server error";
        } else {
            SetLabelTextValue (status, "Checking resource");
//            sLabel.text = "Checking resource";

            HellgateManifestDataEx manifest = Reflection.Convert<HellgateManifestDataEx> ((IDictionary)Json.Deserialize (www.text));

            // Set max chacing.
            Caching.maximumAvailableDiskSpace = manifest.MaxChacing;

            // Set base url.
            HttpData.BASE_URL = manifest._Host.Game;

            List<string> list = new List<string> ();
            list.Add (manifest._Host.Resource);
            list.Add (manifest._Resource.Name);

            string url = Http.CreateURL (list, "json");

            aDownloader = new AssetBundleInitialDownloader (url, manifest._Host.Resource);
            aDownloader.aEvent = CallbackDownloader;
            aDownloader.Download ();
        }
    }

    private void CallbackDownloader (AssetBundleInitalStatus initStatus)
    {
        SetLabelTextValue (status, "");
        this.aStatus = initStatus;

        if (initStatus == AssetBundleInitalStatus.HttpError) {
            HDebug.Log ("Download resource error");
        } else if (initStatus == AssetBundleInitalStatus.HttpTimeover) {
            HDebug.Log ("Download timeover");
        } else if (initStatus == AssetBundleInitalStatus.Start) {
            progress.SetActive (true);
            percent.SetActive (true);
            count.SetActive (true);
        } else {
            SetLabelTextValue (this.status, "Please touch");

            progress.SetActive (false);
            percent.SetActive (false);
            count.SetActive (false);

            goMainFlag = true;
        }
    }

    public void OnClickButton ()
    {
        if (goMainFlag) {
            HellgateMainEx.GoMain ();
        }
    }
}
