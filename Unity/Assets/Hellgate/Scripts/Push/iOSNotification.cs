using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

namespace Hellgate
{
	public abstract partial class Notification : MonoBehaviour
	{
#if UNITY_IOS && !UNITY_EDITOR

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
				NotificationServices.CancelLocalNotification (NotificationServices.localNotifications [0]);
				NotificationServices.ClearLocalNotifications();
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
			Debug.Log ("ScheduleLocalNotification : " + date + " | " + text + " | " + id + " | " + title);
			LocalNotification notif = new LocalNotification ();
			notif.fireDate = date;
			notif.alertBody = text;
			
			Dictionary<string, string> userInfo = new Dictionary<string, string> (1);
			userInfo ["id"] = id;
			notif.userInfo = userInfo;
			NotificationServices.ScheduleLocalNotification (notif);
			
		}
		
		public virtual void CancelLocalNotification (string id)
		{
			Debug.Log ("CancelLocalNotification : " + id);
			LocalNotification[] notifis = NotificationServices.scheduledLocalNotifications;
			Debug.Log ("CancelLocalNotification count : " + notifis.Length);
			for (int i = 0; i < notifis.Length; i++) {
				if (notifis [i].userInfo ["id"].ToString() == id) {
					Debug.Log ("CancelLocalNotification clear : " + id);
					NotificationServices.CancelLocalNotification (notifis[i]);
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
