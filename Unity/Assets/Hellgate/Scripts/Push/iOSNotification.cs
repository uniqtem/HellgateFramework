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
		private List<LocalNotification> notifications;
		protected bool tokenSent;
		protected string hexToken;

		protected virtual void Start ()
		{
			notifications = null;
			tokenSent = false;
			hexToken = "";
		}
		
		protected virtual void Update ()
		{
			if (!tokenSent) {
				byte[] token = NotificationServices.deviceToken;
				if (token != null) {
					hexToken = "%" + System.BitConverter.ToString(token).Replace('-', '%');
					DeviceTokenReceived (hexToken);

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

		public virtual void ScheduleLocalNotification (DateTime date, string text, string title = "")
		{
			if (notifications == null) {
				notifications = new List<LocalNotification> ();
			}

			LocalNotification notif = new LocalNotification ();
			notif.fireDate = date;
			notif.alertBody = text;
			NotificationServices.ScheduleLocalNotification (notif);

			notifications.Add (notif);
		}

		public virtual void CancelLocalNotification ()
		{
			if (notifications == null || notifications.Count <= 0) {
				return;
			}

			NotificationServices.CancelLocalNotification (notifications [0]);
			notifications.RemoveAt (0);
		}

		public virtual void CancelAllLocalNotifications ()
		{
			NotificationServices.CancelAllLocalNotifications ();
		}

		protected abstract void DeviceTokenReceived (string tokenID);
		protected abstract void LocalNotificationReceived (string message);
		protected abstract void RemoteNotificationReceived (string message);
#endif
	}
}
