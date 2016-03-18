//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class SelectORMMaker : SQLMaker
    {
        private class TableInfo
        {
            public Type type;
            public string tableName;
            public Type parentType;

            public TableInfo (Type type, string tableName, Type parentType = null)
            {
                this.type = type;
                this.tableName = tableName;
                this.parentType = parentType;
            }
        }

        private List<TableInfo> tableInfos;
        private List<string> selects;
        private List<string> tables;
        private List<string> wheres;
        private List<string> joins;
        private List<string> joinsQuerys;
        private Type type;
        private SqliteJoinType joinType;
        private string tableName;

        /// <summary>
        /// Gets the table type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type {
            get {
                return type;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.SelectORMMaker"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="tableName">Table name.</param>
        public SelectORMMaker (Type type, string tableName)
        {
            this.type = type;
            this.tableName = tableName;
            if (this.tableName == "") {
                this.tableName = Query.GetTableName (type);
            }

            selects = new List<string> ();
            tables = new List<string> ();
            wheres = new List<string> ();
            joins = new List<string> ();
            joinsQuerys = new List<string> ();

            tableInfos = new List<TableInfo> ();
            tableInfos.Add (new TableInfo (this.type, this.tableName));
            tables.Add (this.tableName);
        }

        /// <summary>
        /// Sets the type.
        /// </summary>
        /// <returns>The type.</returns>
        /// <param name="type">Type.</param>
        /// <param name="index">Index.</param>
        public string SetType (Type type, Type parent = null)
        {
            if (type == null) {
                this.type = type;
                return "";
            }

            this.type = type;
            this.tableName = Query.GetTableName (type);

            if (this.tableName != "") {
                if (!tables.Contains (this.tableName)) {
                    tableInfos.Add (new TableInfo (this.type, this.tableName, parent));
                    tables.Add (this.tableName);
                }
            }

            return this.tableName;
        }

        /// <summary>
        /// Sets the select.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        public void SetSelect (string fieldName)
        {
            selects.Add (GenerateSelectAliasSQL (this.tableName, fieldName));
        }

        /// <summary>
        /// Sets the join.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="joinTable">Join table.</param>
        /// <param name="joinFieldName">Join field name.</param>
        public void SetJoin (Type tableType, string fieldName, string joinFieldName)
        {
            string tableName = Query.GetTableName (tableType);
            string joinQuery = GenerateJoinSQL (tableName, this.tableName, fieldName, joinFieldName, joinType);

            if (joinQuery == "") {
                string key = Period (tableName, joinFieldName);
                string value = Period (this.tableName, fieldName);
                wheres.Add (EqualsSign (key, value));
            } else {
                joins.Add (this.tableName);
                joinsQuerys.Add (joinQuery);
            }
        }

        /// <summary>
        /// Sets the type of the join.
        /// </summary>
        /// <param name="joinType">Join type.</param>
        public void SetJoinType (SqliteJoinType joinType = SqliteJoinType.NONE)
        {
            this.joinType = joinType;
        }

        /// <summary>
        /// Execute this instance.
        /// </summary>
        public string GetQuery (string addQuery = "")
        {
            if (joins.Count > 0) {
                foreach (string j in joins) {
                    if (tables.Contains (j)) {
                        tables.Remove (j);
                    }
                }
            }

            return GenerateSelectSQL (selects, tables, joinsQuerys, wheres, addQuery);
        }
    }

    public partial class Query
    {
        protected System.Reflection.BindingFlags blindingFlags = System.Reflection.BindingFlags.NonPublic;

        /// <summary>
        /// Sets the blinding flags.
        /// Only BindingFlags.NonPublic and BindingFlags.Public
        /// </summary>
        /// <value>The blinding flags.</value>
        public System.Reflection.BindingFlags BlindingFlags {
            set {
                if (value == System.Reflection.BindingFlags.NonPublic || value == System.Reflection.BindingFlags.Public) {
                    blindingFlags = value;
                } else {
                    HDebug.LogWarning ("Only BindingFlags.NonPublic and BindingFlags.Public");
                }
            }
        }

        /// <summary>
        /// Cleans the useless.
        /// </summary>
        /// <returns>The useless.</returns>
        /// <param name="data">Data.</param>
        protected Dictionary<string, object> CleanUseless (Dictionary<string, object> data)
        {
            Dictionary<string, object> temp = new Dictionary<string, object> ();
            foreach (KeyValuePair<string, object> pair in data) {
                if (pair.Value != null) {
                    if (Util.IsValueType (pair.Value.GetType ())) {
                        temp.Add (pair.Key, pair.Value);
                    }
                }
            }

            return temp;
        }

        /// <summary>
        /// INSERT the specified tableName and t.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void INSERT<T> (string tableName, T t)
        {
            Dictionary<string, object> data = Reflection.Convert<T> (t, null, blindingFlags);
            data = CleanUseless (data);

            INSERT (tableName, data);
        }

        /// <summary>
        /// INSERT the specified t.
        /// </summary>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void INSERT<T> (T t)
        {
            INSERT<T> (GetTableName<T> (), t);
        }

        /// <summary>
        /// INSERT BATCH.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void INSERT_BATCH<T> (string tableName, List<T> list)
        {
            List<Dictionary<string, object>> data = Reflection.Convert<T> (list, blindingFlags);

            string[] columns = null;
            string[][] qData = new string[data.Count][];
            for (int i = 0; i < data.Count; i++) {
                data [i] = CleanUseless (data [i]);

                string[] temp = new string[data [i].Count];
                int index = 0;
                foreach (KeyValuePair<string, object> pair in data [i]) {
                    if (i == 0) {
                        if (columns == null) {
                            columns = new string[data [i].Count];
                        }
                        columns [index] = pair.Key;
                    }

                    temp [index] = pair.Value.ToString ();
                    index++;
                }

                qData [i] = temp;
            }

            INSERT_BATCH (tableName, columns, qData);
        }

        /// <summary>
        /// INSERT BATCH.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void INSERT_BATCH<T> (List<T> list)
        {
            INSERT_BATCH<T> (GetTableName<T> (), list);
        }

        /// <summary>
        /// INSERT BATCH.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void INSERT_BATCH<T> (T[] list)
        {
            INSERT_BATCH<T> (GetTableName<T> (), new List<T> (list));
        }

        /// <summary>
        /// UPDATE the specified t and addQuery.
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="addQuery">Add query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void UPDATE<T> (T t, string addQuery)
        {
            Dictionary<string, object> data = Reflection.Convert<T> (t, null, blindingFlags);
            data = CleanUseless (data);

            UPDATE (GetTableName<T> (), data, addQuery);
        }

        /// <summary>
        /// UPDATE the specified t, key and value.
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void UPDATE<T> (T t, string key, object value)
        {
            UPDATE<T> (t, GenerateWhereKeyValueSQL (key, value));
        }

        /// <summary>
        /// SELECT the specified tableName and addQuery.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="addQuery">Add query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected T[] SELECT<T> (string tableName, string addQuery)
        {
            if (tableName == "") {
                return null;
            }

            SelectORMMaker mapper = new SelectORMMaker (typeof(T), tableName);
            mapper = Reflection.SetSelectORMMaker (mapper, blindingFlags);

            DataTable data = ExecuteQuery (mapper.GetQuery (addQuery));
            if (data == null || data.Rows.Count <= 0) {
                return null;
            }

            return Reflection.Convert<T> (data, blindingFlags);
        }

        /// <summary>
        /// SELECT the specified addQuery.
        /// </summary>
        /// <param name="addQuery">Add query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T[] SELECT<T> (string addQuery = "")
        {
            return SELECT<T> (GetTableName<T> (), addQuery);
        }

        /// <summary>
        /// SELECT the specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T[] SELECT<T> (string key, object value)
        {
            return SELECT<T> (GetTableName<T> (), GenerateWhereKeyValueSQL (key, value));
        }

        /// <summary>
        /// DELETE the specified addQuery.
        /// </summary>
        /// <param name="addQuery">Add query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DELETE<T> (string addQuery = "")
        {
            DELETE (GetTableName<T> (), addQuery);
        }

        /// <summary>
        /// DELETE the specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DELETE<T> (string key, object value)
        {
            DELETE (GetTableName<T> (), key, value);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>The table name.</returns>
        /// <param name="type">Type.</param>
        public static string GetTableName (Type type)
        {
            TableAttribute table = type.GetAttributeValue<TableAttribute> ();

            if (table == null) {
                HDebug.LogError ("Not set the table attribute.");
                return "";
            } else if (table.TableName == "") {
                return type.Name;
            } else {
                return table.TableName;
            }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>The table name.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string GetTableName<T> ()
        {
            return GetTableName (typeof(T));
        }
    }
}
