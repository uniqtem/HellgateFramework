package com.hellgate;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.support.v4.app.NotificationCompat;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class Util {
    public static void sendMessage(final String cls, final String method, final String message) {
        try {
            if (TextUtils.isEmpty(message)) {
                UnityPlayer.UnitySendMessage(cls, method, "");
            } else {
                UnityPlayer.UnitySendMessage(cls, method, message);
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

    public static void showNotification(Context context, String title, String text, String ticker, int notificationId) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE);
        boolean bool = sharedPreferences.getBoolean(Config.NOTIFICATION_ENABLED, true);
        if (!bool) {
            return;
        }

        Resources resources = context.getResources();
        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(context.getApplicationContext())
            .setSmallIcon(resources.getIdentifier("app_icon", "drawable", context.getPackageName()))
            .setTicker(ticker)
            .setContentTitle(title)
            .setContentText(text)
            .setDefaults(Notification.DEFAULT_LIGHTS | Notification.DEFAULT_SOUND)
            .setAutoCancel(true);

        Intent resultIntent = new Intent(context, UnityPlayerActivity.class);
        PendingIntent resultPendingIntent = PendingIntent.getActivity(
            context,
            Config.REQUEST_CODE_UNITY_ACTIVITY + notificationId,
            resultIntent,
            PendingIntent.FLAG_UPDATE_CURRENT);

        mBuilder.setContentIntent(resultPendingIntent);

        NotificationManager mNotifyMgr = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        mNotifyMgr.notify(notificationId, mBuilder.build());
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
    }

    public static boolean notificationsEnabled() {
        SharedPreferences sharedPreferences = UnityPlayer.currentActivity.getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE);
        return sharedPreferences.getBoolean(Config.NOTIFICATION_ENABLED, true);
    }

    public static void notificationsEnabled(boolean enabled) {
        SharedPreferences sharedPreferences = UnityPlayer.currentActivity.getSharedPreferences(Config.HELLGATE, Activity.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putBoolean(Config.NOTIFICATION_ENABLED, enabled);
        editor.commit();
    }
}
