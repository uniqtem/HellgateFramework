//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgateMenuEx : MenuController
{
    public void OnClickTap (string name)
    {
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
