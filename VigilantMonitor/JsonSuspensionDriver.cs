using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reactive;
using System.Reactive.Linq;

namespace VigilantMonitor;

public class JsonSuspensionDriver<TState> : ISuspensionDriver
    where TState : class, new()
{
    private const string FileName = "settings.json";

    private readonly IsolatedStorageFile _storage = IsolatedStorageFile.GetUserStoreForApplication();
    private readonly JsonSerializer _serializer = new();

    public IObservable<object> LoadState()
    {
        return Observable.FromAsync(async () =>
        {
            await using var stream = _storage.OpenFile(FileName, FileMode.OpenOrCreate);
            if (stream.Length == 0)
                return new TState();
            await using var jsonReader = new JsonTextReader(new StreamReader(stream));
            var state = _serializer.Deserialize<TState>(jsonReader);
            return state ?? new TState();
        });
    }

    public IObservable<Unit> SaveState(object state)
    {
        return Observable.FromAsync(async () =>
        {
            await using var stream = _storage.OpenFile(FileName, FileMode.OpenOrCreate);
            await using var jsonWriter = new JsonTextWriter(new StreamWriter(stream));
            _serializer.Serialize(jsonWriter, state);
        });
    }

    public IObservable<Unit> InvalidateState()
    {
        _storage.DeleteFile(FileName);

        return Observable.Return(Unit.Default);
    }
}