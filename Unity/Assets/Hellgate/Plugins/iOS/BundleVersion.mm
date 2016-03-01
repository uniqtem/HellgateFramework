extern "C" {
    const char * _GetCFBundleVersion() {
        NSString *version = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
        return strdup([version UTF8String]);
    }
}