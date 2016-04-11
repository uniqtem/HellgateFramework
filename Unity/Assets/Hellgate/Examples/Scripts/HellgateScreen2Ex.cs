//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

public class HellgateScreen2Ex : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject title;

    public override void OnSet (object data)
    {
        base.OnSet (data);

        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);

        SetLabelTextValue (title, intent ["title"].ToString ());

        MenuController.Instance.SetActiveTop (false);
    }

    public void OnClickBack ()
    {
        HellgateSceneEx.GoScene ();
    }
}
