using System.Diagnostics;
using System.Net.Sockets;
using Xunit;

namespace GiftOfTheGivers.Tests;

public sealed class SeleniumWebAppFixture : IAsyncLifetime
{
    private Process? _webProcess;
    private readonly List<string> _output = new();
    private readonly object _outputLock = new();

    public string BaseUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        var port = GetAvailablePort();
        BaseUrl = $"http://127.0.0.1:{port}";
        var webAssemblyPath = FindWebAssemblyPath();
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(webAssemblyPath)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add(webAssemblyPath);

        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";
        startInfo.Environment["Testing__UseInMemoryDatabase"] = "true";
        startInfo.Environment["Testing__DatabaseName"] = $"SeleniumTests_{Guid.NewGuid():N}";
        startInfo.Environment["ConnectionStrings__DefaultConnection"] = "InMemory testing database";
        startInfo.Environment["AzureFunctions__BaseUrl"] = "http://127.0.0.1:1";
        startInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;

        _webProcess = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start the isolated test web host.");
        _webProcess.OutputDataReceived += (_, args) => CaptureOutput(args.Data);
        _webProcess.ErrorDataReceived += (_, args) => CaptureOutput(args.Data);
        _webProcess.BeginOutputReadLine();
        _webProcess.BeginErrorReadLine();

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (DateTime.UtcNow < deadline)
        {
            if (_webProcess.HasExited)
            {
                throw new InvalidOperationException($"The isolated test web host exited with code {_webProcess.ExitCode}. {GetCapturedOutput()}");
            }

            try
            {
                using var response = await client.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"The isolated test web host did not become ready within 60 seconds. {GetCapturedOutput()}");
    }

    public Task DisposeAsync()
    {
        if (_webProcess is { HasExited: false })
        {
            _webProcess.Kill(entireProcessTree: true);
            _webProcess.WaitForExit(10000);
        }

        _webProcess?.Dispose();
        return Task.CompletedTask;
    }

    private static int GetAvailablePort()
    {
        using var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        return ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
    }

    private void CaptureOutput(string? line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            lock (_outputLock)
            {
                _output.Add(line);
                if (_output.Count > 20)
                {
                    _output.RemoveAt(0);
                }
            }
        }
    }

    private string GetCapturedOutput()
    {
        lock (_outputLock)
        {
            return string.Join(Environment.NewLine, _output);
        }
    }

    private static string FindWebAssemblyPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var assemblyPath = Path.Combine(directory.FullName, "GiftOfTheGivers.dll");
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Could not locate the built GiftOfTheGivers.dll from the test output directory.");
    }
}
