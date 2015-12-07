package com.hellgate;

import android.app.Activity;
import android.app.IntentService;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.gcm.GoogleCloudMessaging;

public class UnityIntentService extends IntentService {

	public UnityIntentService() {
		super("UnityIntentService");
	}

	@Override
	protected void onHandleIntent(Intent intent) {
		// local
		boolean localNotification = intent.getBooleanExtra(Config.SCHEDULE_LOCAL_NOTIFICATION, false);
		if (localNotification) {
			String title = intent.getStringExtra("title");
			String text = intent.getStringExtra("text");

			if (Util.isForeground(this)) {
				Util.sendMessage(Config.LOCAL_NOTIFICATION_RECEIVED, text);
			} else {
				Util.showNotification(this, title, text, title, intent.getIntExtra("requestCode", 1));
			}

			ScheduleLocalNotification.unregister(intent.getIntExtra("requestCode", -1),
				getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE));
			return;
		}

		// gcm
		Bundle extras = intent.getExtras();
		GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(this);
		String messageType = gcm.getMessageType(intent);
		if (!extras.isEmpty()) {
			if (GoogleCloudMessaging.MESSAGE_TYPE_MESSAGE.equals(messageType)) {
				int internal = Integer.parseInt(extras.getString("internalType", "0"));
				if (internal > 2) {
					internal = 0;
				}

				String ticker = extras.getString("ticker", "");
				if (ticker == "") {
					ticker = extras.getString("title");
				}
				int requestCode = Integer.parseInt(extras.getString("requestCode", "1"));
				if (Util.isForeground(this)) {
					if (internal == 0 || internal == 1) {
						Util.sendMessage(Config.REMOTE_NOTIFICATION_RECEIVED, extras.getString("text"));
					} else {
						Util.showNotification(this, extras.getString("title"), extras.getString("text"), ticker, requestCode);
					}
				} else {
					if (internal == 0 || internal == 2) {
						Util.showNotification(this, extras.getString("title"), extras.getString("text"), ticker, requestCode);
					}
				}
			}
		}

		UnityBroadcastReceiver.completeWakefulIntent(intent);
	}
}
