#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
#endif

namespace Asis_Batia.Helpers;

public static class ImageResizerHelper {

    static ImageResizerHelper() { }

    public static async Task<byte[]> ResizeImage(byte[] imageData, int width, int height, bool rotate = true) {
#if __IOS__
        return ResizeImageIOS(imageData, width, height);
#endif
#if __ANDROID__
        return ResizeImageAndroid(imageData, width, height, rotate);
#else
        return null;
#endif
    }

#if __IOS__
    public static byte[] ResizeImageIOS(byte[] imageData, int width, int height) {
        UIImage originalImage = ImageFromByteArray(imageData);
        UIImageOrientation orientation = originalImage.Orientation;

        //create a 24bit RGB image
        using(CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                             width, height, 8,
                                             4 * width, CGColorSpace.CreateDeviceRGB(),
                                             CGImageAlphaInfo.PremultipliedFirst)) {

            RectangleF imageRect = new RectangleF(0, 0, width, height);

            // draw the image
            context.DrawImage(imageRect, originalImage.CGImage);
            //rotated 90° counterclockwise from the orientation of its original pixel data.
            UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, UIImageOrientation.Left);

            // save the image as a jpeg
            return resizedImage.AsJPEG().ToArray();
        }
    }

    private static UIImage ImageFromByteArray(byte[] data) {
        if(data == null) {
            return null;
        }

        UIKit.UIImage image;
        try {
            image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
        } catch(Exception e) {
            Console.WriteLine("Image load failed: " + e.Message);
            return null;
        }
        return image;
    }
#endif

#if __ANDROID__
    public static byte[] ResizeImageAndroid(byte[] imageData, int width, int height, bool rotate) {

        Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
        Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, width, height, false);       

        if(DeviceInfo.Current.Idiom == DeviceIdiom.Tablet || !rotate) {
            using(MemoryStream ms = new MemoryStream()) {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }

        Matrix matrix = new Matrix();
        matrix.SetRotate(90);
        Bitmap RotatedImage = Bitmap.CreateBitmap(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height, matrix, true);
        using(MemoryStream ms = new MemoryStream()) {
            RotatedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
            return ms.ToArray();
        }
    }
#endif
}
