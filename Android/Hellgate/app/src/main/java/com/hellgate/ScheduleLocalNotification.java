package com.hellgate;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.SharedPreferences;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.Calendar;
import java.util.HashSet;
import java.util.Set;

public class ScheduleLocalNotification {
	private final static String DIVISION = "||";
	private AlarmManager alarmManager;
	private Activity activity;
	private SharedPreferences sharedPreferences;
	private SharedPreferences.Editor editor;

	public ScheduleLocalNotification() {
		activity = UnityPlayer.currentActivity;
		sharedPreferences = activity.getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE);
		editor = sharedPreferences.edit();
		alarmManager = null;
	}

	public void register(String time, String title, String text, String id) {
		Log.d(Config.HELLGATE, "register : " + time + "|" + title + "|" + text + "|" + id);
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeInMillis(System.currentTimeMillis());
		calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(time.substring(6, 8)));
		calendar.set(Calendar.HOUR_OF_DAY, Integer.parseInt(time.substring(8, 10)));
		calendar.set(Calendar.MINUTE, Integer.parseInt(time.substring(10, 12)));
		calendar.set(Calendar.SECOND, Integer.parseInt(time.substring(12, 14)));

		Set<String> stringSet = sharedPreferences.getStringSet(Config.LOCAL_NOTIFICATION_RECEIVED, new HashSet<String>());
		int requestCode = -1;
		if (stringSet.size() > 0) {
			for (String s : stringSet) {
				String[] parts = s.split(DIVISION);
				int code = Integer.valueOf(parts[1]);
				if (code > requestCode) {
					requestCode = code;
				}
			}
		}
		requestCode++;

		Intent intent = new Intent(activity, com.hellgate.UnityBroadcastReceiver.class);
		intent.putExtra(Config.SCHEDULE_LOCAL_NOTIFICATION, true);
		intent.putExtra("title", title);
		intent.putExtra("text", text);
		intent.putExtra("requestCode", requestCode);

		PendingIntent pendingIntent = PendingIntent.getBroadcast(activity, requestCode, intent, 0);
		alarmManager = (AlarmManager)activity.getSystemService(Activity.ALARM_SERVICE);
		alarmManager.set(AlarmManager.RTC, calendar.getTimeInMillis(), pendingIntent);

		stringSet.add(id + "||" + requestCode);
		editor.putStringSet(Config.LOCAL_NOTIFICATION_RECEIVED, stringSet);
		editor.commit();
	}

	public void unregister(String id) {
		Log.d(Config.HELLGATE, "unregister " + id);

		Set<String> stringSet = sharedPreferences.getStringSet(Config.LOCAL_NOTIFICATION_RECEIVED, new HashSet<String>());
		if (stringSet.size() <= 0) {
			return;
		}

		Intent intent = new Intent(activity, com.hellgate.UnityBroadcastReceiver.class);
		for (String s : stringSet) {
			String[] parts = s.split(DIVISION);
			if (id != "" && parts [0] != id) {
				continue;
			}

			int requestCode = Integer.valueOf(parts[1]);
			Log.d(Config.HELLGATE, "unregister Good " + requestCode);
			PendingIntent pendingIntent = PendingIntent.getBroadcast(activity, requestCode, intent, 0);
			alarmManager.cancel(pendingIntent);

			stringSet.remove(s);
		}

		editor.putStringSet(Config.LOCAL_NOTIFICATION_RECEIVED, stringSet);
		editor.commit();
	}

	public void allUnregister() {
		Log.d(Config.HELLGATE, "allUnregister");
		unregister("");
	}

	public static void unregister(int registerCode)
	{
		Log.d(Config.HELLGATE, "unregister static " + registerCode);
		ScheduleLocalNotification scheduleLocalNotification = new ScheduleLocalNotification();
		Set<String> stringSet = scheduleLocalNotification.sharedPreferences.getStringSet(Config.LOCAL_NOTIFICATION_RECEIVED,
			new HashSet<String>());
		for (String s: stringSet) {
			String[] parts = s.split(DIVISION);
			int code = Integer.valueOf(parts[1]);
			if (code == registerCode) {
				Log.d(Config.HELLGATE, "unregister static Good ");
				stringSet.remove(s);
				scheduleLocalNotification.editor.putStringSet(Config.LOCAL_NOTIFICATION_RECEIVED, stringSet);
				scheduleLocalNotification.editor.commit();
			}
		}
	}
}
