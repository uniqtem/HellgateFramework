//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

namespace HellgeteEx
{
    public class HellgateMainEx : HellgateSceneControllerEx
    {
        [SerializeField]
        private GameObject title;
        [SerializeField]
        private GameObject panel_1;
        [SerializeField]
        private GameObject panel_100;

        public static void GoMain ()
        {
            string[] sprites = new string[] {
                "DarkGrey_Box128x128Measured", "DarkGrey_TrimmedBox128x128", "DarkGrey_Angle128x128",
                "LightGrey_Box128x128Measured", "LightGrey_TrimmedBox128x128", "LightGrey_Angle128x128",
                "Orange_Box128x128Measured", "Orange_TrimmedBox128x128", "Orange_Angle128x128"
            };

            LoadingJobData jobData = new LoadingJobData ("HellgateMain");

            List<AssetBundleData> assetBundles = new List<AssetBundleData> ();

            for (int i = 0; i < sprites.Length; i++) {
                AssetBundleData aBD = new AssetBundleData ("hellgatemain");
                aBD.type = typeof(Sprite);
                aBD.objName = sprites [i];
                assetBundles.Add (aBD);
            }

            jobData.assetBundles = assetBundles;
            jobData.PutExtra ("title", "Main");

            SceneManager.Instance.LoadingJob (jobData);
        }

        public override void Awake ()
        {
            base.Awake ();

            this.isCache = true;
        }

        public override void OnSet (object data)
        {
            base.OnSet (data);

            SoundManager.Instance.PlayBGM ();

            List<object> objs = data as List<object>;
            SetUI2DSpriteValue (panel_1, objs);
            SetUIButton (panel_100, objs);

            Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);
            SetLabelTextValue (title, intent ["title"].ToString ());
        }

        public override void OnKeyBack ()
        {
            base.Quit ("Exit ?");
        }

        public void OnClickQuest ()
        {
            HellgateQuestEx.GoQuest ();
        }

        public void OnClickNotification ()
        {
            HellgateNotificationEx.GoNotification ();
        }

        public void OnClickWebView ()
        {
            SceneManager.Instance.PopUp ("HellgateWebView", "https://www.google.com");
        }

        public void OnClickSound ()
        {
            string[] sounds = new string[] {
                "bgm2", "sound1", "sound2"
            };

            List<AssetBundleData> assetBundles = new List<AssetBundleData> ();

            for (int i = 0; i < sounds.Length; i++) {
                AssetBundleData aBD = new AssetBundleData ("hellgatesound");
                aBD.type = typeof(AudioClip);
                aBD.objName = sounds [i];
                assetBundles.Add (aBD);
            }

            LoadingJobData jobData = new LoadingJobData ();
            jobData.assetBundles = assetBundles;

            jobData.finishedDelegate = delegate(List<object> obj, LoadingJobController job) {
                List<AudioClip> clips = obj.GetListObjects<AudioClip> ();
                for (int i = 0; i < clips.Count; i++) {
                    if (clips [i].name == "bgm2") {
                        SoundManager.Instance.AddBGM (clips [i]);
                    } else {
                        SoundManager.Instance.AddSound (clips [i]);
                    }
                }

                SoundManager.Instance.PlayBGM ("bgm2");
                SceneManager.Instance.Close (delegate() {
                    SceneManager.Instance.PopUp ("The bgm and menu sound has been set.", PopUpType.Ok);
                });
            };

            SceneManager.Instance.LoadingJob (jobData);
        }
    }
}
