using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

namespace Hellgate
{
	public abstract partial class Notification : MonoBehaviour
	{
#if UNITY_IOS
		protected bool tokenSent;
		protected string hexToken;

		protected virtual void Awake ()
		{
			tokenSent = false;
			hexToken = "";
		}
		
		protected virtual void Update ()
		{
			if (!tokenSent) {
				byte[] token = NotificationServices.deviceToken;
				if (token != null) {
					hexToken = "%" + System.BitConverter.ToString(token).Replace('-', '%');
					DevicePushIdReceived (hexToken);

					tokenSent = true;
				}
			}

			if (NotificationServices.localNotificationCount > 0) {
				LocalNotificationReceived (NotificationServices.localNotifications [0].alertBody);
				NotificationServices.ClearLocalNotifications ();
			}

			if (NotificationServices.remoteNotificationCount > 0) {
				LocalNotificationReceived (NotificationServices.remoteNotifications [0].alertBody);
				NotificationServices.ClearRemoteNotifications ();
			}
		}

		public virtual void Register (string id = "")
		{
			NotificationServices.RegisterForNotifications (
				NotificationType.Alert |
				NotificationType.Badge |
				NotificationType.Sound);
		}

		public virtual string GetRegistrationId ()
		{
			return hexToken;
		}

		public virtual void ScheduleLocalNotification (DateTime date, string text, string id = "", string title = "")
		{
			LocalNotification notif = new LocalNotification ();
			notif.fireDate = date;
			notif.alertBody = text;

			if (id != "") {
				Dictionary<string, string> userInfo = new Dictionary<string, string> (1);
				userInfo ["id"] = id;
				notif.userInfo = userInfo;
			}

			NotificationServices.ScheduleLocalNotification (notif);

		}

		public virtual void CancelLocalNotification (string id)
		{
			int numNotif = NotificationServices.localNotificationCount;
			for (int i = 0; i < numNotif; i++) {
				LocalNotification notif = NotificationServices.GetLocalNotification (i);
				if (notif.userInfo ["id"] == id) {
					NotificationServices.CancelLocalNotification (notif);
				}
			}
		}

		public virtual void CancelAllLocalNotifications ()
		{
			NotificationServices.CancelAllLocalNotifications ();
		}

		protected abstract void DevicePushIdReceived (string tokenID);
		protected abstract void LocalNotificationReceived (string message);
		protected abstract void RemoteNotificationReceived (string message);
#endif
	}
}
