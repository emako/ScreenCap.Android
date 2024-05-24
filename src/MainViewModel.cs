using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Gma.System.MouseKeyHook;

namespace ScreenCap;

public partial class MainViewModel : ObservableObject
{
    protected static IKeyboardMouseEvents GlobalMouseHook = Hook.GlobalEvents();
    protected static string ScreenshotPath => Path.Combine(Path.GetTempPath(), "screencap.png");

    [ObservableProperty]
    private BitmapImage image = null!;

    public MainViewModel()
    {
        GlobalMouseHook.KeyDown += OnGlobalMouseHookKeyDown;
    }

    private void OnGlobalMouseHookKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.N)
        {
            ObtainScreenShotAsync()
                .ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task ObtainScreenShotAsync()
    {
        string adb = AdbAttacher.GetPath();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .Arguments("shell screencap /sdcard/screencap.png")
            .Start()
            .WaitForExitAsync();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .Arguments($"pull /sdcard/screencap.png \"{ScreenshotPath}\"")
            .Start()
            .WaitForExitAsync();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .Arguments("shell rm /sdcard/screencap.png")
            .Start()
            .WaitForExitAsync();

        BitmapImage bitmap = ImageExtension.LoadImage(ScreenshotPath);
        Clipboard.SetImage(bitmap);

        Image = bitmap;

        if (File.Exists(ScreenshotPath))
        {
            File.Delete(ScreenshotPath);
        }
    }
}
