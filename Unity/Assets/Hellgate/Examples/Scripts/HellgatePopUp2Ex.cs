//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgatePopUp2Ex : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject label;

    public override void Awake ()
    {
        base.Awake ();
        isCache = true;
    }

    public override void OnSet (object data)
    {
        base.OnSet (data);

        SetLabelTextValue (label, data.ToString ());
    }
}
