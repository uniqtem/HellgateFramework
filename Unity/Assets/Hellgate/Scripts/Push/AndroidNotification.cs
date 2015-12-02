using UnityEngine;
using System;
using System.Collections;

namespace Hellgate
{
	public abstract partial class Notification : MonoBehaviour
	{
#if UNITY_ANDROID || UNITY_PC
		private const string CLASS_NAME = "com.hellgate.UnityRegister";
		private const string DATE_TIME = "yyyyMMddHHmmss";
		protected AndroidJavaClass android;

		protected virtual void Start ()
		{
			android = new AndroidJavaClass (CLASS_NAME);
		}

		protected virtual void Update ()
		{
		}

		public virtual void Register (string gcmSenderId)
		{
			if (Application.platform == RuntimePlatform.Android) {
				using (AndroidJavaClass android = new AndroidJavaClass (CLASS_NAME)) {
					android.CallStatic ("register", gcmSenderId);
				}
			}
		}

		public virtual void Unregister ()
		{
			if (Application.platform == RuntimePlatform.Android) {
				android.CallStatic ("unregister");
			}
		}

		public virtual string GetRegistrationId ()
		{
			string id = "";
			if (Application.platform == RuntimePlatform.Android) {
				id = android.CallStatic<string> ("getRegistrationId");
			}

			return id;
		}

		public virtual void SetNotificationsEnabled (bool enabled)
		{
			if (Application.platform == RuntimePlatform.Android) {
				android.CallStatic ("setNotificationsEnabled", enabled);
			}
		}

		public virtual void ScheduleLocalNotification (DateTime dateTime, string text, string title = "")
		{
			if (Application.platform == RuntimePlatform.Android) {
				string time = dateTime.ToString (DATE_TIME);
				if (title == "") {
					title = Application.productName;
				}

				android.CallStatic ("scheduleLocalNotification", time, text, title);
			}
		}
		
		public virtual void CancelLocalNotification ()
		{
			if (Application.platform == RuntimePlatform.Android) {
				android.CallStatic ("cancelLocalNotification");
			}
		}
		
		public virtual void CancelAllLocalNotifications ()
		{
			if (Application.platform == RuntimePlatform.Android) {
				android.CallStatic ("cancelAllLocalNotifications");
			}
		}
		
		protected abstract void DeviceTokenReceived (string gcmID);
		protected abstract void LocalNotificationReceived (string text);
		protected abstract void RemoteNotificationReceived (string text);
#endif
	}
}
