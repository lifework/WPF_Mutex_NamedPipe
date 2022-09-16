using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static WPF_Mutex_NamedPipe.NamedPipe;

namespace WPF_Mutex_NamedPipe
{
    internal class NamedPipe
    {
        private static string PipeName => $"{Environment.MachineName}.{Environment.UserName}.WPF_Mutex_NamedPipe";
        private const string NamedPipeServerName = ".";
        private static readonly int PipeConnectionTimeout = TimeSpan.FromSeconds(3).Milliseconds;

        private enum MessageCode : int
        {
            WindowActivateRequest = 1,
            ProtocolActivatedEvent = 2
        }

        // .NETで名前付きパイプを試す
        // https://ichiroku11.hatenablog.jp/archive/category/named-pipe

        private static async Task Server(int serverId, Func<Message, Message> process)
        {
            while (true)
            {
                using var stream = new NamedPipeServerStream(PipeName);
                await stream.WaitForConnectionAsync();

                var request = default(Message);
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    request = reader.ReadObject<Message>();
                }
                Console.WriteLine($"Server#{serverId} {nameof(request)}: {request}");

                // リクエストを処理してレスポンスを作る
                var response = process(request);

                // クライアントにレスポンスを送信する
                Console.WriteLine($"Server#{serverId} {nameof(response)}: {response}");
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteObject(response);
                }
            }
        }

        // サーバでの処理（Serverメソッドの引数に渡す）
        // Contentの文字列を逆順にする処理
        private static Message ServerProcess(Message message)
        {
            return new Message
            {
                Id = message.Id,
                Content = new string(message.Content.Reverse().ToArray()),
            };
        }

        private static async Task<Message> Client(int clientId, Message request)
        {
            using var stream = new NamedPipeClientStream(PipeName);
            // サーバに接続
            Console.WriteLine($"Client#{clientId} connecting");
            await stream.ConnectAsync();
            Console.WriteLine($"Client#{clientId} connected");

            // サーバにリクエストを送信する
            Console.WriteLine($"Client#{clientId} {nameof(request)}: {request}");
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteObject(request);
            }

            // サーバからレスポンスを受信する
            var response = default(Message);
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                response = reader.ReadObject<Message>();
            }
            Console.WriteLine($"Client#{clientId} {nameof(response)}: {response}");

            return response;
        }


        public class ObjectConverter<TObject>
        {
            private readonly IFormatter _formatter;

            public ObjectConverter(IFormatter formatter = null)
            {
                _formatter = formatter ?? new BinaryFormatter();
            }

            // オブジェクト=>バイト配列
            public byte[] ToByteArray(TObject obj)
            {
                using (var stream = new MemoryStream())
                {
                    _formatter.Serialize(stream, obj);
                    return stream.ToArray();
                }
            }

            // バイト配列=>オブジェクト
            public TObject FromByteArray(byte[] bytes)
            {
                using (var stream = new MemoryStream(bytes))
                {
                    return (TObject)_formatter.Deserialize(stream);
                }
            }
        }
    }


    // BinaryWriterの拡張メソッドを定義
    public static class BinaryWriterExtensions
    {
        // オブジェクトの書き込み
        public static void WriteObject<TObject>(this BinaryWriter writer, TObject obj)
        {
            // オブジェクトをバイト配列に変換
            var converter = new ObjectConverter<TObject>();
            var bytes = converter.ToByteArray(obj);

            // 長さを書き込んでからバイト配列を書き込む
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }
    }

    // BinaryReaderの拡張メソッドを定義
    public static class BinaryReaderExtensions
    {
        // オブジェクトの読み込み
        public static TObject ReadObject<TObject>(this BinaryReader reader)
        {
            // 長さを読み込んでから、その長さ分のバイト配列を読み込む
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);

            // バイト配列をオブジェクトに変換
            var converter = new ObjectConverter<TObject>();
            return converter.FromByteArray(bytes);
        }
    }

    // メッセージ
    [Serializable]
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $@"{{ {nameof(Id)} = {Id}, {nameof(Content)} = ""{Content}"" }}";
        }
    }
}
