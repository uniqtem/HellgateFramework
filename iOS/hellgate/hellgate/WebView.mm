//
//  WebView.mm
//
//  Copyright © 2015 Uniqtem Co., Ltd. All rights reserved.
//

#import <UIKit/UIKit.h>

const char *WEBVIEW_MANAGER = "WebViewManager";
const char *WEBVIEW_PROGRESS_CHANGED = "OnProgressChanged";
const char *WEBVIEW_ERROR = "OnError";

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

@interface WebViewPlugin : UIViewController<UIWebViewDelegate>
{
    UIWebView *webView;
    NSTimer *timer;
    int progress;
    int sendProgress;
    bool isFinish;
}
@end

@implementation WebViewPlugin

- (id)init
{
    self = [super init];

    UIViewController *viewController = UnityGetGLViewController();
    webView = [[UIWebView alloc] initWithFrame:viewController.view.frame];
    webView.delegate = self;

    [viewController.view addSubview:webView];

    return self;
}

- (void)loadURL:(const char *)url
{
    [webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]]]];
}

- (void)setMargin:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
    UIView *view = UnityGetGLViewController().view;
    CGRect frame = webView.frame;
    CGRect screen = view.bounds;

    CGFloat scale = view.contentScaleFactor;
    if ([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0) {
        scale = view.window.screen.nativeScale;
    }
    scale = 1.0f / scale;

    frame.size.width = screen.size.width - scale * (left + right);
    frame.size.height = screen.size.height - scale * (top + bottom);
    frame.origin.x = scale * left;
    frame.origin.y = scale * top;

    webView.frame = frame;
}

- (void)setVisibility:(BOOL)flag
{
    if (flag) {
        webView.hidden = FALSE;
    } else {
        webView.hidden = TRUE;
    }
}

- (void)setBackground:(BOOL)flag
{
    if (flag) {
        webView.opaque = YES;
        webView.backgroundColor = [UIColor whiteColor];
    } else {
        webView.opaque = NO;
        webView.backgroundColor = [UIColor clearColor];
    }
}

- (void)goBack
{
    [webView goBack];
}

- (void)goForward
{
    [webView goForward];
}

- (bool)canGoBack
{
    return [webView canGoBack];
}

- (bool)canGoForward
{
    return [webView canGoForward];
}

- (void)reload
{
    [webView reload];
}

- (void)destory
{
    [webView removeFromSuperview];
    webView = nil;
}

- (void)dealloc
{
    if (webView != nil) {
        [webView removeFromSuperview];
    }

    webView = nil;
}

- (void)webViewDidStartLoad:(UIWebView *)webView
{
    progress = 0;
    sendProgress = 0;
    isFinish = FALSE;

    UnitySendMessage (WEBVIEW_MANAGER, WEBVIEW_PROGRESS_CHANGED, [@"0" UTF8String]);
    timer = [NSTimer scheduledTimerWithTimeInterval:0.01667 target:self selector:@selector(timer) userInfo:nil repeats:YES];
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
    isFinish = TRUE;

    [timer invalidate];
    UnitySendMessage (WEBVIEW_MANAGER, WEBVIEW_PROGRESS_CHANGED, [@"100" UTF8String]);
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
    isFinish = TRUE;

    [timer invalidate];
    UnitySendMessage (WEBVIEW_MANAGER, WEBVIEW_ERROR, [[error localizedDescription] UTF8String]);
}

- (void)timer
{
    if (!isFinish) {
        if (sendProgress < progress) {
            sendProgress = progress;

            NSString* str = [NSString stringWithFormat:@"%d", sendProgress];
            UnitySendMessage (WEBVIEW_MANAGER, WEBVIEW_PROGRESS_CHANGED, [str UTF8String]);
        }

        progress += 5;
        if (progress >= 95) {
            progress = 95;
        }
    }
}

@end

static WebViewPlugin *instance = nil;

extern "C"
{
    void _WebViewInit();
    void _WebViewLoadURL(const char *url);
    void _WebViewDestroy();
    void _WebViewSetMargin(int left, int top, int right, int bottom);
    void _WebViewSetVisibility(bool flag);
    void _WebViewSetBackground(bool flag);
    void _WebViewGoBack();
    void _WebViewGoForward();
    bool _WebViewCanGoBack();
    bool _WebViewCanGoForward();
    void _WebViewReload();
}

void _WebViewInit()
{
    instance = [[WebViewPlugin alloc] init];
}

void _WebViewLoadURL(const char *url)
{
    if (instance == nil) {
        _WebViewInit();
    }

    [instance loadURL:url];
}

void _WebViewDestroy()
{
    [instance destory];
    instance = nil;
}

void _WebViewSetMargin(int left, int top, int right, int bottom)
{
    if (instance == nil) {
        _WebViewInit();
    }

    [instance setMargin:left top:top right:right bottom:bottom];
}

void _WebViewSetVisibility(bool flag)
{
    if (instance == nil) {
        _WebViewInit();
    }

    [instance setVisibility:flag];
}

void _WebViewSetBackground(bool flag)
{
    if (instance == nil) {
        _WebViewInit();
    }

    [instance setBackground:flag];
}

void _WebViewGoBack()
{
    if (instance == nil) {
        return;
    }

    [instance goBack];
}

void _WebViewGoForward()
{
    if (instance == nil) {
        return;
    }

    [instance goForward];
}

bool _WebViewCanGoBack()
{
    if (instance == nil) {
        return false;
    }

    return [instance canGoBack];
}

bool _WebViewCanGoForward()
{
    if (instance == nil) {
        return false;
    }

    return [instance canGoForward];
}

void _WebViewReload()
{
    if (instance == nil) {
        return;
    }

    [instance reload];
}