//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Hellgate;

namespace HellgateEditor
{
    public class ExcelSqliteMaker
    {
        protected List<ISheet> listSheet;
        protected ExcelImportMaker maker;
        protected Query query;
        protected StringBuilder stringBuilder;
        protected string excelFilePath;
        protected string dbPath;
        protected string dbName;
        protected string path;
        protected int index;
        protected bool splitFlag;

        public ExcelSqliteMaker (string excelFilePath, string dbPath, string dbName)
        {
            this.excelFilePath = excelFilePath;
            this.dbPath = dbPath;
            this.dbName = dbName;
        }

        protected IEnumerator CreateTable ()
        {
            EditorUtility.DisplayProgressBar ("[Exce -> Sqlite DB] Converter",
                                              string.Format ("{0}({1}/{2})",
                                                             listSheet [index].SheetName,
                                                             index,
                                                             listSheet.Count),
                                              (float)index / (float)listSheet.Count);

            Action innerCreateTable = () => {
                if (index < listSheet.Count - 1) {
                    index++;
                    EditorUtil.StartCoroutine (CreateTable ());
                } else {
                    if (stringBuilder.ToString () != "") {
                        EditorUtil.CreateTextFile (Path.GetFileNameWithoutExtension (dbName), stringBuilder.ToString (), dbPath, false, ".sql");
                    }
                    Debug.Log ("saved name : " + path);

                    EditorUtility.ClearProgressBar ();
                    AssetDatabase.Refresh ();
                }
            };

            ISheet sheet = listSheet [index];
            if (sheet.LastRowNum <= 1) {
                innerCreateTable ();
                yield break;
            }

            IRow titleRow = sheet.GetRow (0);
            if (titleRow == null) {
                innerCreateTable ();
                yield break;
            }

            AttributeMappingConfig<ColumnAttribute>[] configs = new AttributeMappingConfig<ColumnAttribute>[titleRow.LastCellNum];
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
            for (int i = 1; i <= sheet.LastRowNum; i++) {
                Dictionary<string, object> dic = new Dictionary<string, object> ();
                for (int j = 0; j < titleRow.LastCellNum; j++) {
                    ICell titleCell = titleRow.GetCell (j);
                    ICell valueCell = sheet.GetRow (i).GetCell (j);

                    Type type = maker.AddData (titleCell, valueCell, dic);
                    if (type == null) {
                        continue;
                    }

                    if (configs [j] == null) {
                        configs [j] = new AttributeMappingConfig<ColumnAttribute> ();
                        configs [j].name = titleCell.StringCellValue;
                        configs [j].type = type;
                    }
                }

                if (dic.Count > 0) {
                    list.Add (dic);
                }
            }

            string dropQuery = query.GenerateDropTableSQL (sheet.SheetName);
            stringBuilder.Append (dropQuery);
            stringBuilder.AppendLine ();

            string tableQuery = query.GenerateCreateTableSQL (sheet.SheetName, configs);
            stringBuilder.Append (tableQuery);
            stringBuilder.AppendLine ();

            for (int i = 0; i < list.Count; i++) {
                string[] column = list [i].Keys.ToArray ();
                string[] value = Array.ConvertAll (list [i].Values.ToArray (), x => x.ToString ());
                string insertQuery = query.GenerateInsertSQL (sheet.SheetName, column, value);
                stringBuilder.Append (insertQuery);
                stringBuilder.AppendLine ();
            }

            query.ExecuteNonQuery (tableQuery);
            query.INSERT_BATCH (sheet.SheetName, list);

            yield return null;

            if (splitFlag) {
                EditorUtil.CreateTextFile (sheet.SheetName, stringBuilder.ToString (), dbPath, false, ".sql");
                stringBuilder = new StringBuilder ();
            }

            innerCreateTable ();
        }

        /// <summary>
        /// Create the specified splitFlag and ignores.
        /// </summary>
        /// <param name="splitFlag">If set to <c>true</c> split flag.</param>
        /// <param name="ignores">Ignores.</param>
        public void Create (bool splitFlag, string[] ignores = null)
        {
            this.splitFlag = splitFlag;

            maker = new ExcelImportMaker (excelFilePath);
            IWorkbook book = maker.FileStream ();

            if (book == null) {
                return;
            }

            listSheet = new List<ISheet> ();
            for (int i = 0; i < book.NumberOfSheets; i++) {
                ISheet sheet = book.GetSheetAt (i);
                if (Array.IndexOf (ignores, sheet.SheetName) < 0) {
                    listSheet.Add (sheet);
                }
            }

            if (listSheet.Count <= 0) {
                return;
            }

            path = Path.Combine (dbPath, dbName);
            query = new Query (path);
            query.Sqlite.CreateFile (true);

            stringBuilder = new StringBuilder ();

            index = 0;
            EditorUtil.StartCoroutine (CreateTable ());
        }
    }
}
