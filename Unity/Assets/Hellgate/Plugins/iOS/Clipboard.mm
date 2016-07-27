//
//  Clipboard
//
//  Copyright © 2016 Uniqtem Co., Ltd. All rights reserved.
//

#import <UIKit/UIKit.h>

extern "C"
{
    void _SetText(const char* text)
    {
        UIPasteboard *board = [UIPasteboard generalPasteboard];
        [board setValue:[NSString stringWithUTF8String:text] forPasteboardType:@"public.utf8-plain-text"];
    }
}