using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive.Disposables;
using System.Security.Principal;

namespace WPF_Mutex_NamedPipe.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        public IRegionManager RegionManager { get; private set; }
        public DelegateCommand<object> SendCommand { get; private set; }
        
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();
        public ReactivePropertySlim<string?> Title { get; set; } = new("WPF_NamedPipe");
        public ReactivePropertySlim<string?> ReceivedMessage { get; set; } = new();
        public ReactivePropertySlim<string?> InputMessage { get; set; } = new();


        public void Dispose()
        {
            Disposable.Dispose();
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            SendCommand = new DelegateCommand<object>(OnSendCommand);

            Disposable.Add(Title);
            Disposable.Add(ReceivedMessage);
            Disposable.Add(InputMessage);
        }

        private async void OnSendCommand(object parameter)
        {
            if (string.IsNullOrEmpty(InputMessage.Value))
            {
                return;
            }
            if (AppendReceivedMessage(InputMessage.Value))
            {
                InputMessage.Value = "";
            }
        }

        private bool AppendReceivedMessage(string message)
        {
            ReceivedMessage.Value += Now() + " " + InputMessage.Value + "\n";
            return true;
        }

        private string Now()
        {
            return "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]";
        }
    }
}
