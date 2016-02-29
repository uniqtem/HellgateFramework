package com.hellgate;

import android.app.Activity;
import android.graphics.Color;
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

public class UnityWebViewActivity {
    private static FrameLayout layout;
    private WebView webView;

    public UnityWebViewActivity() {
    }

    public void init() {
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView != null) {
                    return;
                }

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

                if (layout == null) {
                    layout = new FrameLayout(activity);
                    activity.addContentView(layout, new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT));

                    layout.setFocusable(true);
                    layout.setFocusableInTouchMode(true);
                }

                layout.addView(webView, new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    Gravity.NO_GRAVITY));

                final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    Gravity.NO_GRAVITY);
                params.setMargins(0, 0, 0, 0);

                webView.setLayoutParams(params);
            }
        });
    }

    public void loadURL(final String url) {
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }

                webView.loadUrl(url);
            }
        });
    }

    public void destroy() {
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }

                layout.removeAllViews();
                webView = null;
            }
        });
    }

    public void setMargin(int left, int top, int right, int bottom) {
        final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MATCH_PARENT,
            ViewGroup.LayoutParams.MATCH_PARENT,
            Gravity.NO_GRAVITY);

        params.setMargins(left, top, right, bottom);

        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }
                webView.setLayoutParams(params);
            }
        });
    }

    public void setVisibility(final boolean bool) {
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }

                if (bool) {
                    webView.setVisibility(View.VISIBLE);
                    webView.requestFocus();
                } else {
                    webView.setVisibility(View.GONE);
                }
            }
        });
    }

    public void setBackground(final boolean bool) {
        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (webView == null) {
                    return;
                }

                if (bool) {
                    webView.setBackgroundColor(Color.WHITE);
                } else {
                    webView.setBackgroundColor(Color.TRANSPARENT);
                }
            }
        });
    }
}
