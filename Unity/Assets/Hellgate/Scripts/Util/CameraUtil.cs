//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
    public class CameraUtil
    {
        /// <summary>
        /// Gets the 3D world position.
        /// </summary>
        /// <returns>The 3D world position.</returns>
        /// <param name="ui">NGUI.</param>
        /// <param name="cam">Cam.</param>
        public static Vector3 GetViewportToWorldPoint (GameObject ui, Camera cam, Camera guiCam = null)
        {
            if (guiCam == null) {
                guiCam = SceneManager.Instance.NGUICamera;
                if (guiCam == null) {
                    return Vector3.zero;
                }
            }

            Vector3 position = cam.ViewportToWorldPoint (guiCam.WorldToViewportPoint (ui.transform.position));
            position.z = 0;

            return position;
        }

        /// <summary>
        /// Gets the click NGUI.
        /// </summary>
        /// <returns><c>true</c>, if click NGUI was gotten, <c>false</c> otherwise.</returns>
        public static bool GetClickNGUI ()
        {
            object obj = Reflection.GetStaticMethodInvoke (
                             "UICamera",
                             "Raycast",
                             new System.Type[] { typeof(Vector3) },
                             new object[] { Input.mousePosition });
            
            return (bool)obj;
        }
    }
}
