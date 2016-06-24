//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class SceneController : SSceneController
    {
        /// <summary>
        /// Quit the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        public virtual void Quit (string message)
        {
            SceneManager.Instance.PopUp (message, PopUpType.YesAndNo, delegate(PopUpYNType type) {
                if (type == PopUpYNType.Yes) {
                    Application.Quit ();
                }
            });
        }

        /// <summary>
        /// Raises the click close event.
        /// </summary>
        public void OnClickClose ()
        {
            SceneManager.Instance.Close ();
        }

        /// <summary>
        /// Wait the specified duration and callback.
        /// </summary>
        /// <param name="duration">Duration.</param>
        /// <param name="callback">Callback.</param>
        public void Wait (float duration, SceneManager.FinishedDelegate callback)
        {
            this.StartCoroutine (SceneManager.Instance.CWait (duration, callback));
        }
    }
}
