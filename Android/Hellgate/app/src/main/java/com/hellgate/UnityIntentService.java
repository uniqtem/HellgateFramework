package com.hellgate;

import android.app.Activity;
import android.app.IntentService;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.gcm.GoogleCloudMessaging;
import com.unity3d.player.UnityPlayer;

public class UnityIntentService extends IntentService {

	public UnityIntentService() {
		super("UnityIntentService");
	}

	@Override
	protected void onHandleIntent(Intent intent) {
		// local
		boolean localNotification = intent.getBooleanExtra(Config.SCHEDULE_LOCAL_NOTIFICATION, false);
		Log.d(Config.HELLGATE, "onHandleIntent " + localNotification);
		if (localNotification) {
			String title = intent.getStringExtra("title");
			String text = intent.getStringExtra("text");

			if (Util.isForeground(this)) {
				Util.sendMessage(Config.LOCAL_NOTIFICATION_RECEIVED, text);
			} else {
				Util.showNotification(this, title, text, title);
			}

			ScheduleLocalNotification.unregister(intent.getIntExtra("requestCode", -1));
			return;
		}

		// gcm
		Bundle extras = intent.getExtras();
		GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(this);
		String messageType = gcm.getMessageType(intent);

		if (!extras.isEmpty()) {
			if (GoogleCloudMessaging.MESSAGE_TYPE_MESSAGE.equals(messageType)) {
				if (Util.isForeground(this)) {
					Util.sendMessage(Config.REMOTE_NOTIFICATION_RECEIVED, extras.getString("text"));
				} else {
					String ticker = extras.getString("ticker", "");
					if (ticker == "") {
						ticker = extras.getString("title");
					}

					Util.showNotification(this, extras.getString("title"), extras.getString("text"), ticker);
				}
			}
		}

		UnityBroadcastReceiver.completeWakefulIntent(intent);
	}
}
