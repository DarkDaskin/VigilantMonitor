using ReactiveUI;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace VigilantMonitor;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = ViewModel = RxApp.SuspensionHost.GetAppState<MainViewModel>();

        Validation.AddErrorHandler(this, MainWindow_OnValidationError);

        this.WhenActivated(disposables =>
        {
            //this.Bind(ViewModel,
            //        vm => vm.MinDuration,
            //        v => v.MinDurationText.Text)
            //    .DisposeWith(disposables);

            //this.BindValidation(ViewModel,
            //    vm => vm.MinDuration,
            //    v => v.MinDurationValidationText.Text)
            //    .DisposeWith(disposables);
        });
    }

    private void MainWindow_OnValidationError(object? sender, ValidationErrorEventArgs e)
    {
        if (e.Error.RuleInError.ValidationStep == ValidationStep.ConvertedProposedValue)
            ViewModel!.HasConversionError = e.Action == ValidationErrorEventAction.Added;
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Visibility = Visibility.Hidden;
        e.Cancel = true;
    }
}