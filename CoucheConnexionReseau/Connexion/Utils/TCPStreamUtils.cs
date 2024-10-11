using System.Net.Sockets;

namespace Connexion.Utils
{
    public static class TCPStreamUtils
    {
        public static async Task<Byte[]> ReadAsyncAndPrint(NetworkStream stream)
        {
            Byte[] response = new Byte[256];
            Int32 nbbytes = await stream.ReadAsync(response, 0, response.Length);
            Console.WriteLine($"Response received: {BitConverter.ToString(response, 0, nbbytes).Replace("-", "")}");

            return response;
        }

        public static async Task<Byte[]> ReadAsync(NetworkStream stream)
        {
            Byte[] response = new Byte[256];
            await stream.ReadAsync(response, 0, response.Length);
            return response;
        }

        public static async Task WriteAsyncAndPrint(NetworkStream stream, Byte[] question)
        {
            await stream.WriteAsync(question);
            await stream.FlushAsync();
            Console.WriteLine($"\nQuestion send: {BitConverter.ToString(question).Replace("-", "")}");
        }

        public static async Task WriteAsync(NetworkStream stream, Byte[] question)
        {
            await stream.WriteAsync(question);
            await stream.FlushAsync();
        }
    }
}
