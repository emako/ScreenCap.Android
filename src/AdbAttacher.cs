using System.Diagnostics;
using System.IO;
using System.Text;
using Vanara.PInvoke;

namespace ScreenCap;

public sealed class AdbAttacher
{
    public static string GetPath()
    {
        if (Process.GetProcessesByName("adb").FirstOrDefault() is Process p)
        {
            using Kernel32.SafeHPROCESS hProcess = Kernel32.OpenProcess(new ACCESS_MASK(Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION), false, (uint)p.Id);

            if (!hProcess.IsInvalid)
            {
                StringBuilder devicePath = new(260);
                uint size = (uint)devicePath.Capacity;

                if (Kernel32.QueryFullProcessImageName(hProcess, 0, devicePath, ref size))
                {
                    string path = devicePath.ToString();

                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }
        }
        return Path.GetFileName("adb.exe");
    }
}
