//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HellgateSceneManagerEx : SceneManager
{
    [SerializeField]
    private string buildVersion = "1.0.0";

    protected override void Awake ()
    {
        base.Awake ();

#if UNITY_EDITOR
        PlayerSettings.bundleVersion = buildVersion;

        // set db
        // The example does not use the auto-generation db and table(DDL).
        Sqlite sql = new Sqlite ();
        if (sql.CreateFile ("hellgate.db")) {
            sql.CreateTable ("hellgate.db", new Board ().GetType ());
            sql.CreateTable ("hellgate.db", new Comment ().GetType ());
        }

//        if (sql.CreateFile ("Hellgate.db")) {
//            sql.ExecuteNonQuery ("CREATE TABLE `board` ( " +
//            "`idx` INTEGER PRIMARY KEY AUTOINCREMENT, " +
//            "`name` TEXT, " +
//            "`description` TEXT " +
//            ");");
//        }
#endif
    }
}
