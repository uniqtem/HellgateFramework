#import <UIKit/UIKit.h>

extern "C" {
    const char *_GetCFBundleVersion() {
        NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
        return strdup([version UTF8String]);
    }

    const char *_GetCFBundleShortVersionString() {
        NSString *version = [[NSBundle mainBundle] objectForInfoDictionaryKey: @"CFBundleShortVersionString"];
        return strdup([version UTF8String]);
    }
}