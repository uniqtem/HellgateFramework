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
	[Column (SqliteDataConstraints.AI)]
	private int idx = 0;
	private string name = "";
	private string description = "";

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
	[Column (SqliteDataConstraints.AI)]
	private int idx = 0;
	[Column (SqliteDataConstraints.FK, "board", "idx")]
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

public class HellgateDatabaseEx : SceneController
{
	[SerializeField]
	private UILabel
		title;
	[SerializeField]
	private UIInput
		uName;
	[SerializeField]
	private UIInput
		description;
	[SerializeField]
	private UIGrid
		grid;
	[SerializeField]
	private GameObject
		temp;
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

		title.text = intent ["title"].ToString ();
		idx = 0;

		query = new Query ("Hellgate.db");
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
		for (int i = 0; i < childs.Length; i++) {
			Destroy (childs [i].gameObject);
		}

		Board[] data = query.SELECT<Board> ();
		if (data == null) {
			return;
		}

		for (int i = 0; i < data.Length; i++) {
			GameObject gObj = Instantiate (temp) as GameObject;

			gObj.transform.parent = grid.transform;
			gObj.transform.localScale = new Vector3 (1f, 1f, 1f);
			gObj.transform.localPosition = new Vector3 (0, -i * 100, 0);

			Util.FindChildObject<UILabel> (gObj, "Name").text = data [i].Name.ToString ();
			Util.FindChildObject<UILabel> (gObj, "Description").text = data [i].Description.ToString ();

			Util.FindChildObject (gObj, "Select").name = data [i].Idx.ToString ();
			Util.FindChildObject (gObj, "Delete").name = data [i].Idx.ToString ();

			gObj.SetActive (true);
		}

		grid.repositionNow = true;
	}

	private bool UIException ()
	{
		if (uName.value == "") {
			SceneManager.Instance.PopUp ("Input your name.", PopUpType.Ok);
			return true;
		}
		
		if (description.value == "") {
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
		data.Add ("name", uName.value);
		data.Add ("description", description.value);

		query.INSERT ("board", data);

		ListView ();
	}

	public void OnClickInsertORM ()
	{
		if (UIException ()) {
			return;
		}
		
		Board data = new Board (uName.value, description.value);
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
		data.Add ("name", uName.value);
		data.Add ("description", description.value);

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
		data.Add ("name", uName.value);
		data.Add ("description", description.value);

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

		Board data = new Board (uName.value, description.value);

		query.UPDATE<Board> (data, "idx", idx);
		ListView ();
	}

	public void OnClickSelect ()
	{
		string idx = UIButton.current.name;

		DataTable data = query.SELECT ("Board", "idx", idx);
		this.idx = int.Parse (idx);
		uName.value = data [0] ["name"].ToString ();
		description.value = data [0] ["description"].ToString ();
	}

	public void OnClickSelectORM ()
	{
		string idx = UIButton.current.name;

		this.idx = int.Parse (idx);
		Board[] data = query.SELECT<Board> ("idx", this.idx);
		uName.value = data [0].Name;
		description.value = data [0].Description;
	}

	public void OnClickDelete ()
	{
		string idx = UIButton.current.name;

		query.DELETE ("board", "idx", idx);
		ListView ();
	}

	public void OnClickDeleteORM ()
	{
		string idx = UIButton.current.name;

		query.DELETE<Board> ("idx", idx);
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
