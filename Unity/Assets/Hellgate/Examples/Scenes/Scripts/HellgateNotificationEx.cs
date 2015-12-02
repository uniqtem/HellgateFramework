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
	[SerializeField]
	private GameObject
		gcmOnOff;
	private UILabel OnOffLabel;

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

#if UNITY_ANDROID
		OnOffLabel = gcmOnOff.GetComponentInChildren<UILabel> ();
		bool flag = NotificationManager.Instance.GetNotificationsEnabled ();
		if (flag) {
			OnOffLabel.text = "Noti OFF";
		} else {
			OnOffLabel.text = "Noti On";
		}
#else
		gcmOnOff.SetActive (false);
#endif

		NotificationManager.Instance.devicePushIdReceivedEvent += DevicePushIdReceived;
		NotificationManager.Instance.localNotificationReceivedEvent += LocalNotificationReceived;
		NotificationManager.Instance.remoteNotificationReceivedEvent += RemoteNotificationReceived;
	}

	public override void OnDisable ()
	{
		base.OnDisable ();

		NotificationManager.Instance.devicePushIdReceivedEvent -= DevicePushIdReceived;
		NotificationManager.Instance.localNotificationReceivedEvent -= LocalNotificationReceived;
		NotificationManager.Instance.remoteNotificationReceivedEvent -= RemoteNotificationReceived;
	}
	
	private void DevicePushIdReceived (string token)
	{
		Debug.Log ("DevicePushIdReceived : " + token);
	}

	private void LocalNotificationReceived (string text)
	{
		Debug.Log ("LocalNotificationReceived : " + text);
	}

	private void RemoteNotificationReceived (string text)
	{
		Debug.Log ("RemoteNotificationReceived : " + text);
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

	public void OnClickGcmOnOff ()
	{
#if UNITY_ANDROID
		bool flag = NotificationManager.Instance.GetNotificationsEnabled ();
		if (flag) {
			OnOffLabel.text = "Noti On";
		} else {
			OnOffLabel.text = "Noti Off";
		}

		NotificationManager.Instance.SetNotificationsEnabled (!flag);
#endif
	}

	public void OnClickLocalNoti30Second ()
	{
	}
}
