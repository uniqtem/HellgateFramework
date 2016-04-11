//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Hellgate;

public class HellgateSceneData
{
    private string[] sprite = null;

    public string[] _Sprite {
        get {
            return sprite;
        }
    }
}

public class HellgateSceneEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject title;
    private bool menu1Flag;
    private bool menu2Flag;
    private bool menu3Flag;

    public static void GoScene ()
    {
        LoadingJobData jobData = new LoadingJobData ();

        List<AssetBundleData> assetBundles = new List<AssetBundleData> ();
        assetBundles.Add (new AssetBundleData ("hellgatemaster", "scene", typeof(TextAsset)));

        jobData.assetBundles = assetBundles;
        jobData.finishedDelegate = delegate (List<object> obj, LoadingJobController job) {
            TextAsset text = Util.GetListObject<TextAsset> (obj);

            HellgateSceneData data = Reflection.Convert<HellgateSceneData> ((IDictionary)Json.Deserialize (text.text));

            assetBundles = new List<AssetBundleData> ();
            for (int i = 0; i < data._Sprite.Length; i++) {
                assetBundles.Add (new AssetBundleData ("hellgatescene", data._Sprite [i], typeof(Sprite)));
            }

            job.nextSceneName = "HellgateScene";
            job.LoadAssetBundle (assetBundles);
        };

        jobData.PutExtra ("title", "Scene");
        SceneManager.Instance.LoadingJob (jobData);
    }

    public override void OnSet (object data)
    {
        base.OnSet (data);

        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);

        SetUIButton (gameObject, objs);

        SetLabelTextValue (title, intent ["title"].ToString ());

        menu1Flag = true;
        menu2Flag = true;
        menu3Flag = true;
    }

    public override void OnKeyBack ()
    {
        base.Quit ("Exit ?");
    }

    public void OnClickScreen1 ()
    {
        LoadingJobData jobData = new LoadingJobData ("HellgateScreen1");
        jobData.status = MainMenuStatus.Hide;
        jobData.PutExtra ("title", "Screen1");

        SceneManager.Instance.LoadingJob (jobData, false);
    }

    public void OnClickScreen2 ()
    {
        LoadingJobData jobData = new LoadingJobData ("HellgateScreen2");
        jobData.PutExtra ("title", "Screen2");

        SceneManager.Instance.LoadingJob (jobData);
    }

    public void OnClickScreen3 ()
    {
        SceneManager.Instance.Screen ("HellgateScreen3");
    }

    public void OnClickPopUp1 ()
    {
        LoadingJobData jobData = new LoadingJobData ("HellgatePopUp1");
        jobData.PutExtra ("label", "PopUp Test\nAlpha = 0");
        jobData.nextScenePopUp = true;
        jobData.shieldAlpha = 0;

        SceneManager.Instance.LoadingJob (jobData);
    }

    public void OnClickPopUp2 ()
    {
        SceneManager.Instance.PopUp ("HellgatePopUp2", "PopUp Test");
    }

    public void OnClickPopUp3 ()
    {
        SceneManager.Instance.PopUp ("Yes and No.", PopUpType.YesAndNo, delegate(PopUpYNType type) {
            if (type == PopUpYNType.Yes) {
                SceneManager.Instance.PopUp ("Okay.", PopUpType.Ok);
            }
        });
    }

    public void OnClickMenu1 ()
    {
        menu1Flag = !menu1Flag;
        MenuController.Instance.SetActiveTop (menu1Flag);
    }

    public void OnClickMenu2 ()
    {
        menu2Flag = !menu2Flag;
        MenuController.Instance.SetActiveBottom (menu2Flag);
    }

    public void OnClickMenu3 ()
    {
        menu3Flag = !menu3Flag;
        if (menu3Flag) {
            SceneManager.Instance.DestroyScenesFrom ("HellgateNotice");
        } else {
            if (GameObject.Find ("HellgateNotice") != null) {
                SceneManager.Instance.DestroyScenesFrom ("HellgateNotice");
                menu3Flag = true;
            } else {
                SceneManager.Instance.LoadMenu ("HellgateNotice");
            }
        }
    }
}
