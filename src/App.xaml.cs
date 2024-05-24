using System.Windows;

namespace ScreenCap;

public partial class App : Application
{
    static App()
    {
        _ = DpiAwareHelper.SetProcessDpiAwareness();
    }
}
