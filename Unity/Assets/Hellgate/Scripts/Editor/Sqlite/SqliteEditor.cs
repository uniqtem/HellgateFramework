using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using Hellgate;

namespace HellgateEditor
{
    public class SqliteEditor : EditorWindow
    {
        private const string HELLGATE_DB_NAME = "HellgateDBName";
        public static string dbName = "";

        public void OnEnable ()
        {
            dbName = PlayerPrefs.GetString (HELLGATE_DB_NAME);
        }

        public void OnGUI ()
        {
            GUILayout.Label ("Create Sqlite DB", EditorStyles.boldLabel);

            dbName = EditorGUILayout.TextField ("Sqlite DB name :", dbName);

            if (GUILayout.Button ("Create", GUILayout.Height (40))) {
                if (dbName != "") {
                    Create ();
                }
            }
        }

        [MenuItem ("Window/Hellgate/Create sqlite db", false, 12)]
        public static void ShowWindow ()
        {
            EditorWindow.GetWindow (typeof(SqliteEditor));
        }

        public static void Create ()
        {
            Sqlite sqlite = new Sqlite ();
            sqlite.AutoDDL (dbName, true);
            Debug.Log ("Create DB : " + dbName);
        }
    }
}
