//
//  ImagePicker
//
//  Copyright © 2016 Uniqtem Co., Ltd. All rights reserved.
//

#import <UIKit/UIKit.h>

const char *GALLERY_MANAGER = "GalleryManager";
const char *GALLERY_IMAGELOADED = "OnImageLoaded";

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

@interface ImagePickerPlugin : UIViewController <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
{
}

@property (strong, nonatomic)NSMutableString *mstSelectedImage;

@end

@implementation ImagePickerPlugin

-(void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info
{
    UIImage *myUIImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    NSData *imageData = UIImagePNGRepresentation(myUIImage);
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];

    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"temp.png"];
    [imageData writeToFile:filePath atomically:YES];

    _mstSelectedImage = (NSMutableString *)filePath;

    NSString *temp = [NSString stringWithFormat:@"%s|%d|%d", MakeStringCopy([_mstSelectedImage UTF8String]), (int)myUIImage.size.width, (int)myUIImage.size.height];
    UnitySendMessage(GALLERY_MANAGER, GALLERY_IMAGELOADED, [temp UTF8String]);

    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
}

char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

@end

static ImagePickerPlugin *instance = nil;

extern "C"
{
    void _ImagePickerInit();
    void _ImagePickerOpen();
}

void _ImagePickerInit()
{
    instance = [[ImagePickerPlugin alloc] init];
}

void _ImagePickerOpen()
{
    if (instance == nil) {
        _ImagePickerInit();
    }

    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = instance;
    if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum]) {
        picker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    }else{
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    }

    [UnityGetGLViewController() presentViewController:picker animated:YES completion:NULL];
}