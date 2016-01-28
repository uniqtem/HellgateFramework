//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

public class HellgateScreen1Ex : SceneController
{
    [SerializeField]
    private UILabel title;

    public override void OnSet (object data)
    {
        base.OnSet (data);

        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);

        title.text = intent ["title"].ToString ();
    }

    public void OnClickBack ()
    {
        HellgateSceneEx.GoScene ();
    }
}
