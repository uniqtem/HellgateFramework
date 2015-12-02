package com.hellgate;

import android.util.Log;

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
		Log.v(Config.HELLGATE, "setNotificationsEnabled: " + enabled);
		Util.notificatoinEnabled = enabled;
	}

	public static void scheduleLocalNotification(String time, String title, String text) {
		if (scheduleLocalNotification == null) {
			scheduleLocalNotification = new ScheduleLocalNotification();
		}

		scheduleLocalNotification.register(time, title, text);
	}

	public static void cancelLocalNotification() {
		if (scheduleLocalNotification == null) {
			scheduleLocalNotification = new ScheduleLocalNotification();
		}

		scheduleLocalNotification.unregister();
	}

	public static void cancelAllLocalNotification() {
		if (scheduleLocalNotification == null) {
			scheduleLocalNotification = new ScheduleLocalNotification();
		}

		scheduleLocalNotification.allUnregister();
	}
}
