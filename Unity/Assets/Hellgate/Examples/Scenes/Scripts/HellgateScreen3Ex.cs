//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgateScreen3Ex : SceneController
{
    public override void OnSet (object data)
    {
        base.OnSet (data);

        MenuController.Instance.SetActiveBottom (false);
    }

    public void OnClickBack ()
    {
        HellgateSceneEx.GoScene ();
    }
}
