using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WPF_Mutex_NamedPipe.Utilities
{
    internal class NamedPipeClient : NamedPipeBase
    {
        public static async Task<bool> SendMessageAsync(Message message)
        {
            using var stream = new NamedPipeClientStream(PipeName);
            await stream.ConnectAsync();

            using var writer = new StreamWriter(stream);

            var json = message.Serialize();
            Debug.WriteLine($"SendMessageAsync: {json}");
            await writer.WriteAsync(json);

            return true;
        }
    }
}
