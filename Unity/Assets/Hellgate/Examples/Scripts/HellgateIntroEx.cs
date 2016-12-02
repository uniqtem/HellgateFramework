//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

namespace HellgeteEx
{
    public class HellgateIntroEx : HellgateSceneControllerEx
    {
        public override void OnSet (object data)
        {
            base.OnSet (data);

            SceneManager.Instance.Wait (2f, Callback);
        }

        private void Callback ()
        {
            SceneManager.Instance.Screen ("HellgateLogin");
        }
    }
}
