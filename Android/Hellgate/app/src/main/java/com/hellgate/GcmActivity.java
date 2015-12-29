package com.hellgate;

import android.app.Activity;
import android.os.AsyncTask;
import android.text.TextUtils;
import android.util.Log;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.gcm.GoogleCloudMessaging;
import com.unity3d.player.UnityPlayer;

import java.io.IOException;

public class GcmActivity {
	private final static int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;
	private GoogleCloudMessaging gcm;
	private String senderIds;
	private String regId;

	public GcmActivity() {
		senderIds = "";
		regId = "";
		if (gcm == null) {
			Activity activity = UnityPlayer.currentActivity;
			gcm = GoogleCloudMessaging.getInstance(activity);
		}
	}

	public boolean checkPlayServices() {
		Activity activity = UnityPlayer.currentActivity;
		int resultCode = GooglePlayServicesUtil.isGooglePlayServicesAvailable(activity);
		if (resultCode != ConnectionResult.SUCCESS) {
			if (GooglePlayServicesUtil.isUserRecoverableError(resultCode)) {
				GooglePlayServicesUtil.getErrorDialog(resultCode, activity, PLAY_SERVICES_RESOLUTION_REQUEST).show();
			} else {
				Log.i(Config.HELLGATE, "This device is not supported.");
			}

			return false;
		}

		return true;
	}

	public String register(final String senderIds) {
		this.senderIds = senderIds;
		if (TextUtils.isEmpty(senderIds)) {
			return regId;
		}

		if (!checkPlayServices()) {
			return regId;
		}

		new AsyncTask<Void, Void, String>() {
			@Override
			protected String doInBackground(Void... params) {
				try {
					regId = gcm.register(senderIds);
					Util.sendMessage(Config.NOTIFICATION_MANAGER, Config.DEVICE_PUSH_ID_RECEIVED, regId);
				} catch (IOException ex) {
					Log.e(Config.HELLGATE, ex.getMessage());
				}

				return "";
			}

			@Override
			protected void onPostExecute(String msg) {
				Log.d(Config.HELLGATE, msg);
			}

		}.execute(null, null, null);

		return regId;
	}

	public void unregister() {
		new AsyncTask<Void, Void, String>() {
			@Override
			protected String doInBackground(Void... params) {
				try {
					gcm.unregister();
				} catch (IOException ex) {
					Log.e(Config.HELLGATE, ex.getMessage());
				}

				return "";
			}

			@Override
			protected void onPostExecute(String msg) {
				Log.d(Config.HELLGATE, msg);
			}

		}.execute(null, null, null);
	}

	public String getRegistrationId() {
		if (senderIds == "") {
			return "";
		}

		if (regId == "") {
			register(senderIds);
		}

		return regId;
	}
}
