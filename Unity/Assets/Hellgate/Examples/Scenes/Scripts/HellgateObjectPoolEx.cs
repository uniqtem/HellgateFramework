//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Hellgate;

public class HellgateObjectPoolData
{
	private string[] prefab = null;

	public string[] Prefab {
		get {
			return prefab;
		}
	}
}

public class HellgateObjectPoolEx : SceneController
{
	[SerializeField]
	private UILabel
		title;
	private List<GameObject> prefab;
	private int index;
	private bool dragFlag;

	public static void GoObjectPool ()
	{
		LoadingJobData jobData = new LoadingJobData ();

		List<AssetBundleData> assetBundles = new List<AssetBundleData> ();
		assetBundles.Add (new AssetBundleData ("hellgatemaster", "objectpool", typeof(TextAsset)));

		jobData.finishedDelegate = delegate(List<object> objs, LoadingJobController job) {
			TextAsset text = Util.GetListObject<TextAsset> (objs);

			assetBundles = new List<AssetBundleData> ();
			HellgateObjectPoolData data = Reflection.Convert<HellgateObjectPoolData> ((IDictionary)Json.Deserialize (text.text));
			for (int i = 0; i <  data.Prefab.Length; i++) {
				assetBundles.Add (new AssetBundleData ("hellgateobjectpool", data.Prefab [i]));
			}

			job.nextSceneName = "HellgateObjectPool";
			job.LoadAssetBundle (assetBundles);
		};

		jobData.assetBundles = assetBundles;
		jobData.PutExtra ("title", "ObjectPool");

		SceneManager.Instance.LoadingJob (jobData);
	}

	public override void OnSet (object data)
	{
		base.OnSet (data);

		List<object> objs = data as List<object>;
		Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);
		prefab = Util.GetListObjects<GameObject> (objs);

		title.text = intent ["title"].ToString ();
		index = 0;

		ObjectPoolManager.Init (prefab [0]);
		ObjectPoolManager.Init (prefab [1], 10);
	}

	public override void OnKeyBack ()
	{
		base.Quit ("Exit ?");
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			if (!CameraUtil.GetClickNGUI ()) {
				Create ();
			}
		}
	}

	private void Create ()
	{
		float x = Input.mousePosition.x;
		float y = Input.mousePosition.y;
		float z = MenuController.Instance.UI3D.farClipPlane / 2;
		Vector3 vector3 = MenuController.Instance.UI3D.ScreenToWorldPoint (new Vector3 (x, y, z));

		GameObject temp = ObjectPoolManager.Spawn (prefab [index], vector3, Quaternion.identity);
		ObjectPoolManager.DelayDespawn (temp, 2f);
	}

	public void OnClickObject1 ()
	{
		index = 0;
	}

	public void OnClickObject2 ()
	{
		index = 1;
	}

	public void OnClickObject3 ()
	{
		index = 2;
	}
}
