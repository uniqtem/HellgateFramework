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
    public class ExcelImportMaker
    {
        protected class ExcelData
        {
            public string sheetName;
            public List<Dictionary<string, object>> data;

            public String[] Columns {
                get {
                    return data [0].Keys.ToArray ();
                }
            }

            public ExcelData (string sheetName, List<Dictionary<string, object>> data)
            {
                this.sheetName = sheetName;
                this.data = data;
            }

            public List<Dictionary<string, object>> Join (Dictionary<string, object> joinData)
            {
                if (joinData == null || joinData.Count <= 0) {
                    return data;
                }

                return data.Where (x => joinData.All (y => x.ContainsKey (y.Key) && x [y.Key].Equals (y.Value))).ToList ();
            }
        }

        public delegate TResult TResultDelegate<out TResult> (string type);

        protected IWorkbook book;
        protected string excelFilePath;
        protected string outputJsonPath;
        protected TResultDelegate<Type> joinType;
        protected List<Type> listType;
        protected List<ExcelData> listExcel;
        protected int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="HellgateEditor.ExcelImportMaker"/> class.
        /// </summary>
        /// <param name="excelFilePath">Excel file path.</param>
        /// <param name="outputJsonPath">Output json path.</param>
        public ExcelImportMaker (string excelFilePath, string outputJsonPath = "")
        {
            this.excelFilePath = excelFilePath;
            this.outputJsonPath = outputJsonPath;
        }

        private StringBuilder Append (List<string> keys, Dictionary<string, object> dic)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            for (int i = 0; i < keys.Count; i++) {
                if (i == 0) {
                    stringBuilder.Append (dic [keys [i]]);
                } else {
                    stringBuilder.AppendFormat ("-{0}", dic [keys [i]]);
                }
            }

            return stringBuilder;
        }

        protected void DisplayProgressBar (string text, int index, int maxIndex)
        {
            EditorUtility.DisplayProgressBar ("[Excel -> Json] Converter",
                                              string.Format ("{0}({1}/{2})",
                                                             text,
                                                             index,
                                                             maxIndex),
                                              (float)index / (float)maxIndex);
        }

        protected void ClearProgressBar ()
        {
            EditorUtility.ClearProgressBar ();
            AssetDatabase.Refresh ();
        }

        protected IEnumerator CreateExcelData (Action finish)
        {
            ISheet sheet = book.GetSheetAt (index);
            DisplayProgressBar ("Load sheet " + sheet.SheetName, index, book.NumberOfSheets);

            IRow titleRow = sheet.GetRow (0);
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
            for (int j = 1; j <= sheet.LastRowNum; j++) {
                Dictionary<string, object> dic = new Dictionary<string, object> ();
                for (int k = 0; k < titleRow.LastCellNum; k++) {
                    ICell titleCell = titleRow.GetCell (k);
                    ICell valueCell = sheet.GetRow (j).GetCell (k);

                    AddData (titleCell, valueCell, dic);
                }

                if (dic.Count > 0) {
                    list.Add (dic);
                }
            }

            if (list.Count > 0) {
                listExcel.Add (new ExcelData (sheet.SheetName, list));
            }

            yield return null;

            if (index < book.NumberOfSheets - 1) {
                index++;
                EditorUtil.StartCoroutine (CreateExcelData (finish));
            } else {
                finish ();
            }
        }

        protected IEnumerator CreateNormalJson ()
        {
            ExcelData data = listExcel [index];
            DisplayProgressBar ("Create json " + data.sheetName, index, listExcel.Count);

            EditorUtil.CreateJsonFile (data.sheetName, JsonUtil.ToJson (data.data), outputJsonPath, false);

            yield return null;

            if (index < listExcel.Count - 1) {
                index++;
                EditorUtil.StartCoroutine (CreateNormalJson ());
            } else {
                ClearProgressBar ();
            }
        }

        protected IEnumerator CreateAttributeJson ()
        {
            CreateAttributeJson (index);
            DisplayProgressBar ("Create json " + listType [index], index, listType.Count);

            yield return null;

            if (index < listType.Count - 1) {
                index++;
                EditorUtil.StartCoroutine (CreateAttributeJson ());
            } else {
                ClearProgressBar ();
            }
        }

        protected void CreateAttributeJson (int index)
        {
            Type type = listType [index];
            ExcelAttribute excel = type.GetAttributeValue<ExcelAttribute> ();
            List<Dictionary<string, object>> list = CreateAttributeJson (type);
            if (list == null || list.Count <= 0) {
                return;
            }

            if (excel.IndexFlag) {
                AttributeMappingConfig<ColumnAttribute>[] configs = Reflection.FieldAMCRetrieve<ColumnAttribute> (type);
                List<string> pks = new List<string> ();
                List<string> fks = new List<string> ();
                foreach (AttributeMappingConfig<ColumnAttribute> config in configs) {
                    if (config.t != null) {
                        ColumnAttribute column = config.t as ColumnAttribute;
                        if (column != null) {
                            if (column.CheckConstraints (DataConstraints.PK)) {
                                pks.Add (config.name);
                            }

                            if (column.CheckConstraints (DataConstraints.FK)) {
                                fks.Add (config.name);
                            }
                        }
                    }
                }

                if (pks.Count > 0) {
                    foreach (Dictionary<string, object> dic in list) {
                        StringBuilder stringBuilder = Append (pks, dic);
                        string createFileName = string.Format ("{0}{1}", excel.CreateFileName, stringBuilder.ToString ());
                        EditorUtil.CreateJsonFile (createFileName, JsonUtil.ToJson (dic), outputJsonPath, false);
                    }
                }

                if (fks.Count > 0) {
                    List<string> fk = new List<string> ();
                    foreach (Dictionary<string, object> dic in list) {
                        StringBuilder stringBuilder = Append (fks, dic);
                        fk.Add (stringBuilder.ToString ());
                    }

                    fk = Util.GetDistinctValues<string> (fk);
                    foreach (string s in fk) {
                        List<Dictionary<string, object>> fkList = new List<Dictionary<string, object>> ();
                        foreach (Dictionary<string, object> dic in list) {
                            StringBuilder stringBuilder = Append (fks, dic);
                            if (s == stringBuilder.ToString ()) {
                                fkList.Add (dic);
                            }
                        }

                        string createFileName = string.Format ("{0}{1}", excel.CreateFileName, s);
                        EditorUtil.CreateJsonFile (createFileName, JsonUtil.ToJson (fkList), outputJsonPath, false);
                    }
                }
            } else {
                EditorUtil.CreateJsonFile (excel.CreateFileName, JsonUtil.ToJson (list), outputJsonPath, false);
            }
        }

        protected Dictionary<string, object> CreateAttributeJson (Dictionary<string, object> data,
                                                                  AttributeMappingConfig<ColumnAttribute>[] configs)
        {
            Dictionary<string, object> dic = new Dictionary<string, object> ();
            foreach (AttributeMappingConfig<ColumnAttribute> config in configs) {
                if (Util.IsValueType (config.type)) {
                    if (data.ContainsKey (config.name)) {
                        dic.Add (config.name, data [config.name]);
                    }
                } else {
                    if (!config.type.IsArray) {
                        continue;
                    }

                    Type type = config.type.GetElementType ();
                    string value = "";
                    if (config.t != null) {
                        ColumnAttribute column = config.t as ColumnAttribute;
                        if (column != null) {
                            if (column.Value == "" && column.Type != "") {
                                value = config.name;
                            } else if (joinType != null) {
                                Type tempType = joinType (data [column.Type].ToString ());
                                type = tempType == null ? type : tempType;
                                value = column.Value;
                            }
                        }
                    }

                    Dictionary<string, object> join = new Dictionary<string, object> ();
                    AttributeMappingConfig<ColumnAttribute>[] tempConfigs = Reflection.FieldAMCRetrieve<ColumnAttribute> (config.type.GetElementType ());
                    foreach (AttributeMappingConfig<ColumnAttribute> tempConfig in tempConfigs) {
                        if (tempConfig.t != null) {
                            ColumnAttribute column = tempConfig.t as ColumnAttribute;
                            if (column != null) {
                                if (column.CheckConstraints (DataConstraints.PK)) {
                                    join.Add (tempConfig.name, data [value]);
                                    break;
                                }

                                if (data.ContainsKey (column.Value)) {
                                    join.Add (tempConfig.name, data [column.Value]);
                                }
                            }
                        }
                    }

                    if (join.Count > 0) {
                        dic.Add (config.name, CreateAttributeJson (type, join));
                    }
                }
            }

            return dic;
        }

        protected List<Dictionary<string, object>> CreateAttributeJson (Type type, Dictionary<string, object> join = null)
        {
            ExcelAttribute excel = type.GetAttributeValue<ExcelAttribute> ();
            AttributeMappingConfig<ColumnAttribute>[] configs = Reflection.FieldAMCRetrieve<ColumnAttribute> (type);

            if (configs.Length > 0) {
                if (excel == null) {
                    return null;
                }

                ExcelData excelData = listExcel.Find (x => x.sheetName == excel.SheetName);
                if (excelData == null) {
                    Debug.LogWarning (type.Name + " sheet name is incorrect.");
                    return null;
                }

                List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
                List<Dictionary<string, object>> data = excelData.Join (join);

                for (int i = 0; i < data.Count; i++) {
                    Dictionary<string, object> dic = CreateAttributeJson (data [i], configs);

                    if (dic.Count > 0) {
                        list.Add (dic);
                    }
                }

                return list;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Adds the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="title">Title.</param>
        /// <param name="value">Value.</param>
        /// <param name="dic">Dic.</param>
        public Type AddData (ICell title, ICell value, Dictionary<string, object> dic)
        {
            Type type = null;
            if (title == null || value == null) {
                return type;
            }

            if (dic == null) {
                dic = new Dictionary<string, object> ();
            }

            string temp = "";
            Action innerAddData = () => {
                int i;
                long l;
                float f;
                double d;
                if (int.TryParse (temp, out i)) {
                    type = typeof(int);
                    dic.Add (title.StringCellValue, i);
                } else if (long.TryParse (temp, out l)) {
                    type = typeof(long);
                    dic.Add (title.StringCellValue, l);
                } else if (float.TryParse (temp, out f)) {
                    type = typeof(float);
                    dic.Add (title.StringCellValue, f);
                } else if (double.TryParse (temp, out d)) {
                    type = typeof(double);
                    dic.Add (title.StringCellValue, d);
                } else {
                    type = typeof(string);
                    dic.Add (title.StringCellValue, temp);
                }
            };

            if (value.CellType == CellType.String) {
                temp = value.StringCellValue;
                innerAddData ();
            } else if (value.CellType == CellType.Numeric) {
                temp = value.NumericCellValue.ToString ();
                innerAddData ();
            } else if (value.CellType == CellType.Boolean) {
                type = typeof(bool);
                dic.Add (title.StringCellValue, value.BooleanCellValue);
            }

            return type;
        }

        /// <summary>
        /// Files the stream.
        /// </summary>
        /// <returns>The stream.</returns>
        public IWorkbook FileStream ()
        {
            using (FileStream stream = new FileStream (excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                if (excelFilePath.EndsWith ("xls")) {
                    book = new HSSFWorkbook (stream);
                } else if (excelFilePath.EndsWith ("xlsx")) {
                    if (Application.platform == RuntimePlatform.OSXEditor) {
                        Debug.LogWarning ("xlsx is not supported on OSX.");
                        return null;
                    } else {
                        book = new XSSFWorkbook (stream);
                    }
                } else {
                    return null;
                }

                return book;
            }
        }

        /// <summary>
        /// Create the specified type, ignores and joinType.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="ignores">Ignores.</param>
        /// <param name="joinType">Join type.</param>
        public void Create (JsonImportType type, string[] ignores = null, TResultDelegate<Type> joinType = null)
        {
            book = FileStream ();
            if (book == null) {
                return;
            }

            if (book.NumberOfSheets <= 0) {
                Debug.LogWarning ("No sheet.");
                return;
            }

            index = 0;
            listExcel = new List<ExcelData> ();

            if (type == JsonImportType.NORMAL) {
                EditorUtil.StartCoroutine (CreateExcelData (delegate() {
                    if (listExcel.Count <= 0) {
                        Debug.LogWarning ("No data on the sheet.");
                        ClearProgressBar ();
                        return;
                    }

                    if (ignores != null) {
                        listExcel.RemoveAll (x => ignores.Contains (x.sheetName));
                    }

                    index = 0;
                    EditorUtil.StartCoroutine (CreateNormalJson ());
                }));
            } else {
                this.joinType = joinType;

                EditorUtil.StartCoroutine (CreateExcelData (delegate() {
                    if (listExcel.Count <= 0) {
                        Debug.LogWarning ("No data on the sheet.");
                        ClearProgressBar ();
                        return;
                    }

                    listType = new List<Type> ();
                    Type[] types = Reflection.GetExecutingAssembly ();
                    for (int i = 0; i < types.Length; i++) {
                        ExcelAttribute excel = types [i].GetAttributeValue<ExcelAttribute> ();
                        if (excel == null || excel.CreateFileName == "") {
                            continue;
                        }

                        listType.Add (types [i]);
                    }

                    if (listType.Count <= 0) {
                        Debug.LogWarning ("No class is set to Excel Attribute.");
                        ClearProgressBar ();
                        return;
                    }

                    index = 0;
                    EditorUtil.StartCoroutine (CreateAttributeJson ());
                }));
            }
        }
    }
}
