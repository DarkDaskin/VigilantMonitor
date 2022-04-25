using ReactiveUI;

namespace VigilantMonitor
{
    public partial class App
    {
        private readonly AutoSuspendHelper _autoSuspendHelper;

        public App()
        {
            _autoSuspendHelper = new AutoSuspendHelper(this);
            RxApp.SuspensionHost.CreateNewAppState = () => new MainViewModel();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new JsonSuspensionDriver<MainViewModel>());
        }
    }
}
