using System.IO.Pipes;

namespace MdToPdfConverter.Services;

public class SingleInstanceService : IDisposable
{
    private const string MutexName = "MdToPdfConverter_SingleInstance";
    private const string PipeName = "MdToPdfConverter_Pipe";

    private Mutex? _mutex;
    private CancellationTokenSource? _cts;

    public event Action<string>? FileReceived;

    public bool TryAcquire()
    {
        _mutex = new Mutex(true, MutexName, out var createdNew);
        if (!createdNew)
        {
            _mutex.Dispose();
            _mutex = null;
        }
        return createdNew;
    }

    public void StartListening()
    {
        _cts = new CancellationTokenSource();
        _ = ListenLoopAsync(_cts.Token);
    }

    public static async Task SendFilePathAsync(string filePath)
    {
        await using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
        await client.ConnectAsync(3000);
        await using var writer = new StreamWriter(client);
        await writer.WriteLineAsync(filePath);
        await writer.FlushAsync();
    }

    private async Task ListenLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(PipeName, PipeDirection.In);
                await server.WaitForConnectionAsync(ct);
                using var reader = new StreamReader(server);
                var filePath = await reader.ReadLineAsync(ct);
                if (!string.IsNullOrWhiteSpace(filePath))
                    FileReceived?.Invoke(filePath);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
    }
}
