﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Diagnostics;

namespace WPF_Mutex_NamedPipe.Utilities
{
    internal class NamedPipeServer : NamedPipeBase
    {
        public static async Task ReceiveMessageAsync(Action<string> action)
        {
            while (true)
            {
                using var stream = new NamedPipeServerStream(PipeName);
                await stream.WaitForConnectionAsync();
                using var reader = new StreamReader(stream);
                var message = await reader.ReadLineAsync();
                Debug.WriteLine($"ReceiveMessage: {message}");
                action(message);
            }
        }
    }
}
