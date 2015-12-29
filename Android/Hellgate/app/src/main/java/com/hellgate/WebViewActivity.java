package com.hellgate;

import android.app.Activity;
import android.os.Build;
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
	private Activity activity;
	private WebView webView;

	public WebViewActivity() {
		init();
	}

	public void init () {
		activity = UnityPlayer.currentActivity;
		webView = new WebView(activity);
		webView.setVisibility(View.GONE);
		webView.setFocusable(true);
		webView.setFocusableInTouchMode(true);
		webView.setVisibility(View.GONE);

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
	}

	public void loadURL(final String url) {
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (webView == null) {
					init();
				}
				webView.loadUrl(url);
			}
		});
	}

	public void destroy() {
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

	public void setMargin(int left, int right, int top, int bottom) {
		final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
			ViewGroup.LayoutParams.MATCH_PARENT,
			ViewGroup.LayoutParams.MATCH_PARENT,
			Gravity.NO_GRAVITY
		);
		params.setMargins(left, right, top, bottom);
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
