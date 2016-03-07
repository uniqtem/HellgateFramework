using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MiniJSON;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Hellgate;

namespace HellgateEditor
{
    public enum ExcelImportType
    {
        NORMAL = 1,
        ATTRIBUTE
    }

    public class ExcelImportMaker
    {
        protected class CreateAttributeJsonConfig
        {
            public IRow row;
            public IRow titleRow;
            public AttributeMappingConfig<ColumnAttribute>[] configs;
            public string columnName;
            public object columnValue;

            public CreateAttributeJsonConfig ()
            {
                row = null;
                titleRow = null;
                configs = null;
                columnName = "";
                columnValue = null;
            }
        }

        protected IWorkbook book;
        protected string excelFilePath;
        protected string outputJsonPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="HellgateEditor.ExcelImportMaker"/> class.
        /// </summary>
        /// <param name="excelFilePath">Excel file path.</param>
        /// <param name="outputJsonPath">Output json path.</param>
        public ExcelImportMaker (string excelFilePath, string outputJsonPath)
        {
            this.excelFilePath = excelFilePath;
            this.outputJsonPath = outputJsonPath;
        }

        protected void AddData (ICell tCell, ICell vCell, Dictionary<string, object> dic)
        {
            if (vCell == null) {
                return;
            }

            if (vCell.CellType == CellType.String) {
                int n;
                if (int.TryParse (vCell.StringCellValue, out n)) {
                    dic.Add (tCell.StringCellValue, n);
                } else {
                    dic.Add (tCell.StringCellValue, vCell.StringCellValue);
                }
            } else if (vCell.CellType == CellType.Numeric) {
                dic.Add (tCell.StringCellValue, vCell.NumericCellValue);
            }
        }

        protected void CreateNormalJson (string[] ignores)
        {
            for (int i = 0; i < book.NumberOfSheets; ++i) {
                ISheet s = book.GetSheetAt (i);

                if (Array.IndexOf (ignores, s.SheetName) < 0) {
                    IRow titleRow = s.GetRow (0);
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();

                    for (int j = 1; j <= s.LastRowNum; j++) {
                        Dictionary<string, object> dic = new Dictionary<string, object> ();
                        for (int k = 0; k <= titleRow.LastCellNum; k++) {
                            ICell tCell = titleRow.GetCell (k);
                            ICell vCell = s.GetRow (j).GetCell (k);

                            AddData (tCell, vCell, dic);
                        }
                        list.Add (dic);
                    }

                    EditorUtil.CreateJsonFile (s.SheetName, Json.Serialize (list), outputJsonPath);
                }
            }
        }

        protected void CreateAttributeJson ()
        {
            Type[] types = Reflection.GetExecutingAssembly ();
            foreach (Type type in types) {
                ExcelAttribute excel = type.GetAttributeValue<ExcelAttribute> ();
                if (excel == null || excel.CreateFileName == "") {
                    continue;
                }

                List<Dictionary<string, object>> list = CreateAttributeJson (type, new CreateAttributeJsonConfig ());

                if (excel.IndexFlag) {
                    AttributeMappingConfig<ColumnAttribute>[] configs = Reflection.FieldAMCRetrieve<ColumnAttribute> (type);
                    List<string> keys = new List<string> ();
                    foreach (AttributeMappingConfig<ColumnAttribute> config in configs) {
                        if (config.t != null) {
                            ColumnAttribute column = config.t as ColumnAttribute;
                            if (column != null && column.CheckConstraints (DataConstraints.PK)) {
                                keys.Add (config.name);
                            }
                        }
                    }

                    if (keys.Count > 0) {
                        foreach (Dictionary<string, object> dic in list) {
                            StringBuilder stringBuilder = new StringBuilder ();
                            for (int i = 0; i < keys.Count; i++) {
                                if (i == 0) {
                                    stringBuilder.Append (dic [keys [i]]);
                                } else {
                                    stringBuilder.AppendFormat ("-{0}", dic [keys [i]]);
                                }
                            }

                            string createFileName = string.Format ("{0}{1}", excel.CreateFileName, stringBuilder.ToString ());
                            EditorUtil.CreateJsonFile (createFileName, Json.Serialize (dic), outputJsonPath);
                        }
                    }
                } else {
                    if (list != null && list.Count > 0) {
                        EditorUtil.CreateJsonFile (excel.CreateFileName, Json.Serialize (list), outputJsonPath);
                    }
                }
            }
        }

        protected Dictionary<string, object> CreateAttributeJson (CreateAttributeJsonConfig data)
        {
            Dictionary<string, object> sheetDic = new Dictionary<string, object> ();
            for (int k = 0; k <= data.titleRow.LastCellNum; k++) {
                ICell tCell = data.titleRow.GetCell (k);
                ICell vCell = data.row.GetCell (k);

                AddData (tCell, vCell, sheetDic);
            }

            Dictionary<string, object> dic = new Dictionary<string, object> ();
            foreach (AttributeMappingConfig<ColumnAttribute> config in data.configs) {
                if (sheetDic.ContainsKey (Util.ConvertCamelToUnderscore (config.name))) {
                    if (data.columnName == config.name) {
                        if (data.columnValue.ToString () != sheetDic [Util.ConvertCamelToUnderscore (config.name)].ToString ()) {
                            return null;
                        }
                    }

                    dic.Add (config.name, sheetDic [Util.ConvertCamelToUnderscore (config.name)]);
                } else {
                    if (!Util.IsValueType (config.type)) {
                        AttributeMappingConfig<ColumnAttribute>[] temp = Reflection.FieldAMCRetrieve<ColumnAttribute> (config.type);
                        foreach (AttributeMappingConfig<ColumnAttribute> c in temp) {
                            if (c.t != null) {
                                ColumnAttribute column = c.t as ColumnAttribute;
                                if (column != null) {
                                    if (sheetDic.ContainsKey (Util.ConvertCamelToUnderscore (column.Value))) {
                                        data.columnName = column.Value;
                                        data.columnValue = sheetDic [Util.ConvertCamelToUnderscore (column.Value)];
                                        dic.Add (config.name, CreateAttributeJson (config.type, data));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dic;
        }

        protected List<Dictionary<string, object>> CreateAttributeJson (Type type, CreateAttributeJsonConfig data)
        {
            ExcelAttribute excel = type.GetAttributeValue<ExcelAttribute> ();
            AttributeMappingConfig<ColumnAttribute>[] configs = Reflection.FieldAMCRetrieve<ColumnAttribute> (type);

            if (configs.Length > 0) {
                ISheet s = book.GetSheet (excel.SheetName);
                IRow titleRow = s.GetRow (0);
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
                for (int i = 1; i <= s.LastRowNum; i++) {
                    data.row = s.GetRow (i);
                    data.titleRow = titleRow;
                    data.configs = configs;
                    Dictionary<string, object> dic = CreateAttributeJson (data);
                    if (dic != null && dic.Count > 0) {
                        list.Add (dic);
                    }
                }

                return list;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Create Json the specified type and ignores.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="ignores">Ignores.</param>
        public void Create (ExcelImportType type, string[] ignores = null)
        {
            using (FileStream stream = new FileStream (excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                if (excelFilePath.EndsWith ("xls")) {
                    book = new HSSFWorkbook (stream);
                } else if (excelFilePath.EndsWith ("xlsx")) {
                    if (Application.platform == RuntimePlatform.OSXEditor) {
                        Debug.LogWarning ("xlsx is not supported on OSX.");
                        return;
                    } else {
                        book = new XSSFWorkbook (stream);
                    }
                } else {
                    return;
                }

                if (type == ExcelImportType.NORMAL) {
                    CreateNormalJson (ignores);
                } else {
                    CreateAttributeJson ();
                }
            }
        }
    }
}
