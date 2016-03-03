//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
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
        public static void CreateJsonFile (string name, string text, string path)
        {
            if (name == "" || text == "" || text == "[]") {
                return;
            }

            CreateDirectory (path);

            path = path + "/" + name + ".json";
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

            AssetDatabase.Refresh ();
        }

        public static void CreateDirectory (string path)
        {
            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }
        }
    }
}
