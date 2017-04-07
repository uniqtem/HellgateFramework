//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HellgateEditor
{
    public class ShowWindow
    {
        [MenuItem ("Window/Hellgate/Build AssetBundles", false, 1)]
        public static void AssetBundleEditor ()
        {
            EditorWindow.GetWindow (typeof(AssetBundleEditor));
        }

        [MenuItem ("Window/Hellgate/Json Converter for Excel", false, 101)]
        public static void ExcelImportEditor ()
        {
            EditorWindow.GetWindow (typeof(ExcelImportEditor));
        }

        [MenuItem ("Window/Hellgate/Sqlite DB Converter for Excel", false, 102)]
        public static void ExcelSqliteEditor ()
        {
            EditorWindow.GetWindow (typeof(ExcelSqliteEditor));
        }

        [MenuItem ("Window/Hellgate/Create Sqlite DB", false, 201)]
        public static void SqliteEditor ()
        {
            EditorWindow.GetWindow (typeof(SqliteEditor));
        }

        [MenuItem ("Window/Hellgate/Json Converter for Sqlite DB", false, 202)]
        public static void SqliteImportEditor ()
        {
            EditorWindow.GetWindow (typeof(SqliteImportEditor));
        }
    }
}
