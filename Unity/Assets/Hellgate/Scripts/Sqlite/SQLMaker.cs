/// <summary>
/// SQL maker.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hellgate
{
    public class SQLMaker : SQLConverter
    {
        protected string ConvertListToString (List<string> list, bool comma = true)
        {
            if (list == null || list.Count <= 0) {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder ();
            for (int i = 0; i < list.Count; i++) {
                stringBuilder.Append (list [i]);
                if (i < list.Count - 1) {
                    if (comma) {
                        stringBuilder.Append (", ");
                    } else {
                        stringBuilder.Append (" ");
                    }
                }
            }

            return stringBuilder.ToString ();
        }

        protected StringBuilder GenerateInsertSQL (StringBuilder stringBuilder, string[] sA, bool apostrophe = false)
        {
            string apos = apostrophe ? "'" : "";

            int index = 0;
            foreach (string s in sA) {
                index++;
                stringBuilder.AppendFormat ("{0}{1}{2}", apos, s, apos);

                if (sA.Length > index) {
                    stringBuilder.Append (", ");
                }
            }

            return stringBuilder;
        }

        /// <summary>
        /// Column Converts the camel to underscore.
        /// </summary>
        /// <returns>The camel to underscore.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        protected string ConvertCamelToUnderscore (string tableName, string columnName)
        {
            return Underline (tableName, Util.ConvertCamelToUnderscore (columnName));
        }

        /// <summary>
        /// Generates the insert SQL.
        /// </summary>
        /// <returns>The insert SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        public string GenerateInsertSQL (string tableName, string[] columnName, string[] data)
        {
            if (columnName.Length != data.Length) {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("INSERT INTO {0} (", tableName);
            stringBuilder = GenerateInsertSQL (stringBuilder, columnName);
            stringBuilder.Append (") VALUES (");
            stringBuilder = GenerateInsertSQL (stringBuilder, data, true);
            stringBuilder.Append (");");

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the insert batch SQL.
        /// </summary>
        /// <returns>The insert batch SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        public string GenerateInsertBatchSQL (string tableName, string[] columnName, string[][] data)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            for (int i = 0; i < data.Length; i++) {
                if (columnName.Length != data [i].Length) {
                    continue;
                }

                stringBuilder.Append (GenerateInsertSQL (tableName, columnName, data [i]));
                stringBuilder.AppendLine ();
            }

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the update SQL.
        /// </summary>
        /// <returns>The update SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        /// <param name="addQuery">Add query.</param>
        public string GenerateUpdateSQL (string tableName, string[] columnName, string[] data, string addQuery)
        {
            if (columnName.Length != data.Length) {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("UPDATE {0} SET ", tableName);

            for (int i = 0; i < columnName.Length; i++) {
                stringBuilder.AppendFormat ("{0} = '{1}'", columnName [i], data [i]);

                if (i < (columnName.Length - 1)) {
                    stringBuilder.Append (" , ");
                }
            }
            stringBuilder.AppendFormat (" {0};", addQuery);

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the update batch SQL.
        /// </summary>
        /// <returns>The update batch SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="whereName">Where name.</param>
        /// <param name="dataName">Data name.</param>
        /// <param name="where">Where.</param>
        /// <param name="data">Data.</param>
        public string GenerateUpdateBatchSQL (string tableName, string whereName, string dataName, string[] where, string[] data)
        {
            if (where.Length != data.Length) {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("UPDATE {0} SET {1} = CASE {2} ", tableName, dataName, whereName);

            for (int i = 0; i < where.Length; i++) {
                stringBuilder.AppendFormat ("WHEN {0} THEN '{1}' ", where [i], data [i]);
            }
            stringBuilder.AppendFormat ("END WHERE {0} IN (", whereName);

            for (int j = 0; j < where.Length; j++) {
                stringBuilder.AppendFormat ("'{0}'", where [j]);

                if (j < (where.Length - 1)) {
                    stringBuilder.Append (", ");
                }
            }
            stringBuilder.Append (");");

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the select SQL.
        /// </summary>
        /// <returns>The select SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="addQuery">Add query.</param>
        public string GenerateSelectSQL (string tableName, string addQuery = "")
        {
            return GenerateSelectSQL (
                new List<string> (new string[] { "*" }),
                new List<string> (new string[] { tableName }),
                null,
                null,
                addQuery);
        }

        /// <summary>
        /// Generates the select SQL.
        /// </summary>
        /// <returns>The select SQL.</returns>
        /// <param name="selects">Selects.</param>
        /// <param name="tables">Tables.</param>
        /// <param name="addQuery">Add query.</param>
        public string GenerateSelectSQL (List<string> selects,
                                         List<string> tables,
                                         List<string> joins = null,
                                         List<string> wheres = null,
                                         string addQuery = "")
        {
            if (wheres != null && wheres.Count > 0) {
                for (int i = 0; i < wheres.Count; i++) {
                    string temp = "";
                    if (i == 0) {
                        temp = "WHERE";
                    } else {
                        temp = "AND";
                    }

                    wheres [i] = string.Format ("{0} {1}", temp, wheres [i]);
                }
            }

            return string.Format (
                "SELECT {0} FROM {1} {2} {3} {4};",
                ConvertListToString (selects),
                ConvertListToString (tables),
                ConvertListToString (joins, false),
                ConvertListToString (wheres, false),
                addQuery
            );
        }

        /// <summary>
        /// Generates the select sync SQL.
        /// </summary>
        /// <returns>The select sync SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="whereKey">Where key.</param>
        /// <param name="where">Where.</param>
        public string GenerateSelectSyncSQL (string tableName, string whereKey, List<string> where)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("SELECT * FROM {0} WHERE ", tableName);

            for (int i = 0; i < where.Count; i++) {
                stringBuilder.AppendFormat ("{0} = '{1}'", whereKey, where [i]);

                if (i < where.Count - 1) {
                    stringBuilder.Append (" OR ");
                }
            }
            stringBuilder.Append (";");

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the delete SQL.
        /// </summary>
        /// <returns>The delete SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="addQuery">Add query.</param>
        public string GenerateDeleteSQL (string tableName, string addQuery = "")
        {
            return string.Format ("DELETE FROM {0} {1};", tableName, addQuery);
        }

        /// <summary>
        /// Generates the where key value SQL.
        /// </summary>
        /// <returns>The where key value SQL.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public string GenerateWhereKeyValueSQL (string key, object value)
        {
            return string.Format ("WHERE {0} = '{1}'", key, value);
        }

        /// <summary>
        /// Generates the create table SQL.
        /// </summary>
        /// <returns>The create table SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="configs">Configs.</param>
        public string GenerateCreateTableSQL (string tableName, AttributeMappingConfig<ColumnAttribute>[] configs)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("CREATE TABLE '{0}' (", tableName);

            for (int i = 0; i < configs.Length; i++) {
                StringBuilder temp = new StringBuilder ();
                temp.AppendFormat ("'{0}'", Util.ConvertCamelToUnderscore (configs [i].name));

                if (configs [i].t == null || configs [i].t.Type == "") {
                    temp.Append (ConvertToSQLType (configs [i].type));
                } else {
                    temp.Append (configs [i].t.Type);
                }

                if (configs [i].t != null) {
                    temp.Append (ConvertToSQLConstraints (configs [i].t.Constraints));
                }

                if (i < configs.Length - 1) {
                    temp.Append (", ");
                }

                stringBuilder.Append (temp);
            }

            stringBuilder.Append (");");
            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Generates the select alias SQL.
        /// </summary>
        /// <returns>The select alias SQ.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        public string GenerateSelectAliasSQL (string tableName, string columnName)
        {
            return string.Format ("{0} as {1}",
                                  Period (tableName, Util.ConvertCamelToUnderscore (columnName)),
                                  ConvertCamelToUnderscore (tableName,
                                                            columnName));
        }

        /// <summary>
        /// Generates the join SQL.
        /// </summary>
        /// <returns>The join SQL.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="joinTableName">Join table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="joinColumnName">Join column name.</param>
        /// <param name="type">Type.</param>
        public string GenerateJoinSQL (string tableName,
                                       string joinTableName,
                                       string columnName,
                                       string joinColumnName,
                                       SqliteJoinType type)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            string joinQuery = ConvertToSQLJoinType (type);
            if (joinQuery == "") {
                return joinQuery;
            }

            stringBuilder.AppendFormat (joinQuery, joinTableName);
            stringBuilder.Append (EqualsSign (Period (tableName, columnName), Period (joinTableName, joinColumnName))); 
            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Period the specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public string Period (string key, string value)
        {
            return string.Format ("{0}.{1}", key, value);
        }

        /// <summary>
        /// Equalses the sign.
        /// </summary>
        /// <returns>The sign.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public string EqualsSign (string key, string value)
        {
            return string.Format ("{0} = {1}", key, value);
        }

        /// <summary>
        /// Underline the specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public string Underline (string key, string value)
        {
            return string.Format ("{0}_{1}", key, value);
        }
    }
}