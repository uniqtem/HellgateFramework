//
//  WebView.mm
//
//  Copyright © 2015 Uniqtem Co., Ltd. All rights reserved.
//

#import <UIKit/UIKit.h>

const char *WEBVIEW_MANAGER = "WebViewManager";
const char *WEBVIEW_PROGRESS_CHANGED = "OnProgressChanged";

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

static UIWebView *webView = nil;

@interface WebViewPlugin : UIViewController<UIWebViewDelegate>
{

    int progress;
    bool isFinish;
}
@end

@implementation WebViewPlugin

- (id)init
{
    self = [super init];
    webView.delegate = self;
    return self;
}

- (void)webViewDidStartLoad:(UIWebView *)webView
{
    progress = 0;
    isFinish = false;
    [NSTimer scheduledTimerWithTimeInterval:0.01667 target:self selector:@selector(timer) userInfo:nil repeats:YES];
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
    isFinish = true;
}

- (void)timer
{
    if (!isFinish) {
        NSString* str = [NSString stringWithFormat:@"%d", progress];
        UnitySendMessage (WEBVIEW_MANAGER, WEBVIEW_PROGRESS_CHANGED, [str UTF8String]);
        progress += 5;
        if (progress >= 95) {
            progress = 95;
        }
    }
}

@end

extern "C"
{
    void _WebViewInit();
    void _WebViewLoadURL(char *url, int left, int right, int top, int bottom);
    void _WebViewDestroy();
    void _WebViewSetMargins(int left, int right, int top, int bottom);
    void _WebViewSetVisibility(bool visibility);
}

void _WebViewInit()
{
    UIViewController *viewController = UnityGetGLViewController();
    webView = [[UIWebView alloc] initWithFrame:viewController.view.frame];
    webView.hidden = TRUE;
    
    [viewController.view addSubview:webView];
}

void _WebViewLoadURL(char *url, int left, int right, int top, int bottom)
{
    if (webView == nil) {
        _WebViewInit();
    }

    _WebViewSetMargins(left, right, top, bottom);
    [webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]]]];
}

void _WebViewDestroy()
{
    if (webView == nil) {
        return;
    }

    webView.hidden = FALSE;
    webView = nil;
}

void _WebViewSetMargins(int left, int right, int top, int bottom)
{
    if (webView == nil) {
        return;
    }

    UIViewController *viewController = UnityGetGLViewController();
    CGRect frame = viewController.view.frame;
    CGFloat scale = viewController.view.contentScaleFactor;
    CGRect screenBound = [[UIScreen mainScreen] bounds];
    CGSize screenSize = screenBound.size;

    frame.size.width = screenSize.width - (top + bottom) / scale;
    frame.size.width = screenSize.height - (left + right) / scale;

    frame.origin.x += left / scale;
    frame.origin.y += top / scale;

    webView.frame = frame;
}

void _WebViewSetVisibility(bool visibility) {
    webView.hidden = visibility;
}