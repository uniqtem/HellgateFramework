package com.hellgate;

public class UnityRegister {
    private static GcmActivity gcmActivity;
    private static ScheduleLocalNotification scheduleLocalNotification;


    public static boolean checkPlayServices() {
        if (gcmActivity == null) {
            gcmActivity = new GcmActivity();
        }
        return gcmActivity.checkPlayServices();
    }

    public static void register(final String senderIds) {
        if (gcmActivity == null) {
            gcmActivity = new GcmActivity();
        }
        gcmActivity.register(senderIds);
    }

    public static void unregister() {
        if (gcmActivity == null) {
            gcmActivity = new GcmActivity();
        }
        gcmActivity.unregister();
    }

    public static String getRegistrationId() {
        if (gcmActivity == null) {
            gcmActivity = new GcmActivity();
        }

        return gcmActivity.getRegistrationId();
    }

    public static void setNotificationsEnabled(boolean enabled) {
        Util.notificationsEnabled(enabled);
    }

    public static boolean getNotificationsEnabled() {
        return Util.notificationsEnabled();
    }

    public static void scheduleLocalNotification(String time, String title, String text, String id) {
        if (scheduleLocalNotification == null) {
            scheduleLocalNotification = new ScheduleLocalNotification();
        }

        scheduleLocalNotification.register(time, title, text, id);
    }

    public static void cancelLocalNotification(String id) {
        if (scheduleLocalNotification == null) {
            scheduleLocalNotification = new ScheduleLocalNotification();
        }

        scheduleLocalNotification.unregister(id);
    }

    public static void cancelAllLocalNotifications() {
        if (scheduleLocalNotification == null) {
            scheduleLocalNotification = new ScheduleLocalNotification();
        }

        scheduleLocalNotification.allUnregister();
    }
}
