using System.Net.Sockets;

namespace Reseau.Utils
{
    public static class TCPStreamUtils
    {
        public static async Task<Byte[]> ReadAsyncAndPrint(NetworkStream stream, string serverId)
        {
            Byte[] question = new Byte[256];
            Int32 nbbytes = await stream.ReadAsync(question, 0, question.Length);
            Console.WriteLine($"\n[{serverId}] Received question : {BitConverter.ToString(question, 0, nbbytes).Replace("-", "")}");

            return question;
        }

        public static async Task<Byte[]> ReadAsync(NetworkStream stream)
        {
            Byte[] question = new Byte[256];
            await stream.ReadAsync(question, 0, question.Length);

            return question;
        }

        public static async Task WriteAsyncAndPrint(NetworkStream stream, Byte[] response, string serverId)
        {
            await stream.WriteAsync(response);
            await stream.FlushAsync();
            Console.WriteLine($"[{serverId}] Sending Response : {BitConverter.ToString(response).Replace("-", "")}");
        }

        public static async Task WriteAsync(NetworkStream stream, Byte[] response)
        {
            await stream.WriteAsync(response);
            await stream.FlushAsync();
        }
    }
}
