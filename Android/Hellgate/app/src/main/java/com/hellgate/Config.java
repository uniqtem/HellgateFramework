package com.hellgate;

public class Config {
    public static final String HELLGATE = "Hellgate";
    public static final String SCHEDULE_LOCAL_NOTIFICATION = "ScheduleLocalNotification";
    public static final String NOTIFICATION_ENABLED = "notificationEnabled";
    public static final int REQUEST_CODE_UNITY_ACTIVITY = 1000;
    public static final  int REQUEST_CODE_GALLERY_ACTIVITY = 1;
    // receive class
    public static final String NOTIFICATION_MANAGER = "NotificationManager";
    public static final String WEBVIEW_MANAGER = "WebViewManager";
    public static final String GALLERY_MANAGER = "GalleryManager";
    // receive method
    public static final String DEVICE_PUSH_ID_RECEIVED = "DevicePushIdReceived";
    public static final String REMOTE_NOTIFICATION_RECEIVED = "RemoteNotificationReceived";
    public static final String LOCAL_NOTIFICATION_RECEIVED = "LocalNotificationReceived";
    public static final String WEBVIEW_PROGRESS_CHANGED = "OnProgressChanged";
    public static final String WEBVIEW_URL_CHANGED = "OnURLChanged";
    public static final String WEBVIEW_ERROR = "OnReceivedError";
    public static final String GALLERY_PATH_RECEIVED = "OnImageLoaded";
}
