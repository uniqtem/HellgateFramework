//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Hellgate;

public class HellgateQuestEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject cLabel;
    private List<GameObject> missiles;
    private GameObject user;
    private GameObject rUser;
    private float time;
    private int count;
    private bool isStart;
    private bool isPopUp;

    public static void GoQuest ()
    {
        LoadingJobData jobData = new LoadingJobData ();

        List<AssetBundleData> assetBundles = new List<AssetBundleData> ();
        assetBundles.Add (new AssetBundleData ("hellgatemaster", "quest", typeof(TextAsset)));

        jobData.finishedDelegate = delegate(List<object> obj, LoadingJobController job) {
            TextAsset text = Util.GetListObject<TextAsset> (obj);

            HellgateQuestDataEx data = Reflection.Convert<HellgateQuestDataEx> ((IDictionary)Json.Deserialize (text.text));

            assetBundles = new List<AssetBundleData> ();
            assetBundles.Add (new AssetBundleData ("hellgatequest", data._User.Prefab));
            for (int i = 0; i < data._Missile.Count; i++) {
                assetBundles.Add (new AssetBundleData ("hellgatequest", data._Missile [i].Prefab));
            }

            job.PutExtra ("data", data);
            job.nextSceneName = "HellgateQuest";
            job.LoadAssetBundle (assetBundles);
        };

        jobData.assetBundles = assetBundles;
        jobData.status = MainMenuStatus.Hide;
        jobData.assetBundleAllUnload = true;
        SceneManager.Instance.LoadingJob (jobData, false);
    }

    public override void OnSet (object data)
    {
        base.OnSet (data);
        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);
        List<GameObject> prefabs = Util.GetListObjects<GameObject> (objs);

        HellgateQuestDataEx qData = intent ["data"] as HellgateQuestDataEx;

        rUser = Instantiate (Util.FindGameObject (prefabs, qData._User.Prefab));
        rUser.transform.parent = Camera.main.transform;
        rUser.transform.localPosition = new Vector3 (0, 0, 0);
        rUser.GetComponent<HellgateUserEx> ().speed = qData._User.Speed;

        missiles = new List<GameObject> ();
        for (int i = 0; i < qData._Missile.Count; i++) {
            GameObject temp = Util.FindGameObject (prefabs, qData._Missile [i].Prefab);
            temp.GetComponent<HellgateMissileEx> ().speed = qData._Missile [i].Speed;

            missiles.Add (temp);
        }

        time = 0;
        count = 0;
        isStart = true;
        isPopUp = false;
    }

    void Update ()
    {
        time += Time.deltaTime;
        if (isStart) {
            if (time > 2f) {
                isStart = false;
            }
        }

        if (time >= 0.5f) {
            time = 0;

            int index = Random.Range (0, missiles.Count);
            GameObject missile = ObjectPoolManager.Spawn (missiles [index]);

            Vector3 pos = new Vector3 (Random.Range (0, Screen.width), Screen.height + 100, 0);
            pos = Camera.main.ScreenToWorldPoint (pos);
            missile.transform.position = pos;

            Vector3 toTarget = rUser.transform.position - missile.transform.position;
            missile.GetComponent<HellgateMissileEx> ().target = toTarget;

            ObjectPoolManager.DelayDespawn (missile, 10f);

            count++;
            SetLabelTextValue (cLabel, count.ToString ());
        }
    }

    public override void OnKeyBack ()
    {
        OnClickBack ();
    }

    public void OnClickBack ()
    {
        isPopUp = true;
        SceneManager.Instance.PopUp ("Go main!!", PopUpType.Ok, delegate(PopUpYNType type) {
            HellgateMainEx.GoMain ();
        });
    }

    public void Die ()
    {
        if (isPopUp) {
            return;
        }

        SceneManager.Instance.PopUp ("Die!!", PopUpType.Ok, delegate(PopUpYNType type) {
            HellgateMainEx.GoMain ();
        });
    }
}
