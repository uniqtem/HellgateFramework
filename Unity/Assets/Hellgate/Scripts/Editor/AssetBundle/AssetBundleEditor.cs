//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//               Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Framework editor.
/// </summary>
namespace HellgateEditor
{
    public class AssetBundleEditor : EditorWindow
    {
        private const string HELLGATE_ASSETBUNDLE_PATH = "HellgateAssetBundlePath";
        public static string assetBundlesOutputPath = "";

        public void OnEnable ()
        {
            string basePath = Application.dataPath + "AssetBundle";
            basePath = basePath.Replace ("Assets", "");
            assetBundlesOutputPath = PlayerPrefs.GetString (HELLGATE_ASSETBUNDLE_PATH, basePath);
        }

        public void OnGUI ()
        {
            string target = GetPlatformFolderForAssetBundles (EditorUserBuildSettings.activeBuildTarget);
            if (target == string.Empty) {
                GUILayout.Label ("Only supports PC / Android / iOS.", EditorStyles.boldLabel);
                return;
            }

            if (assetBundlesOutputPath == "") {
                GUILayout.Label ("Path :  Please set the path assetbundle.", EditorStyles.boldLabel);
            } else {
                GUILayout.Label ("Path : " + assetBundlesOutputPath, EditorStyles.boldLabel);
            }

            GUILayout.Label ("Target : " + target);

            EditorGUILayout.BeginHorizontal ();
            assetBundlesOutputPath = EditorGUILayout.TextField ("Output folder :", assetBundlesOutputPath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22)))
                assetBundlesOutputPath = EditorUtility.OpenFolderPanel ("Select your assetbundle save folder", "", "");
            EditorGUILayout.EndHorizontal ();

            if (GUILayout.Button ("Create", GUILayout.Height (40))) {
                if (assetBundlesOutputPath != "") {
                    Create ();
                }
            }
        }

        [MenuItem ("Window/Hellgate/Build AssetBundles", false, 10)]
        public static void ShowWindow ()
        {
            EditorWindow.GetWindow (typeof(AssetBundleEditor));
        }

        /// <summary>
        /// Create this assetbundle.
        /// </summary>
        public static void Create ()
        {
            // save
            PlayerPrefs.SetString (HELLGATE_ASSETBUNDLE_PATH, assetBundlesOutputPath);

            string[] names = AssetDatabase.GetAllAssetBundleNames ();
            string[] unNames = AssetDatabase.GetUnusedAssetBundleNames ();
            if (names.Length <= 0) {
                Debug.LogError ("There is no set assetbundle.");
                return;
            } else {
                string cN = "";
                string uCN = "";

                foreach (string name in names) {
                    if (Array.Find (unNames, element => element.StartsWith (name, StringComparison.Ordinal)) == "") {
                        cN += name + "||";
                    }
                }

                foreach (string name in unNames) {
                    uCN += name + "||";
                }

                if (cN != "") {
                    Debug.Log ("Created assetbundle : " + cN);
                }

                if (uCN != "") {
                    Debug.Log ("Uncreated assetbundle : " + uCN);
                }
            }

            string target = GetPlatformFolderForAssetBundles (EditorUserBuildSettings.activeBuildTarget);
            string outputPath = Path.Combine (assetBundlesOutputPath, target);
            if (!Directory.Exists (outputPath)) {
                Directory.CreateDirectory (outputPath);
            }
            BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        /// Gets the platform folder for asset bundles.
        /// </summary>
        /// <returns>The platform folder for asset bundles.</returns>
        /// <param name="target">Target.</param>
        public static string GetPlatformFolderForAssetBundles (BuildTarget target)
        {
            switch (target) {
            case BuildTarget.Android:
                return "android";
            case BuildTarget.iOS:
                return "ios";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                return "pc";
            default:
                return string.Empty;
            }
        }
    }
}