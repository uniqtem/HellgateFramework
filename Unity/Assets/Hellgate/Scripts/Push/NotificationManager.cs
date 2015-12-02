//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;

namespace Hellgate
{
	public class NotificationManager : Notification
	{
#region Const
		public const string NOTIFICATION_MANAGER = "NotificationManager";
#endregion

#region Singleton
		private static NotificationManager instance = null;
		
		public static NotificationManager Instance {
			get {
				if (instance == null) {
					GameObject gObj = new GameObject ();
					instance = gObj.AddComponent<NotificationManager> ();
					instance.Awake ();

					gObj.name = NOTIFICATION_MANAGER;
					DontDestroyOnLoad (gObj);
				}
				
				return instance;
			}
		}
#endregion

		public event Action<string> devicePushIdReceivedEvent;
		public event Action<string> localNotificationReceivedEvent;
		public event Action<string> remoteNotificationReceivedEvent;

		protected override void Awake ()
		{
			if (instance == null) {
				base.Awake ();

				instance = this;
				DontDestroyOnLoad (this.gameObject);
			}
		}

		protected override void DevicePushIdReceived (string id)
		{
			if (devicePushIdReceivedEvent != null) {
				devicePushIdReceivedEvent (id);
			}
		}

		protected override void LocalNotificationReceived (string text)
		{
			if (localNotificationReceivedEvent != null) {
				localNotificationReceivedEvent (text);
			}
		}

		protected override void RemoteNotificationReceived (string text)
		{
			if (remoteNotificationReceivedEvent != null) {
				remoteNotificationReceivedEvent (text);
			}
		}

		public override void Register (string gcmSenderId = "")
		{
			base.Register (gcmSenderId);
		}

		public override string GetRegistrationId ()
		{
			return base.GetRegistrationId();
		}

		public override void ScheduleLocalNotification (DateTime dateTime, string text, string id = "", string title = "")
		{
			base.ScheduleLocalNotification (dateTime, text, id, title);
		}

		public override void CancelLocalNotification (string id)
		{
			base.CancelLocalNotification (id);
		}

		public override void CancelAllLocalNotifications ()
		{
			base.CancelAllLocalNotifications ();
		}
		
#if UNITY_ANDROID
		public override void Unregister ()
		{
			base.Unregister ();
		}

		public override void SetNotificationsEnabled (bool enabled)
		{
			base.SetNotificationsEnabled (enabled);
		}

		public override bool GetNotificationsEnabled ()
		{
			return base.GetNotificationsEnabled ();
		}
#endif
	}
}
