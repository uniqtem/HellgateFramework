package com.hellgate;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.SharedPreferences;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

public class ScheduleLocalNotification {
	private List<PendingIntent> pendingIntentList;
	private AlarmManager alarmManager;
	private Activity activity;
	private SharedPreferences sharedPreferences;
	private SharedPreferences.Editor editor;

	public ScheduleLocalNotification() {
		activity = UnityPlayer.currentActivity;
		sharedPreferences = activity.getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE);
		editor = sharedPreferences.edit();
		pendingIntentList = new ArrayList<PendingIntent>();
		alarmManager = null;
	}

	public void register(String time, String title, String text) {
		Log.d(Config.HELLGATE, "register : " + time + "|" + title + "|" + text);
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeInMillis(System.currentTimeMillis());
		calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(time.substring(6, 8)));
		calendar.set(Calendar.HOUR_OF_DAY, Integer.parseInt(time.substring(8, 10)));
		calendar.set(Calendar.MINUTE, Integer.parseInt(time.substring(10, 12)));
		calendar.set(Calendar.SECOND, Integer.parseInt(time.substring(12, 14)));

		Intent intent = new Intent(activity, com.hellgate.UnityBroadcastReceiver.class);
		intent.putExtra(Config.SCHEDULE_LOCAL_NOTIFICATION, true);
		intent.putExtra("title", title);
		intent.putExtra("text", text);

		int requestCode = sharedPreferences.getInt(Config.LOCAL_NOTIFICATION_RECEIVED, 0);
		PendingIntent pendingIntent = PendingIntent.getBroadcast(activity, requestCode, intent, 0);
		alarmManager = (AlarmManager)activity.getSystemService(Activity.ALARM_SERVICE);
		alarmManager.set(AlarmManager.RTC, calendar.getTimeInMillis(), pendingIntent);

		requestCode++;
		editor.putInt(Config.LOCAL_NOTIFICATION_RECEIVED, requestCode);
		editor.commit();

		pendingIntentList.add(pendingIntent);
	}

	public void unregister() {
		Log.d(Config.HELLGATE, "unregister " + pendingIntentList.size());
		if (pendingIntentList.size() <= 0) {
			return;
		}

		alarmManager.cancel(pendingIntentList.get(0));
		pendingIntentList.remove(0);

	}

	public void allUnregister() {
		int requestCode = sharedPreferences.getInt(Config.LOCAL_NOTIFICATION_RECEIVED, 0);
		Log.d(Config.HELLGATE, "allUnregister " + requestCode);
		if (requestCode <= 0) {
			return;
		}

		Intent intent = new Intent(activity, com.hellgate.UnityBroadcastReceiver.class);
		for (int i = 0; i < requestCode; i++) {
			PendingIntent pendingIntent = PendingIntent.getBroadcast(activity, 0, intent, 0);
			alarmManager.cancel(pendingIntent);
		}

		pendingIntentList.clear();
	}
}
