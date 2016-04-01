//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System;

namespace HellgateEditor
{
    public class ExcelImportEditor : EditorWindow
    {
        private const string HELLGATE_EXCEL_PATH = "HellgateExcelPath";
        private const string HELLGATE_EXCEL_OUTPUT_PATH = "HellgateExcelOutputPath";
        private const string HELLGATE_EXCEL_IGNORE_SHEET = "HellgateExcelIgnoreSheet";
        private const string HELLGATE_EXCEl_TYPE_SELECTED = "HellgateExcelTypeSelected";
        public static string excelFilePath = "";
        public static string outputJsonPath = "";
        public static string ignoreSheetName = "";
        public static int selected = 0;

        public void OnEnable ()
        {
            excelFilePath = PlayerPrefs.GetString (HELLGATE_EXCEL_PATH);
            ignoreSheetName = PlayerPrefs.GetString (HELLGATE_EXCEL_IGNORE_SHEET);

            string basePath = Application.dataPath + "Excel";
            basePath = basePath.Replace ("Assets", "");
            outputJsonPath = PlayerPrefs.GetString (HELLGATE_EXCEL_OUTPUT_PATH, basePath);

            selected = PlayerPrefs.GetInt (HELLGATE_EXCEl_TYPE_SELECTED, 0);
        }

        public void OnGUI ()
        {
            GUILayout.Label ("[Excel -> Json] Converter", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal ();
            excelFilePath = EditorGUILayout.TextField ("Excel file path :", excelFilePath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                excelFilePath = EditorUtility.OpenFilePanel ("Where is excel file?", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            outputJsonPath = EditorGUILayout.TextField ("Output folder :", outputJsonPath);
            if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (22))) {
                outputJsonPath = EditorUtility.OpenFolderPanel ("Select your json save folder", "", "");
            }
            EditorGUILayout.EndHorizontal ();

            selected = EditorGUILayout.Popup ("Option : ", selected, Enum.GetNames (typeof(ExcelImportType)));

            GUILayout.Label ("Set Ignore table name", EditorStyles.boldLabel);
            GUILayout.Label ("Ex) Info,Version,Description  or  Info|Version|Description");
            ignoreSheetName = EditorGUILayout.TextField ("Ignore list :", ignoreSheetName);

            if (GUILayout.Button ("Create", GUILayout.Height (40))) {
                if (excelFilePath != "" && outputJsonPath != "") {
                    if (excelFilePath.EndsWith ("xls")) {
                        Create ("xls");
                    } else if (excelFilePath.EndsWith ("xlsx")) {
                        Create ("xlsx");
                    } else {
                        Debug.LogWarning ("Please set .xls and .xlsx file.");
                    }
                }
            }
        }

        [MenuItem ("Window/Hellgate/Json Converter for Excel", false, 11)]
        public static void ShowWindow ()
        {
            EditorWindow.GetWindow (typeof(ExcelImportEditor));
        }

        /// <summary>
        /// Create the specified extension.
        /// </summary>
        /// <param name="extension">Extension.</param>
        public static void Create (string extension)
        {
            ExcelImportMaker maker = new ExcelImportMaker (excelFilePath, outputJsonPath);

            string[] ignores = ignoreSheetName.Split (new string[] { ",", "|" }, System.StringSplitOptions.None);
            if (selected == 0) {
                maker.Create (ExcelImportType.NORMAL, ignores);
            } else {
                maker.Create (ExcelImportType.ATTRIBUTE, ignores);
            }

            PlayerPrefs.SetString (HELLGATE_EXCEL_PATH, excelFilePath);
            PlayerPrefs.SetString (HELLGATE_EXCEL_IGNORE_SHEET, ignoreSheetName);
            PlayerPrefs.SetString (HELLGATE_EXCEL_OUTPUT_PATH, outputJsonPath);
            PlayerPrefs.SetInt (HELLGATE_EXCEl_TYPE_SELECTED, selected);
        }
    }
}
