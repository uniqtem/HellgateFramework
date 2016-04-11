//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEngine.EventSystems;
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

public class HellgateObjectPoolEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject cam;
    [SerializeField]
    private GameObject title;
    private List<GameObject> prefab;
    private int index;
    private bool dragFlag;
    private Camera mCam;

    public static void GoObjectPool ()
    {
        LoadingJobData jobData = new LoadingJobData ();

        List<AssetBundleData> assetBundles = new List<AssetBundleData> ();
        assetBundles.Add (new AssetBundleData ("hellgatemaster", "objectpool", typeof(TextAsset)));

        jobData.finishedDelegate = delegate(List<object> objs, LoadingJobController job) {
            TextAsset text = Util.GetListObject<TextAsset> (objs);

            assetBundles = new List<AssetBundleData> ();
            HellgateObjectPoolData data = Reflection.Convert<HellgateObjectPoolData> ((IDictionary)Json.Deserialize (text.text));
            for (int i = 0; i < data.Prefab.Length; i++) {
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

        if (cam != null) {
            mCam = cam.GetComponent<Camera> ();
        }


        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);
        prefab = Util.GetListObjects<GameObject> (objs);

        SetLabelTextValue (title, intent ["title"].ToString ());
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
            if (SceneManager.Instance._UIType == UIType.NGUI) {
                if (!CameraUtil.GetClickNGUI ()) {
                    Create ();
                }
            } else {
                if (!EventSystem.current.IsPointerOverGameObject ()) {
                    Create ();
                }
            }
        }
    }

    private void Create ()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        float z = mCam.farClipPlane / 2;
        Vector3 vector3 = mCam.ScreenToWorldPoint (new Vector3 (x, y, z));

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
