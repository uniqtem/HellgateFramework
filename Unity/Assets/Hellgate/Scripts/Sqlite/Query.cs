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
    /// <summary>
    /// Sqlite ORM and OM. Query template.
    /// </summary>
    public partial class Query : SQLMaker
    {
        private class Partitioning
        {
            public string[] keys;
            public string[] values;

            public Partitioning (Dictionary<string, object> data)
            {
                int count = data.Count;

                string[] keys = new string [count];
                data.Keys.CopyTo (keys, 0);

                object[] temp = new object [count];
                data.Values.CopyTo (temp, 0);

                string[] values = new string[count];
                for (int i = 0; i < count; i++) {
                    if (temp [i] == null) {
                        values [i] = "";
                    } else {
                        values [i] = temp [i].ToString ();
                    }
                }

                this.keys = keys;
                this.values = values;
            }
        }

        /// <summary>
        /// The sqlite.
        /// </summary>
        protected Sqlite sqlite;
        protected string dbName;

        public Sqlite Sqlite {
            get {
                return sqlite;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Query"/> class.
        /// </summary>
        /// <param name="db">Database name && path.</param>
        public Query (string db)
        {
            dbName = db;
            sqlite = new Sqlite (db);
        }

        /// <summary>
        /// Resets the DB.
        /// </summary>
        public void ResetDB ()
        {
            sqlite = new Sqlite (dbName, true);
        }

        /// <summary>
        /// INSERT batch.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        public void INSERT_BATCH (string tableName, string[] columnName, string[][] data)
        {
            string query = GenerateInsertBatchSQL (tableName, columnName, data);
            sqlite.ExecuteNonQuery (query);
        }

        /// <summary>
        /// INSERT the specified tableName, columnName and data.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        public void INSERT (string tableName, string[] columnName, string[] data)
        {
            string query = GenerateInsertSQL (tableName, columnName, data);
            sqlite.ExecuteNonQuery (query);
        }

        /// <summary>
        /// INSERT the specified tableName and data.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="data">Data.</param>
        public void INSERT (string tableName, Dictionary<string, object> data)
        {
            Partitioning part = new Partitioning (data);

            INSERT (tableName, part.keys, part.values);
        }

        /// <summary>
        /// UPDATE the specified tableName, columnName, data and addQuery.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="data">Data.</param>
        /// <param name="addQuery">Add query.</param>
        public void UPDATE (string tableName, string[] columnName, string[] data, string addQuery)
        {
            string query = GenerateUpdateSQL (tableName, columnName, data, addQuery);
            sqlite.ExecuteNonQuery (query);
        }

        /// <summary>
        /// UPDATE the specified tableName, data and addQuery.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="data">Data.</param>
        /// <param name="addQuery">Add query.</param>
        public void UPDATE (string tableName, Dictionary<string, object> data, string addQuery)
        {
            Partitioning part = new Partitioning (data);

            UPDATE (tableName, part.keys, part.values, addQuery);
        }

        /// <summary>
        /// UPDATE the specified tableName, data, key and value.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="data">Data.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void UPDATE (string tableName, Dictionary<string, object> data, string key, object value)
        {
            UPDATE (tableName, data, GenerateWhereKeyValueSQL (key, value));
        }

        /// <summary>
        /// UPDATE batch.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="whereName">Where name.</param>
        /// <param name="dataName">Data name.</param>
        /// <param name="where">Where.</param>
        /// <param name="data">Data.</param>
        public void UPDATE_BATCH (string tableName, string whereName, string dataName, string[] where, string[] data)
        {
            string query = GenerateUpdateBatchSQL (tableName, whereName, dataName, where, data);
            sqlite.ExecuteNonQuery (query);
        }

        /// <summary>
        /// SELECT the specified tableName and addQuery.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="addQuery">Add query.</param>
        public DataTable SELECT (string tableName, string addQuery = "")
        {
            string query = GenerateSelectSQL (tableName, addQuery);
            DataTable data = sqlite.ExecuteQuery (query);

            return data;
        }

        /// <summary>
        /// SELECT the specified tableName, key and value.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public DataTable SELECT (string tableName, string key, object value)
        {
            return SELECT (tableName, GenerateWhereKeyValueSQL (key, value));
        }

        /// <summary>
        /// SELECT synchronize.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="whereKey">Where key.</param>
        /// <param name="where">Where.</param>
        public DataTable SELECT_SYNC (string tableName, string whereKey, List<string> where)
        {
            string query = GenerateSelectSyncSQL (tableName, whereKey, where);
            DataTable data = sqlite.ExecuteQuery (query);

            return data;
        }

        /// <summary>
        /// DELETE the specified tableName and addQuery.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="addQuery">Add query.</param>
        public void DELETE (string tableName, string addQuery = "")
        {
            string query = GenerateDeleteSQL (tableName, addQuery);
            sqlite.ExecuteNonQuery (query);
        }

        /// <summary>
        /// DELETE the specified tableName, key and value.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void DELETE (string tableName, string key, object value)
        {
            DELETE (tableName, GenerateWhereKeyValueSQL (key, value));
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="query">Query.</param>
        public DataTable ExecuteQuery (string query)
        {
            return sqlite.ExecuteQuery (query);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="query">Query.</param>
        public void ExecuteNonQuery (string query)
        {
            sqlite.ExecuteNonQuery (query);
        }
    }
}
