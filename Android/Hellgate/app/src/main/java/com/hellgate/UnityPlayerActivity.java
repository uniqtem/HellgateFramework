package com.hellgate;

import android.content.Intent;
import android.os.Bundle;

public class UnityPlayerActivity extends com.unity3d.player.UnityPlayerActivity {
    private static UnityPlayerActivity instance;

    public static UnityPlayerActivity getInstance() {
        return instance;
    }

    @Override
    protected void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        instance = this;
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode != RESULT_OK || data == null) {
            return;
        }

        if (requestCode == Config.REQUEST_CODE_GALLERY_ACTIVITY) {
            UnityGalleryActivity.getInstance().onActivityResult(requestCode, resultCode, data);
        }
    }
}
