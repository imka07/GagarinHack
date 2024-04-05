#import <UIKit/UIKit.h>
#import <Photos/Photos.h>

extern "C" {

    void SaveImageToGallery(const unsigned char* data, int width, int height)
    {
        NSData *imageData = [[NSData alloc] initWithBytes:data length:width * height * 4];
        UIImage *image = [UIImage imageWithData:imageData];

        UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
    }

}

