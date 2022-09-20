using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive.Disposables;
using System.Security.Principal;
using System.Threading.Tasks;
using WPF_Mutex_NamedPipe.Utilities;

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

        public bool IsServer => WPF_Mutex_NamedPipe.App.IsNamedPipeServer;
        public bool SendButtonEnabled => !IsServer;
        public string SenderName => $"{Environment.ProcessId}";

        public void Dispose()
        {
            Disposable.Dispose();
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            SendCommand = new DelegateCommand<object>(OnSendCommand);

            Title.Value = $"{(IsServer ? "Server" : "Client")} - {SenderName}";

            Disposable.Add(Title);
            Disposable.Add(ReceivedMessage);
            Disposable.Add(InputMessage);

            if (IsServer)
            {
                Task.Run(() => NamedPipeServer.ReceiveMessageAsync(message => ProcessReceivedMessage(message)));
            }
        }

        private async void OnSendCommand(object parameter)
        {
            if (string.IsNullOrEmpty(InputMessage.Value))
            {
                return;
            }

            if (await SendPipedMessage(InputMessage.Value))
            {
                InputMessage.Value = "";
            }
        }

        private async Task<bool> SendPipedMessage(string text)
        {
            var message = new Message(SenderName, text);
            return await NamedPipeClient.SendMessageAsync(message);
        }

        private bool ProcessReceivedMessage(Message? message)
        {
            if (message != null)
            {
                ReceivedMessage.Value += $"[{Now()}][{message.Sender}] {message.Text}\n";
            }
            return true;
        }

        private string Now()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
