//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace HellgateEditor
{
    [Hellgate.Table ("sqlite_master")]
    public class SqliteMastser
    {
        private string type = "";
        private string name = "";
        private string tblName = "";
        private int rootpage = 0;
        private string sql = "";

        public string Type {
            get {
                return type;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public string TblName {
            get {
                return tblName;
            }
        }

        public int Rootpage {
            get {
                return rootpage;
            }
        }

        public string Sql {
            get {
                return sql;
            }
        }
    }

    public class SqliteImportEditor : EditorWindow
    {
        private const string HELLGATE_DB_PATH = "HellgateDBPath";
        private const string HELLGATE_DB_OUTPUT_PATH = "HellgateDBOutputPath";
        private const string HELLGATE_DB_IGNORE_TABLE = "HellgateDBIgnoreTable";
        public static string sqlitePath;
        public static string outputJsonPath;
        public static string ignoreTableName;
        public static Hellgate.Query query;

        public void OnEnable ()
        {
            sqlitePath = PlayerPrefs.GetString (HELLGATE_DB_PATH);
            ignoreTableName = PlayerPrefs.GetString (HELLGATE_DB_IGNORE_TABLE);

            string basePath = Application.dataPath + "Sqlite";
            basePath = basePath.Replace ("Assets", "");
            outputJsonPath = PlayerPrefs.GetString (HELLGATE_DB_OUTPUT_PATH, basePath);
        }

        public void OnGUI ()
        {
            GUILayout.Label ("[Sqlite DB -> Json] Converter", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal ();
            sqlitePath = EditorGUILayout.TextField ("Sqlite DB file path :", sqlitePath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                sqlitePath = EditorUtility.OpenFilePanel ("Where is DB file?", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            outputJsonPath = EditorGUILayout.TextField ("Output folder :", outputJsonPath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                outputJsonPath = EditorUtility.OpenFolderPanel ("Select your json save folder", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            GUILayout.Label ("Set Ignore DB table name", EditorStyles.boldLabel);
            GUILayout.Label ("Ex) Info,Version,Description  or  Info|Version|Description");
            ignoreTableName = EditorGUILayout.TextField ("Ignore list :", ignoreTableName);

            if (GUILayout.Button ("Create", GUILayout.Height (40))) {
                if (sqlitePath != "" && outputJsonPath != "") {
                    Create ();
                }
            }
        }

        [MenuItem ("Window/Hellgate/Json Converter for Sqlite DB", false, 11)]
        public static void ShowWindow ()
        {
            EditorWindow.GetWindow (typeof(SqliteImportEditor));
        }

        /// <summary>
        /// Create this instance.
        /// </summary>
        public static void Create ()
        {
            // save
            PlayerPrefs.SetString (HELLGATE_DB_PATH, sqlitePath);
            PlayerPrefs.SetString (HELLGATE_DB_OUTPUT_PATH, outputJsonPath);
            PlayerPrefs.SetString (HELLGATE_DB_IGNORE_TABLE, ignoreTableName);

            string[] ignores = ignoreTableName.Split (new string[] { ",", "|" }, System.StringSplitOptions.None);

            query = new Hellgate.Query (sqlitePath);

            SqliteMastser[] master = query.SELECT<SqliteMastser> ("type", (object)"table");
            if (master == null) {
                Debug.Log ("There are no tables");
            } else {
                for (int i = 0; i < master.Length; i++) {
                    if (master [i].Name == "sqlite_sequence") {
                        continue;
                    }

                    if (Array.IndexOf (ignores, master [i].Name) >= 0) {
                        continue;
                    }

                    CreateUsalJson (master [i].Name);
                }
            }

        }

        /// <summary>
        /// Creates the usal json.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        public static void CreateUsalJson (string tableName)
        {
            Hellgate.DataTable data = query.SELECT (tableName);
            if (data.Rows.Count <= 0) {
                return;
            }

            string json = Json.Serialize (data.Rows);
            EditorUtil.CreateJsonFile (tableName, json, outputJsonPath);
        }
    }
}

