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
        public static Vector3 Get3DWorldPosition (GameObject ui, Camera cam)
        {
            Camera guiCam = NGUITools.FindCameraForLayer (ui.layer);

            Vector3 position = cam.ViewportToWorldPoint (guiCam.WorldToViewportPoint (ui.transform.position));
            position.z = 0;

            return position;
        }

        /// <summary>
        /// Gets the NGUI position.
        /// </summary>
        /// <returns>The NGUI position.</returns>
        /// <param name="gObj">Game4object.</param>
        /// <param name="ui">NGUI.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="cam">Cam.</param>
        public static Vector3 GetNGUIPosition (GameObject gObj, GameObject ui, Vector3 offset, Camera cam)
        {
            Camera guiCam = NGUITools.FindCameraForLayer (ui.layer);
            Vector3 p3D = cam.WorldToViewportPoint (gObj.transform.position + offset);
            Vector3 p2D = guiCam.ViewportToWorldPoint (p3D);
            p2D.z = 0;

            return p2D;
        }

        /// <summary>
        /// Gets the click NGUI.
        /// </summary>
        /// <returns><c>true</c>, if click NGUI was gotten, <c>false</c> otherwise.</returns>
        public static bool GetClickNGUI ()
        {
            return UICamera.Raycast (Input.mousePosition);
        }
    }
}
