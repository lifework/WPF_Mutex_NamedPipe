using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace WPF_Mutex_NamedPipe.Utilities
{
    internal class NamedPipeBase
    {
        public const string PipeName = "875aec39-8d51-d559-41cc-551c17518c72";
    }

    // .NET での JSON のシリアル化と逆シリアル化
    // https://learn.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json-overview?pivots=dotnet-6-0
    public class Message
    {
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

        public string Sender { get; private set; }
        public string Text { get; private set; }

        public Message(string sender, string text)
        {
            Sender = sender;
            Text = text;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize<Message>(this, JsonOptions);
        }

        public static Message? Deserialize(string? serialized)
        {
            try
            {
                if (string.IsNullOrEmpty(serialized))
                {
                    return null;
                }
                return JsonSerializer.Deserialize<Message>(serialized, JsonOptions);
            }
            catch(System.Text.Json.JsonException e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }
    }

}
