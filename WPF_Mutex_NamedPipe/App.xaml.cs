using Prism.Ioc;
using System.Threading;
using System.Windows;
using WPF_Mutex_NamedPipe.Views;

namespace WPF_Mutex_NamedPipe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly string MutexName = "WPF_Mutex_NamedPipe";

        private static readonly System.Threading.Mutex mutex = new Mutex(false, MutexName);
        private static bool HasMutexHandle = false;
        public static bool IsNamedPipeServer = true;

        private void GetMutexLock()
        {
            HasMutexHandle = mutex.WaitOne(0, false);
            if (HasMutexHandle == false)
            {
                IsNamedPipeServer = false;
            }
        }

        private void ReleaseMutexLock()
        {
            if (HasMutexHandle)
            {
                mutex.ReleaseMutex();
            }
            mutex.Close();
        }


        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            GetMutexLock();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ReleaseMutexLock();
        }
    }
}
