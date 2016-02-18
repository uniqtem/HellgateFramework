package com.hellgate;

import android.app.Activity;
import android.graphics.Point;
import android.os.Build;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.FrameLayout;

import com.unity3d.player.UnityPlayer;

public class WebViewActivity {
    private FrameLayout layout;
    private WebView webView;

    public WebViewActivity() {
        Log.d(Config.HELLGATE, "WebViewActivity");
    }

    public void init() {
        final Activity activity = UnityPlayer.currentActivity;
        webView = new WebView(activity);
        webView.setVisibility(View.GONE);
        webView.setFocusable(true);
        webView.setFocusableInTouchMode(true);

        webView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                Util.sendMessage(Config.WEBVIEW_MANAGER, Config.WEBVIEW_PROGRESS_CHANGED, String.valueOf(newProgress));
            }
        });

        webView.setWebViewClient(new WebViewClient());

        WebSettings webSettings = webView.getSettings();
        webSettings.setSupportZoom(false);
        webSettings.setJavaScriptEnabled(true);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN) {
            webSettings.setAllowUniversalAccessFromFileURLs(true);
        }

        layout = new FrameLayout(activity);
        activity.addContentView(layout, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MATCH_PARENT,
            ViewGroup.LayoutParams.MATCH_PARENT
        ));
        layout.setFocusable(true);
        layout.setFocusableInTouchMode(true);
        layout.addView(webView, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MATCH_PARENT,
            ViewGroup.LayoutParams.MATCH_PARENT,
            Gravity.NO_GRAVITY
        ));

        Log.d(Config.HELLGATE, layout.toString());
    }

    public void loadURL(final String url) {
        Log.d(Config.HELLGATE, "loadURL : " + url);
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(Config.HELLGATE, "loadURL run");
                if (webView == null) {
                    Log.d(Config.HELLGATE, "loadURL run null");
                    init();
                }
                webView.loadUrl(url);
            }
        });
    }

    public void destroy() {
        Log.d(Config.HELLGATE, "destroy");
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(Config.HELLGATE, "destroy run");
                if (webView == null) {
                    Log.d(Config.HELLGATE, "destroy run null");
                    return;
                }

                layout.removeAllViews();
                webView = null;
            }
        });
    }

    public void setMargin(int left, int right, int top, int bottom) {
        Log.d(Config.HELLGATE, "setMargin " + left + " / " + right + " / " + top + " / " + bottom);
        final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MATCH_PARENT,
            ViewGroup.LayoutParams.MATCH_PARENT,
            Gravity.NO_GRAVITY
        );
        params.setMargins(left, right, top, bottom);

        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    init();
                }
                webView.setLayoutParams(params);
            }
        });
    }

    public void setVisibility(final boolean bool) {
        Log.d(Config.HELLGATE, "setVisibility " + bool);
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }

                if (!bool) {
                    webView.setVisibility(View.VISIBLE);
                    webView.requestFocus();
                } else {
                    webView.setVisibility(View.GONE);
                }
            }
        });
    }
}
