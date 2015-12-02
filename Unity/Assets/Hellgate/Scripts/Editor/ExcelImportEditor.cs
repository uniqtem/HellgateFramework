//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace HellgateEditor
{
	public class ExcelImportEditor : EditorWindow
	{
		private const string HELLGATE_EXCEL_PATH = "HellgateExcelPath";
		private const string HELLGATE_EXCEL_OUTPUT_PATH = "HellgateExcelOutputPath";
		private const string HELLGATE_EXCEL_IGNORE_SHEET = "HellgateExcelIgnoreSheet";
		public static string excelFilePath = "";
		public static string outputJsonPath = "";
		public static string ignoreSheetName = "";

		public void OnEnable ()
		{
			excelFilePath = PlayerPrefs.GetString (HELLGATE_EXCEL_PATH);
			ignoreSheetName = PlayerPrefs.GetString (HELLGATE_EXCEL_IGNORE_SHEET);

			string basePath = Application.dataPath + "Excel";
			basePath = basePath.Replace ("Assets", "");
			outputJsonPath = PlayerPrefs.GetString (HELLGATE_EXCEL_OUTPUT_PATH, basePath);
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

			GUILayout.Label ("Set Ignore table name", EditorStyles.boldLabel);
			GUILayout.Label ("Ex) Info,Version,Description  or  Info|Version|Description");
			ignoreSheetName = EditorGUILayout.TextField ("Ignore list :", ignoreSheetName);

			if (GUILayout.Button ("Create", GUILayout.Height (40))) {
				if (excelFilePath != "" && outputJsonPath != "") {
					string extension = excelFilePath.Substring (excelFilePath.Length - 4);
					if (extension != ".xls") {
						Debug.Log ("Please set .xls file.");
						return;
					}

					Create ();
				}
			}
		}

		[MenuItem ("Window/Hellgate/Json Converter for Excel", false, 11)]
		public static void ShowWindow ()
		{
			EditorWindow.GetWindow (typeof(ExcelImportEditor));
		}

		/// <summary>
		/// Create this instance.
		/// </summary>
		public static void Create ()
		{
			using (FileStream stream = File.Open (excelFilePath, FileMode.Open, FileAccess.Read)) {
				// save
				PlayerPrefs.SetString (HELLGATE_EXCEL_PATH, excelFilePath);
				PlayerPrefs.SetString (HELLGATE_EXCEL_IGNORE_SHEET, ignoreSheetName);
				PlayerPrefs.SetString (HELLGATE_EXCEL_OUTPUT_PATH, outputJsonPath);

				IWorkbook book = new HSSFWorkbook (stream);
				string[] ignores = ignoreSheetName.Split (new string[] {",", "|"}, System.StringSplitOptions.None);
				for (int i = 0; i < book.NumberOfSheets; ++i) {
					ISheet s = book.GetSheetAt (i);

					if (Array.IndexOf (ignores, s.SheetName) < 0) {
						CreateUsualJson (s);
					}
				}
			}
		}

		/// <summary>
		/// Adds the data.
		/// </summary>
		/// <param name="tCell">T cell.</param>
		/// <param name="vCell">V cell.</param>
		/// <param name="dic">Dic.</param>
		public static void AddData (ICell tCell, ICell vCell, Dictionary<string, object> dic)
		{
			if (vCell == null) {
				return;
			}
			
			if (vCell.CellType == CellType.STRING) {
				int n;
				if (int.TryParse (vCell.StringCellValue, out n)) {
					dic.Add (tCell.StringCellValue, n);
				} else {
					dic.Add (tCell.StringCellValue, vCell.StringCellValue);
				}
			} else if (vCell.CellType == CellType.NUMERIC) {
				dic.Add (tCell.StringCellValue, vCell.NumericCellValue);
			}
		}

		/// <summary>
		/// Creates the usual json.
		/// </summary>
		/// <param name="iS">I s.</param>
		public static void CreateUsualJson (ISheet iS)
		{
			IRow titleRow = iS.GetRow (0);
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
			
			for (int i = 1; i <= iS.LastRowNum; i++) {
				Dictionary<string, object> dic = new Dictionary<string, object> ();
				for (int j = 0; j <= titleRow.LastCellNum; j++) {
					ICell tCell = titleRow.GetCell (j);
					ICell vCell = iS.GetRow (i).GetCell (j);
					
					AddData (tCell, vCell, dic);
				}
				list.Add (dic);
			}
			
			EditorUtil.CreateJsonFile (iS.SheetName, Json.Serialize (list), outputJsonPath);
		}
	}
}
