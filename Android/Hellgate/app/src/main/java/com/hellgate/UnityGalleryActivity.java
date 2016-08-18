package com.hellgate;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Base64;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.provider.MediaStore;

import java.io.ByteArrayOutputStream;

public class UnityGalleryActivity {
    private static UnityGalleryActivity instance;

    public static UnityGalleryActivity getInstance() {
        if (instance == null) {
            instance = new UnityGalleryActivity();
        }

        return instance;
    }

    public void openGallery() {
        Intent intent = new Intent();
        intent.setType("image/*");
        intent.setAction(Intent.ACTION_GET_CONTENT);
        com.hellgate.UnityPlayerActivity.getInstance().startActivityForResult(intent, Config.REQUEST_CODE_GALLERY_ACTIVITY);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        String path = "";
        Uri uri = data.getData();
        String[] column = {MediaStore.Images.Media.DATA};
        Cursor cursor = com.hellgate.UnityPlayerActivity.getInstance().getContentResolver().query(uri, column, null, null, null);
        cursor.moveToFirst();

        int index = cursor.getColumnIndex(column[0]);
        path = cursor.getString(index);
        cursor.close();

        if (path == null) {
            path = uri.getPath();
        }

        if (path != null) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.append(path);
            stringBuilder.append("|");

            String image = "";
            if (path != null) {
                Bitmap bitMap = BitmapFactory.decodeFile(path);
                float aspect = (float)bitMap.getWidth() / (float)bitMap.getHeight();
                float height = 512 / aspect;
                Bitmap scaledBitmap = bitMap.createScaledBitmap(bitMap, 512, (int)height, false);

                ByteArrayOutputStream stream = new ByteArrayOutputStream();
                scaledBitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
                image = Base64.encodeToString(stream.toByteArray(), 0);

                stringBuilder.append(image);
                stringBuilder.append("|");
                stringBuilder.append(bitMap.getWidth());
                stringBuilder.append("|");
                stringBuilder.append(bitMap.getHeight());
            }

            Util.sendMessage(Config.GALLERY_MANAGER, Config.GALLERY_PATH_RECEIVED, stringBuilder.toString());
        }
    }
}
