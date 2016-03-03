using UnityEngine;
using System;
using System.Collections;

namespace Hellgate
{
    public abstract partial class Notification : MonoBehaviour
    {
        private const string SCHEDULE_LOCAL_NOTIFICATION = "HellgateScheduleLocalNotification";
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_PC
#region Const

        private const string CLASS_NAME = "com.hellgate.UnityRegister";

#endregion

        protected const string DATE_TIME = "yyyyMMddHHmmss";
        protected AndroidJavaClass android;

        protected virtual void Awake ()
        {
            if (Application.platform == RuntimePlatform.Android) {
                android = new AndroidJavaClass (CLASS_NAME);
            }
        }

        protected virtual void Update ()
        {
        }

        public virtual void Register (string gcmSenderId)
        {
            if (Application.platform == RuntimePlatform.Android) {
                android.CallStatic ("register", gcmSenderId);
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

        public virtual bool GetNotificationsEnabled ()
        {
            if (Application.platform == RuntimePlatform.Android) {
                bool flag = android.CallStatic<bool> ("getNotificationsEnabled");
                return flag;
            }

            return true;
        }

        public virtual void ScheduleLocalNotification (DateTime dateTime, string text, string id = "", string title = "")
        {
            if (Application.platform == RuntimePlatform.Android) {
                string time = dateTime.ToString (DATE_TIME);
                if (title == "") {
                    title = Application.productName;
                }

                if (id == "") {
                    id = SCHEDULE_LOCAL_NOTIFICATION;
                }

                android.CallStatic ("scheduleLocalNotification", time, title, text, id);
            }
        }

        public virtual void CancelLocalNotification (string id = "")
        {
            if (Application.platform == RuntimePlatform.Android) {
                if (id == "") {
                    id = SCHEDULE_LOCAL_NOTIFICATION;
                }

                android.CallStatic ("cancelLocalNotification", id);
            }
        }

        public virtual void CancelAllLocalNotifications ()
        {
            if (Application.platform == RuntimePlatform.Android) {
                android.CallStatic ("cancelAllLocalNotifications");
            }
        }

        protected abstract void DevicePushIdReceived (string gcmID);

        protected abstract void LocalNotificationReceived (string text);

        protected abstract void RemoteNotificationReceived (string text);
#endif
    }
}
