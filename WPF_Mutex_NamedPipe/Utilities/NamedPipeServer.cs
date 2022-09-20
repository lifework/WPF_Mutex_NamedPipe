using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Mutex_NamedPipe.Utilities
{
    internal class NamedPipeServer : NamedPipeBase
    {
        private static async Task Server()
        {
            using var stream = new NamedPipeServerStream(PipeName);
            await stream.WaitForConnectionAsync();

            using var reader = new StreamReader(stream);
            var message = await reader.ReadLineAsync();
        }

    }
}
