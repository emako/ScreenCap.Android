using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ScreenCap;

public partial class MainViewModel : ObservableObject
{
    protected static IKeyboardMouseEvents GlobalMouseHook = Hook.GlobalEvents();
    protected static string ScreenshotPath => Path.Combine(Path.GetTempPath(), "screencap.png");

    [ObservableProperty]
    private BitmapImage image = null!;

    [ObservableProperty]
    private byte[] imageData = [];

    public MainViewModel()
    {
        GlobalMouseHook.KeyDown += OnGlobalMouseHookKeyDown;

        _ = SetupAsync();
    }

    private async void OnGlobalMouseHookKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.N)
        {
            await CaptureScreenShotAsync();
        }
    }

    [RelayCommand]
    private async Task SetupAsync()
    {
        string adb = AdbAttacher.GetPath();
        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .UseShellExecute(false)
            .Arguments("devices -l")
            .Start()
            .WaitForExitAsync();
    }

    [RelayCommand]
    private async Task CaptureScreenShotAsync()
    {
        string adb = AdbAttacher.GetPath();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .UseShellExecute(false)
            .Arguments("shell screencap /sdcard/screencap.png")
            .Start()
            .WaitForExitAsync();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .UseShellExecute(false)
            .Arguments($"pull /sdcard/screencap.png \"{ScreenshotPath}\"")
            .Start()
            .WaitForExitAsync();

        await FluentProcess.Create()
            .FileName(adb)
            .CreateNoWindow()
            .UseShellExecute(false)
            .Arguments("shell rm /sdcard/screencap.png")
            .Start()
            .WaitForExitAsync();

        ImageData = File.ReadAllBytes(ScreenshotPath);
        BitmapImage bitmap = ImageExtension.LoadImage(ScreenshotPath);
        Clipboard.SetImage(bitmap);

        Image = bitmap;

        if (File.Exists(ScreenshotPath))
        {
            File.Delete(ScreenshotPath);
        }
    }

    [RelayCommand]
    private void SaveAsScreenShot()
    {
        if (ImageData == null || ImageData.Length == 0)
        {
            return;
        }

        SaveFileDialog saveFileDialog = new()
        {
            Filter = "PNG Files (*.png)|*.png",
            DefaultExt = "png",
            AddExtension = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = $"screencap_{DateTime.Now:yyyymmddHHMMssfff}.png"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            File.WriteAllBytes(saveFileDialog.FileName, ImageData);
        }
    }
}
