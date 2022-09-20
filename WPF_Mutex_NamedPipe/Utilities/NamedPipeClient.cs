using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Mutex_NamedPipe.Utilities
{
    internal class NamedPipeClient : NamedPipeBase
    {
        public static async Task<bool> SendMessageAsync(string message)
        {
            using var stream = new NamedPipeClientStream(PipeName);
            await stream.ConnectAsync();

            using var writer = new StreamWriter(stream);

            Console.WriteLine($"Send");
            await writer.WriteLineAsync(message);
            Console.WriteLine($"Sent: {message}");

            return true;
        }
    }
}
