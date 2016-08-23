//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

namespace HellgeteEx
{
    public class HellgateMenuEx : MenuController
    {
        public void OnClickTap (string name)
        {
            if (SoundManager.Instance.Sounds != null) {
                SoundManager.Instance.PlaySound (SoundManager.Instance.Sounds [Random.Range (0, SoundManager.Instance.Sounds.Count)].name);
            }

            switch (name) {
            case "Main":
                HellgateMainEx.GoMain ();
            break;
            case "AssetBundle":
                HellgateAssetEx.GoAsset ();
            break;
            case "Http":
                HellgateHttpEx.GoHttp ();
            break;
            case "ObjectPool":
                HellgateObjectPoolEx.GoObjectPool ();
            break;
            case "Scene":
                HellgateSceneEx.GoScene ();
            break;
            case "DB":
                HellgateDatabaseEx.GoDatabase ();
            break;
            }
        }
    }
}