//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Hellgate;

public class HellgateAssetData
{
    private string[] sprite = null;
    private string[] prefab = null;

    public string[] _Sprite {
        get {
            return sprite;
        }
    }

    public string[] Prefab {
        get {
            return prefab;
        }
    }
}

public class HellgateAssetEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject cam;
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject[] sprites;
    [SerializeField]
    private GameObject[] containers;
    private List<Sprite> pSprites;
    private List<GameObject> prefabs;
    private List<GameObject> cubes;
    private GameObject gObj;
    private GameObject cube;
    private Camera mCam;

    public static void GoAsset ()
    {
        LoadingJobData jobData = new LoadingJobData ();

        List<AssetBundleData> assetBundles = new List<AssetBundleData> ();
        assetBundles.Add (new AssetBundleData ("hellgatemaster", "asset", typeof(TextAsset)));

        jobData.assetBundles = assetBundles;
        jobData.finishedDelegate = delegate (List<object> obj, LoadingJobController job) {
            TextAsset text = Util.GetListObject<TextAsset> (obj);

//            HellgateAssetData data = Reflection.Convert<HellgateAssetData> ((IDictionary)Json.Deserialize (text.text));
            IDictionary iDic = (IDictionary)Json.Deserialize (text.text);
            HellgateAssetData data = iDic.Convert<HellgateAssetData> ();

            assetBundles = new List<AssetBundleData> ();
            for (int i = 0; i < data._Sprite.Length; i++) {
                assetBundles.Add (new AssetBundleData ("hellgateasset", data._Sprite [i], typeof(Sprite)));
            }

            for (int i = 0; i < data.Prefab.Length; i++) {
                assetBundles.Add (new AssetBundleData ("hellgateasset", data.Prefab [i]));
            }

            job.PutExtra ("title", "AssetBundle");

            job.nextSceneName = "HellgateAsset";
            job.LoadAssetBundle (assetBundles);
        };

        SceneManager.Instance.LoadingJob (jobData);
    }

    public override void OnSet (object data)
    {
        base.OnSet (data);

        gObj = gameObject;

        if (cam != null) {
            mCam = cam.GetComponent<Camera> ();
        }

        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);

        pSprites = Util.GetListObjects<Sprite> (objs);
        prefabs = Util.GetListObjects<GameObject> (objs);

        SetLabelTextValue (title, intent ["title"].ToString ());

        StartCoroutine (ViewAsset ());
    }

    public override void OnKeyBack ()
    {
        base.Quit ("Exit ?");
    }

    private IEnumerator ViewAsset ()
    {
        yield return new WaitForSeconds (0.5f);

        System.Random ran = new System.Random ();
        int sIndex = ran.Next (0, pSprites.Count);
        int pIndex = ran.Next (0, prefabs.Count);

        Set2DSpriteValue (sprites [0], pSprites [sIndex]);

        if (cube != null) {
            Destroy (cube);
        }

        Vector3 position = Vector3.zero;
        if (SceneManager.Instance._UIType == UIType.NGUI) {
            position = CameraUtil.GetViewportToWorldPoint (containers [0].gameObject, mCam);
        } else {
            position = containers [0].gameObject.transform.position;
        }

        cube = Instantiate (prefabs [pIndex]) as GameObject;
        cube.transform.localPosition = position;
        cube.transform.parent = gObj.transform;

        StartCoroutine (ViewAsset ());
    }

    private IEnumerator DelayDestory (GameObject gObj)
    {
        yield return new WaitForSeconds (2f);
        Destroy (gObj);
    }

    public void OnClickSprite ()
    {
        string[] loads = new string[] {
            "Inventory_Arrow", "Inventory_Bone"
        };

        System.Random ran = new System.Random ();

        AssetBundleData data = new AssetBundleData ("hellgateasset");
        data.objName = loads [ran.Next (0, loads.Length)];
        data.type = typeof(Sprite);

        AssetBundleManager.Instance.LoadAssetBundle (data, delegate(object obj) {
            Sprite temp = obj as Sprite;
            Set2DSpriteValue (sprites [1], temp);
        });
    }

    public void OnClickPrefab ()
    {
        string[] loads = new string[] {
            "CubeGreen", "CubeYellow"
        };

        List<AssetBundleData> datas = new List<AssetBundleData> ();
        for (int i = 0; i < loads.Length; i++) {
            AssetBundleData data = new AssetBundleData ("hellgateasset", loads [i]);
            datas.Add (data);
        }

        AssetBundleManager.Instance.LoadAssetBundle (datas, delegate(object obj) {
            List<object> objs = obj as List<object>;
            cubes = Util.GetListObjects<GameObject> (objs);

            System.Random ran = new System.Random ();

            Vector3 position = Vector3.zero;
            if (SceneManager.Instance._UIType == UIType.NGUI) {
                position = CameraUtil.GetViewportToWorldPoint (containers [1].gameObject, mCam);
            } else {
                position = containers [1].gameObject.transform.position;
            }

            GameObject cube = Instantiate (cubes [ran.Next (0, loads.Length)]) as GameObject;
            cube.transform.localPosition = position;
            cube.transform.parent = gObj.transform;

            StartCoroutine (DelayDestory (cube));
        });
    }
}
