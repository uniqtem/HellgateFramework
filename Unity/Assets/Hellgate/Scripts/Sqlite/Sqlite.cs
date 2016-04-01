//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

#if !UNITY_WEBPLAYER
using Mono.Data.Sqlite;
#endif

namespace Hellgate
{
    /// <summary>
    /// Sqlite join type.
    /// </summary>
    public enum SqliteJoinType
    {
        NONE = 1,
        CROSS,
        INNER,
        // SQLite only supports the LEFT OUTER JOIN.
        OUTER
    }

    public class Sqlite
    {
        public const string BASE_PATH = "URI=file:";
#if !UNITY_WEBPLAYER
        protected SqliteConnection dbconn;
        protected SqliteCommand dbcmd;
        protected SqliteDataReader reader;
        protected SqliteTransaction dbtrans;
#endif
        protected string pathDB;
        protected bool canQuery;
        protected bool isConnectionOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Sqlite"/> class.
        /// </summary>
        public Sqlite ()
        {
            canQuery = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Sqlite"/> class.
        /// The DB name '/' Do not put
        /// Data Base name. (the file needs exist in the streamingAssets folder)
        /// </summary>
        /// <param name="db">DB name && path.</param>
        /// <param name="resetDB">reset DB.</param>
        public Sqlite (string db, bool resetDB = false)
        {
            canQuery = true;

            if (db.Contains ("/")) {
                pathDB = db;
                return;
            }

            pathDB = Path.Combine (Application.persistentDataPath, db);
#if !UNITY_WEBPLAYER
            // StreamingAssets folder
            string resourcePath = Path.Combine (Application.streamingAssetsPath, db);

#if UNITY_EDITOR
            AutoDDL (db, false);
#endif

            if (!File.Exists (pathDB) || (File.GetLastWriteTimeUtc (resourcePath) > File.GetLastWriteTimeUtc (pathDB)) || resetDB) {
                if (resourcePath.Contains ("://")) { // android
                    WWW www = new WWW (resourcePath);
                    while (!www.isDone) {
                        ;
                    }

                    if (www.error == null) {
                        File.WriteAllBytes (pathDB, www.bytes);
                    } else {
                        canQuery = false;
                        HDebug.LogWarning (www.error);
                    }
                } else {
                    if (File.Exists (resourcePath)) {
                        File.Copy (resourcePath, pathDB, true);
                    } else {
                        canQuery = false;
                        HDebug.LogError ("The file DB named " + db + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                    }
                }
            }
#endif
        }

        protected void Open (string conn)
        {
#if !UNITY_WEBPLAYER
            conn = BASE_PATH + conn;
            dbconn = new SqliteConnection (conn);
            dbconn.Open (); //Open connection to the database.
            dbcmd = dbconn.CreateCommand ();
#endif
        }

        /// <summary>
        /// Open this DB.
        /// </summary>
        public bool Open ()
        {
            Open (pathDB);

#if !UNITY_WEBPLAYER
            if ((ConnectionState)dbconn.State == ConnectionState.Open) {
                isConnectionOpen = true;
            }
#endif

            return isConnectionOpen;
        }

        /// <summary>
        /// Close this DB.
        /// </summary>
        public void Close ()
        {
#if !UNITY_WEBPLAYER
            if (reader != null) {
                reader.Close ();
                reader = null;
            }

            if (dbcmd != null) {
                dbcmd.Dispose ();
                dbcmd = null;
            }

            if (dbconn != null) {
                dbconn.Close ();
                dbconn = null;
            }

            if (dbtrans != null) {
                dbtrans.Dispose ();
                dbtrans = null;
            }
#endif

            isConnectionOpen = false;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public void BeginTransaction ()
        {
#if !UNITY_WEBPLAYER
            if (!isConnectionOpen) {
                Open (pathDB);

                if ((ConnectionState)dbconn.State == ConnectionState.Open) {
                    isConnectionOpen = true;
                }
            }

            dbtrans = dbconn.BeginTransaction ();
            dbcmd.Transaction = dbtrans;
#endif
        }

        /// <summary>
        /// Commit this DB.
        /// </summary>
        public void Commit ()
        {
#if !UNITY_WEBPLAYER
            try {
                dbtrans.Commit ();
            } catch {
                try {
                    dbtrans.Rollback ();
                } catch (Exception e2) {
                    HDebug.LogError (e2.Message);
                }
            }
#endif

            Close ();
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="query">Query.</param>
        public DataTable ExecuteQuery (string query)
        {
            if (!canQuery) {
                HDebug.LogWarning ("Can't execute the query, verify DB origin file");
                return null;
            }

            if (!isConnectionOpen) {
                Open (pathDB);
            }

#if !UNITY_WEBPLAYER
            if ((ConnectionState)dbconn.State != ConnectionState.Open) {
                HDebug.LogWarning ("Sqlite DB is not open");
                return null;
            }


            dbcmd.CommandText = query;
            try {
                reader = dbcmd.ExecuteReader ();
            } catch (Exception e) {
                HDebug.Log ("Query : " + query);
                HDebug.LogError (e.Message);
                return null;
            }
#endif

            DataTable dataTable = new DataTable ();

#if !UNITY_WEBPLAYER
            for (int i = 0; i < reader.FieldCount; i++) {
                dataTable.Columns.Add (reader.GetName (i));
            }

            while (reader.Read ()) {
                DataRow row = new DataRow ();
                for (int i = 0; i < reader.FieldCount; i++) {
                    row.Add (reader.GetName (i), reader.GetValue (i));
                }

                dataTable.Rows.Add (row);
            }
#endif


            if (!isConnectionOpen) {
                Close ();
            }

            return dataTable;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="query">Query.</param>
        public void ExecuteNonQuery (string query)
        {
            if (!canQuery) {
                HDebug.LogWarning ("Can't execute the query, verify DB origin file");
                return;
            }

            if (!isConnectionOpen) {
                Open (pathDB);
            }
#if !UNITY_WEBPLAYER
            if ((ConnectionState)dbconn.State != ConnectionState.Open) {
                HDebug.LogWarning ("Sqlite DB is not open");
                return;
            }

            dbcmd.CommandText = query;
            try {
                dbcmd.ExecuteNonQuery ();
            } catch (Exception e) {
                HDebug.Log ("Query : " + query);
                HDebug.LogError (e.Message);
                return;
            }
#endif

            if (!isConnectionOpen) {
                Close ();
            }
        }

#region Editor

#if UNITY_EDITOR
        /// <summary>
        /// Creates the Sqlite db file.
        /// </summary>
        /// <param name="db">DB Name.</param>
        /// <param name="resetDB">If set to <c>true</c> Reset Db.</param>
        public bool CreateFile (string db, bool resetDB = false)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists (streamingAssetsPath)) {
                Directory.CreateDirectory (streamingAssetsPath);
            }
			
            string resourcePath = Path.Combine (streamingAssetsPath, db);
            if (!resetDB) {
                if (File.Exists (resourcePath)) {
                    return false;
                }
            }

#if !UNITY_WEBPLAYER
            try {
                SqliteConnection.CreateFile (resourcePath);
            } catch (Exception e) {
                HDebug.LogError (e.Message);

                return false;
            }
#endif

            pathDB = resourcePath;
            UnityEditor.AssetDatabase.Refresh ();
            return true;
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <returns>The table.</returns>
        /// <param name="db">Db.</param>
        /// <param name="type">Type.</param>
        /// <param name="returnQuery">If set to <c>true</c> return query.</param>
        /// <param name="tableAutoGenerated">If set to <c>true</c> table auto generated.</param>
        public string CreateTable (string db, Type type, bool returnQuery = false, bool tableAutoGenerated = false)
        {
            if (!returnQuery) {
                CreateFile (db);
            }

            /// table
            TableAttribute table = type.GetAttributeValue<TableAttribute> ();
            if (table != null) {
                if (tableAutoGenerated) {
                    if (!table.TableAutoGenerated) {
                        return "";
                    }
                }

                string tableName;
                if (table.TableName == "") {
                    tableName = type.Name;
                } else {
                    tableName = table.TableName;
                }

                // columns
                AttributeMappingConfig<ColumnAttribute>[] configs = Reflection.FieldAMCRetrieve<ColumnAttribute> (type);
                if (configs.Length > 0) {
                    SQLMaker sql = new SQLMaker ();
                    string query = sql.GenerateCreateTableSQL (tableName, configs);

                    if (returnQuery) {
                        return query;
                    } else {
                        ExecuteNonQuery (query);
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Autos the DDL(Data Definition Language).
        /// </summary>
        /// <param name="db">DB.</param>
        /// <param name="resetDB">If set to <c>true</c> reset DB.</param>
        public void AutoDDL (string db, bool resetDB = false)
        {
            if (!CreateFile (db, resetDB)) {
                return;
            }

            string query = "";

            // tables
            Type[] types = Reflection.GetExecutingAssembly ();
            for (int i = 0; i < types.Length; i++) {
                query += CreateTable (db, types [i], true, true) + " ";
            }

            if (query != "") {
                ExecuteNonQuery (query);
            }
        }
#endif

#endregion
    }
}
