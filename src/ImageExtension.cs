using System.Windows.Media.Imaging;

namespace ScreenCap;

internal static class ImageExtension
{
    public static BitmapImage LoadImage(string uriString)
    {
        BitmapImage bitmap = new();

        bitmap.BeginInit();
        bitmap.UriSource = new Uri(uriString);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bitmap.EndInit();
        return bitmap;
    }
}
