//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;
using MiniJSON;

public class HellgateLoginEx : SceneController
{
	// const
	private const string BASE_URL = "https://dl.dropboxusercontent.com/u/95277951/hellgate/";
	[SerializeField]
	private GameObject
		status;
	[SerializeField]
	private GameObject
		progress;
	[SerializeField]
	private GameObject
		count;
	[SerializeField]
	private GameObject
		percent;
	private AssetBundleInitialDownloader aDownloader;
	private AssetBundleInitalStatus aStatus;
	private UISlider slider;
	private UILabel sLabel;
	private UILabel cLabel;
	private UILabel pLabel;
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

		// UI
		slider = progress.GetComponent<UISlider> ();
		sLabel = status.GetComponent<UILabel> ();
		cLabel = count.GetComponent<UILabel> ();
		pLabel = percent.GetComponent<UILabel> ();
		progress.SetActive (false);
		percent.SetActive (false);
		count.SetActive (false);

		goMainFlag = false;

		sLabel.text = "Checking maifest";
	}

	void Update ()
	{
		// AssetBundle status.
		if (aStatus == AssetBundleInitalStatus.Start && aDownloader != null) {
			slider.value = aDownloader.Progress;
			pLabel.text = aDownloader.SProgress;
			cLabel.text = aDownloader.CurretIndex + " / " + aDownloader.DownloadCount;
		}
	}

	public override void OnKeyBack ()
	{
		base.Quit ("Exit ?");
	}

	private void CallbackManifest (WWW www)
	{
		if (www == null) {
			sLabel.text = "Time over";
		} else if (www.error != null) {
			sLabel.text = "Server error";
		} else {
			sLabel.text = "Checking resource";
			
			HellgateManifestDataEx manifest = Reflection.Convert<HellgateManifestDataEx> ((IDictionary)Json.Deserialize (www.text));
			
			// Set max chacing.
			Caching.maximumAvailableDiskSpace = manifest.MaxChacing;
			
			// Set base url.
			HttpData.BASE_URL = manifest._Host.Game;
			
			aDownloader = new AssetBundleInitialDownloader (manifest._Host.Resource, manifest._Resource.Name + ".json");
			aDownloader.aEvent = CallbackDownloader;
			aDownloader.Download ();
		}
	}

	private void CallbackDownloader (AssetBundleInitalStatus status)
	{
		sLabel.text = "";
		this.aStatus = status;

		if (status == AssetBundleInitalStatus.HttpError) {
			HDebug.Log ("Download resource error");
		} else if (status == AssetBundleInitalStatus.HttpTimeover) {
			HDebug.Log ("Download timeover");
		} else if (status == AssetBundleInitalStatus.Start) {
			progress.SetActive (true);
			percent.SetActive (true);
			count.SetActive (true);
		} else {
			sLabel.text = "Please touch";
			
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
