using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace PrefixLib; 

public static class Prefix {
    public static async Task SendInt32 (this TcpClient client, int data) {
        
        byte[] bytes = BitConverter.GetBytes (data);
        await client.GetStream ().WriteAsync (bytes);
    }

    public static async Task SendInt64 (this TcpClient client, long data) {
        byte[] bytes = BitConverter.GetBytes (data);
        await client.GetStream ().WriteAsync (bytes);
    }

    public static async Task SendSingle (this TcpClient client, float data) {
        byte[] bytes = BitConverter.GetBytes (data);
        await client.GetStream ().WriteAsync (bytes);
    }

    public static async Task SendBoolean (this TcpClient client, bool data) {
        byte[] bytes = BitConverter.GetBytes (data);
        await client.GetStream ().WriteAsync (bytes);
    }

    public static async Task SendPrefixBytes (this TcpClient client, byte[] bytes) {
        // сначала отправить длину
        await client.SendInt32 (bytes.Length);
        // а потом сами данные
        await client.GetStream ().WriteAsync (bytes);
    }

    public static async Task SendString (this TcpClient client, string data) {
        await client.SendPrefixBytes (Encoding.UTF8.GetBytes (data));
    }

    public static async Task SendObjectAsJson (this TcpClient client, object data) {
        // сериализуем object -> string
        string json = JsonSerializer.Serialize (data);
        // отправляем string
        await client.SendString (json);
    }

    public static async Task SendBinaryWriter (this TcpClient client, Action<BinaryWriter> serialize) {
        MemoryStream memory = new ();  // накопитель байтов
        BinaryWriter writer = new BinaryWriter (memory);  // умеет писать
        serialize (writer);  // что именно сериализуем?
        await client.SendPrefixBytes (memory.ToArray ());  // отправляем массив байтов
    }

    public static async Task SendFileContent (this TcpClient client, Stream file) {
        await client.SendInt64 (file.Length);

        // await file.CopyToAsync (client.GetStream ());

        byte[] bytes = new byte[1024];
        long pos = 0;
        while (pos < file.Length) {
            int read = await file.ReadAsync (bytes);
            await client.GetStream ().WriteAsync (bytes, 0, read);
            pos += read;
            // и обновить полоску прогресса
        }
    }



    // если длина почему-то известна
    public static async Task<byte[]> ReceiveBytesFixed (this TcpClient client, int length) {
        byte[] bytes = new byte[length];
        await client.GetStream ().ReadExactlyAsync (bytes);
        return bytes;
    }

    public static async Task<int> ReceiveInt32 (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesFixed (sizeof (int));  // зачёрпываем 4 байта
        return BitConverter.ToInt32 (bytes);
    }

    public static async Task<long> ReceiveInt64 (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesFixed (sizeof (long));  // зачёрпываем 8 байт
        return BitConverter.ToInt64 (bytes);
    }

    public static async Task<float> ReceiveSingle (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesFixed (sizeof (float));  // зачёрпываем 4 байта
        return BitConverter.ToSingle (bytes);
    }

    public static async Task<bool> ReceiveBoolean (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesFixed (sizeof (bool));  // зачёрпываем 1 байт
        return BitConverter.ToBoolean (bytes);
    }

    public static async Task<byte[]> ReceiveBytesVariable (this TcpClient client) {
        // сначала принять длину
        int length = await client.ReceiveInt32 ();
        // потом байты такой длины
        return await client.ReceiveBytesFixed (length);
    }

    public static async Task<string> ReceiveString (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesVariable ();
        return Encoding.UTF8.GetString (bytes);
    }

    public static async Task<T> ReceiveObjectAsJson<T> (this TcpClient client) {
        string json = await client.ReceiveString ();
        return JsonSerializer.Deserialize<T> (json)
            ?? throw new NullReferenceException ();
    }

    public static async Task<BinaryReader> ReceiveBinaryReader (this TcpClient client) {
        byte[] bytes = await client.ReceiveBytesVariable ();
        MemoryStream memory = new MemoryStream (bytes);
        BinaryReader reader = new BinaryReader (memory);
        return reader;
    }

    public static async Task ReceiveFileContent (this TcpClient client, Stream file) {
        long length = await client.ReceiveInt64 ();

        byte[] bytes = new byte[1024];
        long pos = 0;
        while (pos < length) {
            int read = await client.GetStream ().ReadAsync (
                bytes, 0, (int) Math.Min (bytes.Length, length - pos));
            await file.WriteAsync (bytes, 0, read);
            pos += read;
            // обновить полоску прогресса
        }
    }
}
