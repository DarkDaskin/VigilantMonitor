using System.Diagnostics;
using Microsoft.Win32;

namespace VigilantMonitor;

public class AutoRunService
{
    private const string AppName = "VigilantMonitor";

    private readonly RegistryKey _registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
        .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)!;

    public bool IsEnabled
    {
        get => _registryKey.GetValue(AppName) != null;
        set
        {
            if (value)
            {
                using var currentProcess = Process.GetCurrentProcess();
                _registryKey.SetValue(AppName, currentProcess.MainModule!.FileName!);
            }
            else
                _registryKey.DeleteValue(AppName, false);
        }
    }
}