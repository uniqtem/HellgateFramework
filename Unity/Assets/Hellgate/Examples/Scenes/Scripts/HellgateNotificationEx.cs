//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

public class HellgateNotificationEx : SceneController
{
	[SerializeField]
	private UILabel
		title;

	public static void GoNotification ()
	{
		LoadingJobData jobData = new LoadingJobData ("HellgateNotification");
		jobData.PutExtra ("title", "Notification\nPlease Mobile test.");

		SceneManager.Instance.LoadingJob (jobData);
	}

	public override void OnSet (object data)
	{
		base.OnSet (data);

		List<object> objs = data as List<object>;

		Dictionary<string, object> intent = Util.GetListObject<Dictionary<string, object>> (objs);
		title.text = intent ["title"].ToString ();
	}

	public override void OnKeyBack ()
	{
		base.Quit ("Exit ?");
	}

	public void OnClickRegister ()
	{
		NotificationManager.Instance.Register ("496264591522");
	}

	public void OnClickGcmApnsId ()
	{
		string id = NotificationManager.Instance.GetRegistrationId ();
		if (id == "") {
			Debug.Log ("Please on click register");
			return;
		}
		Debug.Log (id);
	}

	public void OnClickTest ()
	{
		Debug.Log ("Debug.Log");
	}

	public void OnClickTest2 ()
	{
		Debug.LogWarning ("Debug.LogWarning");
	}

	public void OnClickTest3 ()
	{
		Debug.LogError ("Debug.LogError");
	}
}
