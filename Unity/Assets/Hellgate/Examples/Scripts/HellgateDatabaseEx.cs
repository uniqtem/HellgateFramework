//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

//[Table ("board", true)] // table name : "board", table auto generate
//[Table (true)] // table name : class name, table auto generate
[Table ("board")]
public class Board
{
    [Column (DataConstraints.AI)]
    private int idx = 0;
    private string name = "";
    private string description = "";
    [Join (SqliteJoinType.OUTER)]
    private Comment comment = null;

    public int Idx {
        get {
            return idx;
        }
    }

    public string Name {
        get {
            return name;
        }
    }

    public string Description {
        get {
            return description;
        }
    }

    public Comment Comment {
        get {
            return comment;
        }
    }

    public Board ()
    {
    }

    public Board (string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}

[Table ("comment")]
public class Comment
{
    [Column (DataConstraints.AI)]
    private int idx = 0;
    [Column (DataConstraints.FK, typeof(Board), "idx")]
    private int boardIdx = 0;
    private string name = "";
    private string description = "";

    public int Idx {
        get {
            return idx;
        }
    }

    public int BoardIdx {
        get {
            return boardIdx;
        }
    }

    public string Name {
        get {
            return name;
        }
    }

    public string Description {
        get {
            return description;
        }
    }
}

public class HellgateDatabaseEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject uName;
    [SerializeField]
    private GameObject description;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private GameObject temp;
    private string[] column;
    private Query query;
    private int idx;

    public static void GoDatabase ()
    {
        LoadingJobData jobData = new LoadingJobData ("HellgateDatabase");
        jobData.PutExtra ("title", "Database");

        SceneManager.Instance.LoadingJob (jobData);
    }

    public override void OnSet (object data)
    {
        base.OnSet (data);

        List<object> objs = data as List<object>;
        Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);

        SetLabelTextValue (title, intent ["title"].ToString ());
        idx = 0;

        query = new Query ("hellgate.db");
        temp.SetActive (false);
    }

    public override void Start ()
    {
        base.Start ();

        ListView ();
    }

    public override void OnKeyBack ()
    {
        base.Quit ("Exit ?");
    }

    private void ListView ()
    {
        HellgateTempEx[] childs = grid.GetComponentsInChildren<HellgateTempEx> ();
        if (childs.Length > 0) {
            for (int i = 0; i < childs.Length; i++) {
                Destroy (childs [i].gameObject);
            }
        }

        Board[] data = query.SELECT<Board> ();
        if (data == null) {
            return;
        }

        for (int i = 0; i < data.Length; i++) {
            GameObject gObj = Instantiate (temp) as GameObject;

            gObj.transform.SetParent (grid.transform);
            gObj.transform.localScale = new Vector3 (1f, 1f, 1f);
            gObj.transform.localPosition = new Vector3 (0, i, 0);

            SetLabelTextValue (gObj.FindChildObject<Transform> ("Name").gameObject, data [i].Name);
            SetLabelTextValue (gObj.FindChildObject<Transform> ("Description").gameObject, data [i].Description);

            gObj.FindChildObject ("Select").name = data [i].Idx.ToString ();
            gObj.FindChildObject ("Delete").name = data [i].Idx.ToString ();

            gObj.SetActive (true);
        }

        SetGridRepositionNow (grid);
    }

    private bool UIException ()
    {
        if (GetInputValue (uName) == "") {
            SceneManager.Instance.PopUp ("Input your name.", PopUpType.Ok);
            return true;
        }

        if (GetInputValue (uName) == "") {
            SceneManager.Instance.PopUp ("Input your description.", PopUpType.Ok);
            return true;
        }

        return false;
    }

    public void OnClickInsert ()
    {
        if (UIException ()) {
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object> ();
        data.Add ("name", GetInputValue (uName));
        data.Add ("description", GetInputValue (description));

        query.INSERT ("board", data);

        ListView ();
    }

    public void OnClickInsertORM ()
    {
        if (UIException ()) {
            return;
        }

        Board data = new Board (GetInputValue (uName), GetInputValue (description));
        query.INSERT<Board> (data);
        ListView ();
    }

    public void OnClickInsertBatchORM ()
    {
        List<Board> data = new List<Board> ();
        data.Add (new Board ("abc1", "abcd1"));
        data.Add (new Board ("abc2", "abcd2"));
        data.Add (new Board ("abc3", "abcd3"));

        query.INSERT_BATCH<Board> (data);
        ListView ();
    }

    public void OnClickUpdate ()
    {
        if (idx <= 0) {
            SceneManager.Instance.PopUp ("Select from the list.", PopUpType.Ok);
            return;
        }

        if (UIException ()) {
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object> ();
        data.Add ("name", GetInputValue (uName));
        data.Add ("description", GetInputValue (description));

        query.UPDATE ("board", data, "idx", idx);
        ListView ();
    }

    public void OnClickUpdateTransaction ()
    {
        if (idx <= 0) {
            SceneManager.Instance.PopUp ("Select from the list.", PopUpType.Ok);
            return;
        }
		
        if (UIException ()) {
            return;
        }
		
        Dictionary<string, object> data = new Dictionary<string, object> ();
        data.Add ("name", GetInputValue (uName));
        data.Add ("description", GetInputValue (description));

        query.Sqlite.BeginTransaction ();
        query.UPDATE ("board", data, "idx", idx);
        query.INSERT ("board", data);
        query.Sqlite.Commit ();
		
        ListView ();
    }

    public void OnClickUpdateORM ()
    {
        if (idx <= 0) {
            SceneManager.Instance.PopUp ("Select from the list.", PopUpType.Ok);
            return;
        }

        if (UIException ()) {
            return;
        }

        Board data = new Board (GetInputValue (uName), GetInputValue (description));
        query.UPDATE<Board> (data, "board.idx", idx);
        ListView ();
    }

    public void OnClickSelect (GameObject gObj)
    {
        DataTable data = query.SELECT ("Board", "idx", gObj.name);
        this.idx = int.Parse (gObj.name);

        SetInputValue (uName, data [0] ["name"].ToString ());
        SetInputValue (description, data [0] ["description"].ToString ());
    }

    public void OnClickSelectORM (GameObject gObj)
    {
        this.idx = int.Parse (gObj.name);
        Board[] data = query.SELECT<Board> ("board.idx", this.idx);

        SetInputValue (uName, data [0].Name);
        SetInputValue (description, data [0].Description);
    }

    public void OnClickDelete (GameObject gObj)
    {
        query.DELETE ("board", "idx", gObj.name);
        ListView ();
    }

    public void OnClickDeleteORM (GameObject gObj)
    {
        query.DELETE<Board> ("idx", gObj.name);
        ListView ();
    }

    public void OnClickDeleteAll ()
    {
        query.DELETE ("board");
        ListView ();
    }

    public void OnClickDeleteORMAll ()
    {
        query.DELETE <Board> ();
        ListView ();
    }
}
