//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hellgate
{
    [ExecuteInEditMode]
    public class AutoSceneManager : MonoBehaviour
    {
#if UNITY_EDITOR
        void Awake ()
        {
            AutoAddEmptyScene ();
            AutoAddNGUISheildCamera ();
        }

        void OnValidate ()
        {
            AutoAddEmptyScene ();
            AutoAddNGUISheildCamera ();
        }

        private void AutoAddEmptyScene ()
        {
            if (!Application.isPlaying) {
                string emptyScene = Util.GetPathTemplateFile ("HellgateEmpty.unity");
                emptyScene = emptyScene.Replace (System.IO.Path.DirectorySeparatorChar, '/');

                var scenes = EditorBuildSettings.scenes;

                foreach (var scene in scenes) {
                    // Check if exist, return
                    if (scene.path.CompareTo (emptyScene) == 0) {
                        return;
                    }
                }

                // If not exist
                var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];

                for (int i = 0; i < scenes.Length; i++) {
                    newScenes [i] = scenes [i];
                }

                EditorBuildSettingsScene emptySettingScene = new EditorBuildSettingsScene (emptyScene, true);
                newScenes [newScenes.Length - 1] = emptySettingScene;

                // Save
                EditorBuildSettings.scenes = newScenes;
                AssetDatabase.SaveAssets ();
            }
        }

        private void AutoAddNGUISheildCamera ()
        {
            if (gameObject.GetComponent<SceneManager> ()._UIType == UIType.NGUI) {
                GameObject shiled = Resources.Load ("HellgateNGUIShield") as GameObject;
                System.Type type = FindType ("UICamera");
                if (type != null) {
                    Camera camera = shiled.GetComponentInChildren<Camera> ();
                    if (camera.GetComponent ("UICamera") == null) {
                        camera.gameObject.AddComponent (type);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the type.
        /// https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Utils/TypeUtil.cs
        /// </summary>
        /// <returns>The type.</returns>
        /// <param name="typeName">Type name.</param>
        /// <param name="useFullName">If set to <c>true</c> use full name.</param>
        /// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
        private System.Type FindType (string typeName, bool useFullName = false, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty (typeName))
                return null;

            StringComparison e = (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (useFullName) {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies()) {
                    foreach (var t in assemb.GetTypes()) {
                        if (string.Equals (t.FullName, typeName, e))
                            return t;
                    }
                }
            } else {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies()) {
                    foreach (var t in assemb.GetTypes()) {
                        if (string.Equals (t.FullName, typeName, e))
                            return t;
                    }
                }
            }
            return null;
        }
#endif
    }
}

