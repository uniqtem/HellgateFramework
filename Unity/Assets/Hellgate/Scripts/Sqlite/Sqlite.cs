//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

#if !UNITY_WEBPLAYER
using Mono.Data.Sqlite;
#endif

namespace Hellgate
{
    [Table ("sqlite_master")]
    public class SqliteMastser
    {
        private string type = "";
        private string name = "";
        private string tbl_name = "";
        private int rootpage = 0;
        private string sql = "";

        public string Type {
            get {
                return type;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public string TblName {
            get {
                return tbl_name;
            }
        }

        public int Rootpage {
            get {
                return rootpage;
            }
        }

        public string Sql {
            get {
                return sql;
            }
        }
    }

    /// <summary>
    /// Sqlite join type.
    /// </summary>
    public enum SqliteJoinType
    {
        None = 1,
        Cross,
        Inner,
        // SQLite only supports the LEFT OUTER JOIN.
        Outer
    }

    public class Sqlite
    {
        public const string basePath = "URI=file:";
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

            if (!File.Exists (pathDB) || resetDB) {
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
            conn = basePath + conn;
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
                } catch (Exception e) {
                    HDebug.LogError (e.Message);
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

        /// <summary>
        /// Simples the migration.
        /// </summary>
        /// <param name="force">If set to <c>true</c> force.</param>
        public void SimpleMigration (bool force = true)
        {
            // StreamingAssets folder
            string path = Path.Combine (Application.streamingAssetsPath, Path.GetFileName (pathDB));
            if (path.Contains ("://")) { // android
                WWW www = new WWW (path);
                while (!www.isDone) {
                    ;
                }

                if (www.error == null) {
                    path = string.Format ("{0}_copy{1}", Path.GetFileNameWithoutExtension (path), Path.GetExtension (path));
                    path = Path.Combine (Application.persistentDataPath, path);
                    File.WriteAllBytes (path, www.bytes);
                } else {
                    HDebug.LogWarning (www.error);
                    return;
                }

                path = Path.GetFileName (path);
            }

            Query query = new Query (path);
            SqliteMastser[] resources = query.SELECT<SqliteMastser> ();
            List<SqliteMastser> list = new List<SqliteMastser> (resources);

            query = new Query (pathDB);
            SqliteMastser[] masters = query.SELECT<SqliteMastser> ();

            StringBuilder stringBuilder = new StringBuilder ();
            for (int i = 0; i < masters.Length; i++) {
                SqliteMastser master = list.Find (x => x.Name == masters [i].Name);
                if (master != null) {
                    list.Remove (master);
                }

                if (force) {
                    if (master != null) {
                        if (masters [i].Sql != master.Sql) {
                            stringBuilder.Append (query.GenerateDropTableSQL (master.Name));
                            stringBuilder.AppendLine ();
                            stringBuilder.AppendFormat ("{0};", master.Sql);
                            stringBuilder.AppendLine ();
                        }
                    } else {
                        stringBuilder.Append (query.GenerateDeleteSQL (masters [i].Name));
                        stringBuilder.AppendLine ();
                    }
                }
            }

            if (list.Count > 0) {
                for (int i = 0; i < list.Count; i++) {
                    stringBuilder.AppendFormat ("{0};", list [i].Sql);
                    stringBuilder.AppendLine ();
                }
            }

            if (stringBuilder.ToString () != "") {
                query.ExecuteNonQuery (stringBuilder.ToString ());
            }

            if (path.Contains ("://")) { // android
                File.Delete (path);
            }
        }

#region Editor

#if UNITY_EDITOR
        /// <summary>
        /// Creates the Sqlite db file.
        /// </summary>
        /// <param name="db">DB Name or Path.</param>
        /// <param name="resetDB">If set to <c>true</c> Reset Db.</param>
        public bool CreateFile (string db, bool resetDB = false)
        {
            string path = Application.streamingAssetsPath;
            string file = db;
            if (db.Contains ("/")) {
                path = Path.GetDirectoryName (db);
            } else {
                file = Path.Combine (path, db);
            }

            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }

            if (!resetDB) {
                if (File.Exists (file)) {
                    return false;
                }
            }

#if !UNITY_WEBPLAYER
            try {
                SqliteConnection.CreateFile (file);
            } catch (Exception e) {
                HDebug.LogError (e.Message);

                return false;
            }
#endif

            pathDB = file;
            UnityEditor.AssetDatabase.Refresh ();
            return true;
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <returns><c>true</c>, if file was created, <c>false</c> otherwise.</returns>
        /// <param name="resetDB">If set to <c>true</c> reset D.</param>
        public bool CreateFile (bool resetDB = false)
        {
            return CreateFile (pathDB, resetDB);
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
