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

        /// <summary>
        /// The NOTIFICATION_MANAGER.
        /// </summary>
        public const string NOTIFICATION_MANAGER = "NotificationManager";

#endregion

#region Singleton

        private static NotificationManager instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
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

        /// <summary>
        /// Occurs when device push identifier received event.
        /// </summary>
        public event Action<string> devicePushIdReceivedEvent;
        /// <summary>
        /// Occurs when local notification received event.
        /// </summary>
        public event Action<string> localNotificationReceivedEvent;
        /// <summary>
        /// Occurs when remote notification received event.
        /// </summary>
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

        /// <summary>
        /// Register the specified gcmSenderId.
        /// </summary>
        /// <param name="gcmSenderId">Gcm sender identifier.</param>
        public override void Register (string gcmSenderId = "")
        {
            base.Register (gcmSenderId);
        }

        /// <summary>
        /// Gets the registration identifier.
        /// </summary>
        /// <returns>The registration identifier.</returns>
        public override string GetRegistrationId ()
        {
            return base.GetRegistrationId ();
        }

        /// <summary>
        /// Schedules the local notification.
        /// "Hellgate ScheduleLocalNotification" id Do not use
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="title">Title.</param>
        public override void ScheduleLocalNotification (DateTime dateTime, string text, string id = "", string title = "")
        {
            base.ScheduleLocalNotification (dateTime, text, id, title);
        }

        /// <summary>
        /// Determines whether this instance cancel local notification the specified id.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel local notification the specified id; otherwise, <c>false</c>.</returns>
        /// <param name="id">Identifier.</param>
        public override void CancelLocalNotification (string id = "")
        {
            base.CancelLocalNotification (id);
        }

        /// <summary>
        /// Determines whether this instance cancel all local notifications.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel all local notifications; otherwise, <c>false</c>.</returns>
        public override void CancelAllLocalNotifications ()
        {
            base.CancelAllLocalNotifications ();
        }

#if UNITY_ANDROID
        /// <summary>
        /// Unregister this instance.
        /// </summary>
        public override void Unregister ()
        {
            base.Unregister ();
        }

        /// <summary>
        /// Sets the notifications enabled.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        public override void SetNotificationsEnabled (bool enabled)
        {
            base.SetNotificationsEnabled (enabled);
        }

        /// <summary>
        /// Gets the notifications enabled.
        /// </summary>
        /// <returns><c>true</c>, if notifications enabled was gotten, <c>false</c> otherwise.</returns>
        public override bool GetNotificationsEnabled ()
        {
            return base.GetNotificationsEnabled ();
        }
#endif
    }
}
