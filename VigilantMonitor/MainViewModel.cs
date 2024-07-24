using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace VigilantMonitor;

[DataContract]
public class MainViewModelBase : ReactiveValidationObject
{
    public string Title => "Vigilant Monitor";

    [Reactive, DataMember]
    public bool IsEnabled { get; set; } = true;

    [Reactive, DataMember]
    public float MinDuration { get; set; }

    [Reactive]
    public bool RunAtWindowsStartup { get; set; }

    [Reactive]
    public bool HasConversionError { get; set; }

    [Reactive]
    public Visibility WindowVisibility { get; set; }

    public ICommand ShowSettingsCommand { get; }

    public ICommand HideSettingsCommand { get; }

    public ICommand ExitCommand { get; } = ReactiveCommand.Create(Application.Current.Shutdown);

    protected MainViewModelBase()
    {
        ShowSettingsCommand = ReactiveCommand.Create(() => WindowVisibility = Visibility.Visible);
        HideSettingsCommand = ReactiveCommand.Create(() => WindowVisibility = Visibility.Hidden, ValidationContext.Valid);

        this.ValidationRule(vm => vm.HasConversionError, v => !v,
            "Dummy text"); // Only for CanExecute, the message is taken from exception by WPF.
        this.ValidationRule(vm => vm.MinDuration, v => v >= 0,
            "Duration must be greater or equal to zero.");
    }
}

[DataContract]
public class MainViewModel : MainViewModelBase
{
    private readonly AudioMeterService _audioMeterService = new();
    private readonly DisplayLockService _displayLockService = new();
    private readonly AutoRunService _autoRunService = new();

    public MainViewModel()
    {
        WindowVisibility = Visibility.Hidden;

        RunAtWindowsStartup = _autoRunService.IsEnabled;
        this.ObservableForProperty(vm => vm.RunAtWindowsStartup)
            .DistinctUntilChanged()
            .Subscribe(oc => _autoRunService.IsEnabled = oc.Value);

        var scheduler = new SynchronizationContextScheduler(
            SynchronizationContext.Current ?? throw new InvalidOperationException("A synchronization context is required."));
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), scheduler)
            .Select(v => IsEnabled && _audioMeterService.CurrentVolume > 0)
            .DistinctUntilChanged()
            .Select(v => Observable.Return(v).Delay(TimeSpan.FromSeconds(MinDuration)))
            .Switch()
            .DistinctUntilChanged()
            .SkipWhile(v => !v)
            .Subscribe(HandleVolume);
    }

    private void HandleVolume(bool thresholdReached)
    {
        if (thresholdReached)
            _displayLockService.Lock();
        else
            _displayLockService.Unlock();
    }
}

public class MainViewModelDesignTime : MainViewModelBase
{
    public MainViewModelDesignTime()
    {
        WindowVisibility = Visibility.Visible;
        MinDuration = -1;
    }
}