//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace HellgateEditor
{
    public class ExcelSqliteEditor : EditorWindow
    {
        private const string HELLGATE_EXCEL_SQLITE_PATH = "HellgateExcelSqlitePath";
        private const string HELLGATE_EXCEL_SQLITE_DB_PATH = "HellgateExcelSqliteDBPath";
        private const string HELLGATE_EXCEL_SQLITE_DB_NAME = "HellgateExcelSqliteDBName";
        private const string HELLGATE_EXCEL_SQLITE_IGNORE_SHEET = "HellgateExcelSqliteIgnoreSheet";
        private const string HELLGATE_EXCEL_SQLITE_SPLIT_FLAG = "HellgateExcelSqliteSplitFlag";
        public static string excelFilePath = "";
        public static string dbPath = "";
        public static string dbName = "";
        public static string ignoreSheetName = "";
        public static bool splitFlag;

        public void OnEnable ()
        {
            string baseDBPath = Application.dataPath + "Sqlite";
            baseDBPath = baseDBPath.Replace ("Assets", "");
            dbPath = PlayerPrefs.GetString (HELLGATE_EXCEL_SQLITE_DB_PATH, baseDBPath);

            excelFilePath = PlayerPrefs.GetString (HELLGATE_EXCEL_SQLITE_PATH);
            dbName = PlayerPrefs.GetString (HELLGATE_EXCEL_SQLITE_DB_NAME);
            ignoreSheetName = PlayerPrefs.GetString (HELLGATE_EXCEL_SQLITE_IGNORE_SHEET);
            splitFlag = PlayerPrefs.GetInt (HELLGATE_EXCEL_SQLITE_SPLIT_FLAG, 0) > 0;
        }

        void OnGUI ()
        {
            GUILayout.Label ("[Excel -> Sqlite DB] Converter", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal ();
            excelFilePath = EditorGUILayout.TextField ("Excel file path :", excelFilePath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                excelFilePath = EditorUtility.OpenFilePanel ("Where is excel file?", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            dbPath = EditorGUILayout.TextField ("Output folder :", dbPath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                dbPath = EditorUtility.OpenFilePanel ("Select your Sqlite DB save folder", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            dbName = EditorGUILayout.TextField ("Sqlite DB name :", dbName);
            splitFlag = EditorGUILayout.Toggle ("SQL Split :", splitFlag);

            GUILayout.Label ("Set Ignore table name", EditorStyles.boldLabel);
            GUILayout.Label ("Ex) Info,Version,Description  or  Info|Version|Description");
            ignoreSheetName = EditorGUILayout.TextField ("Ignore list :", ignoreSheetName);

            if (GUILayout.Button ("Create", GUILayout.Height (40))) {
                if (excelFilePath != "" && dbPath != "" && dbName != "") {
                    if (excelFilePath.EndsWith ("xls") || excelFilePath.EndsWith ("xlsx")) {
                        Create ();
                    } else {
                        Debug.LogWarning ("Please set .xls and .xlsx file.");
                    }
                }
            }
        }

        [MenuItem ("Window/Hellgate/Sqlite DB Converter for Excel", false, 12)]
        public static void ShowWindow ()
        {
            EditorWindow.GetWindow (typeof(ExcelSqliteEditor));
        }

        public static void Create ()
        {
            string[] ignores = ignoreSheetName.Split (new string[] { ",", "|" }, System.StringSplitOptions.None);

            ExcelSqliteMaker maker = new ExcelSqliteMaker (excelFilePath, dbPath, dbName);
            maker.Create (splitFlag, ignores);

            PlayerPrefs.SetString (HELLGATE_EXCEL_SQLITE_PATH, excelFilePath);
            PlayerPrefs.SetString (HELLGATE_EXCEL_SQLITE_DB_PATH, dbPath);
            PlayerPrefs.SetString (HELLGATE_EXCEL_SQLITE_DB_NAME, dbName);
            PlayerPrefs.SetString (HELLGATE_EXCEL_SQLITE_IGNORE_SHEET, ignoreSheetName);
            PlayerPrefs.SetInt (HELLGATE_EXCEL_SQLITE_SPLIT_FLAG, splitFlag ? 1 : 0);
        }
    }
}
