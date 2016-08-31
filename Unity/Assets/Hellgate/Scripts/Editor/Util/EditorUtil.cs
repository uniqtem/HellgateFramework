//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;

namespace HellgateEditor
{
    public class EditorUtil
    {
        /// <summary>
        /// Creates the json file.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="text">Text.</param>
        /// <param name="path">Path.</param>
        /// <param name="refresh">If set to <c>true</c> refresh.</param>
        public static void CreateJsonFile (string name, string text, string path, bool refresh = true, string extension = ".json")
        {
            CreateTextFile (name, text, path, refresh, extension);
        }

        /// <summary>
        /// Creates the text file.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="text">Text.</param>
        /// <param name="path">Path.</param>
        /// <param name="refresh">If set to <c>true</c> refresh.</param>
        /// <param name="extension">Extension.</param>
        public static void CreateTextFile (string name, string text, string path, bool refresh, string extension)
        {
            if (name == "" || text == "") {
                return;
            }

            CreateDirectory (path);

            path = string.Format ("{0}/{1}{2}", path, name, extension);
            Debug.Log ("saved name : " + path);
            if (!File.Exists (path)) {
                File.Create (path).Dispose ();
                using (TextWriter tw = new StreamWriter (path)) {
                    tw.WriteLine (text);
                    tw.Close ();
                }

            } else if (File.Exists (path)) {
                using (TextWriter tw = new StreamWriter (path)) {
                    tw.WriteLine (text);
                    tw.Close (); 
                }
            }

            if (refresh) {
                AssetDatabase.Refresh ();
            }
        }

        public static void CreateDirectory (string path)
        {
            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }
        }

        public static void StartCoroutine (IEnumerator coroutine)
        {
            EditorCoroutine editor = new EditorCoroutine (coroutine);
            editor.Start ();
        }
    }
}
