using System.Runtime.InteropServices;
using MikuSB.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace MikuSB.Proxy;

public sealed class WindowsSystemProxyService(
    IOptions<ProxyOptions> options,
    ILogger<WindowsSystemProxyService> logger) : IHostedService, IDisposable
{
    private const string InternetSettingsPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
    private readonly ProxyOptions _options = options.Value;
    private ConsoleCtrlHandler? _consoleCtrlHandler;
    private int _proxyDisabled;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled || !_options.ManageSystemProxy)
            return Task.CompletedTask;

        if (!OperatingSystem.IsWindows())
        {
            logger.LogWarning("System proxy management is only supported on Windows");
            return Task.CompletedTask;
        }

        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsPath, writable: true);
        if (key is null)
        {
            logger.LogWarning("Unable to open Windows Internet Settings registry key");
            return Task.CompletedTask;
        }

        var proxyServer = $"http=127.0.0.1:{_options.Port};https=127.0.0.1:{_options.Port}";

        key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
        key.SetValue("ProxyServer", proxyServer, RegistryValueKind.String);
        key.SetValue("ProxyOverride", _options.ProxyOverride, RegistryValueKind.String);
        NotifyProxySettingsChanged();
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        RegisterConsoleCtrlHandler();

        logger.LogWarning("Windows system proxy enabled for MikuSB: {ProxyServer}", proxyServer);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled || !_options.ManageSystemProxy || !_options.RestoreSystemProxyOnStop)
            return Task.CompletedTask;

        DisableSystemProxy();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        UnregisterConsoleCtrlHandler();

        if (_options.Enabled && _options.ManageSystemProxy && _options.RestoreSystemProxyOnStop)
            DisableSystemProxy();
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        if (_options.Enabled && _options.ManageSystemProxy && _options.RestoreSystemProxyOnStop)
            DisableSystemProxy();
    }

    private void DisableSystemProxy()
    {
        if (!OperatingSystem.IsWindows())
            return;

        if (Interlocked.Exchange(ref _proxyDisabled, 1) == 1)
            return;

        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsPath, writable: true);
        if (key is null)
            return;

        key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
        key.DeleteValue("ProxyServer", throwOnMissingValue: false);
        key.DeleteValue("ProxyOverride", throwOnMissingValue: false);
        NotifyProxySettingsChanged();
        logger.LogWarning("Windows system proxy disabled for MikuSB shutdown");
    }

    private void RegisterConsoleCtrlHandler()
    {
        if (!OperatingSystem.IsWindows())
            return;

        _consoleCtrlHandler = OnConsoleCtrl;
        SetConsoleCtrlHandler(_consoleCtrlHandler, add: true);
    }

    private void UnregisterConsoleCtrlHandler()
    {
        if (!OperatingSystem.IsWindows() || _consoleCtrlHandler is null)
            return;

        SetConsoleCtrlHandler(_consoleCtrlHandler, add: false);
        _consoleCtrlHandler = null;
    }

    private bool OnConsoleCtrl(int signal)
    {
        if (_options.Enabled && _options.ManageSystemProxy && _options.RestoreSystemProxyOnStop)
            DisableSystemProxy();

        return false;
    }

    private static void NotifyProxySettingsChanged()
    {
        InternetSetOption(IntPtr.Zero, 39, IntPtr.Zero, 0);
        InternetSetOption(IntPtr.Zero, 37, IntPtr.Zero, 0);
    }

    private delegate bool ConsoleCtrlHandler(int signal);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandler handler, bool add);

    [DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetSetOption(IntPtr internet, int option, IntPtr buffer, int bufferLength);
}
