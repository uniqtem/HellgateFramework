package com.hellgate;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.support.v4.app.NotificationCompat;
import android.text.TextUtils;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerProxyActivity;

public class Util {
	public static boolean notificatoinEnabled = true;
	public static void sendMessage(final String method, final String message) {
		try {
			if (TextUtils.isEmpty(message)) {
				UnityPlayer.UnitySendMessage(Config.RECEIVER_NAME, method, "");
			} else {
				UnityPlayer.UnitySendMessage(Config.RECEIVER_NAME, method, message);
			}
		} catch (UnsatisfiedLinkError e) {
			e.printStackTrace();
		}
	}

	public static void showToast(final String message) {
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
			public void run() {
				Toast.makeText(UnityPlayer.currentActivity, message,
					Toast.LENGTH_SHORT).show();
			}
		});
	}

	public static void showNotification(Context context, String title, String text, String ticker) {
		if (!notificatoinEnabled) {
			return;
		}

		Resources resources = context.getResources();
		NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(context.getApplicationContext())
			.setSmallIcon(resources.getIdentifier("app_icon", "drawable", context.getPackageName()))
			.setTicker(ticker)
			.setContentTitle(title)
			.setContentText(text)
			.setDefaults(Notification.DEFAULT_ALL)
			.setAutoCancel(true);

		Intent resultIntent = new Intent(context, UnityPlayerProxyActivity.class);
		PendingIntent resultPendingIntent = PendingIntent.getActivity(context,
			0,
			resultIntent,
			PendingIntent.FLAG_UPDATE_CURRENT);

		mBuilder.setContentIntent(resultPendingIntent);

		int mNotificationId = 001;
		NotificationManager mNotifyMgr = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
		mNotifyMgr.notify(mNotificationId, mBuilder.build());
	}

	public static void clearAllNotifications() {
		NotificationManager nm = (NotificationManager) UnityPlayer.currentActivity
			.getSystemService(Context.NOTIFICATION_SERVICE);
		nm.cancelAll();
	}

	public static boolean isForeground(Context context) {
		Activity activity = UnityPlayer.currentActivity;
		if (activity == null) {
			return false;
		} else {
			return true;
		}
//		if (activity != null) {
//			ActivityManager manager = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
//			List<ActivityManager.RunningAppProcessInfo> task = manager.getRunningAppProcesses();
//			ComponentName componentInfo = task.get(0).importanceReasonComponent;
//			Log.d(Config.HELLGATE, String.valueOf(componentInfo));
//
//			if (componentInfo.getPackageName().equals(activity.getPackageName())) {
//				return true;
//			}
//		}
//
//		return false;
	}
}
