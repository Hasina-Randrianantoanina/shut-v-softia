using System.Net.Sockets;

namespace TestConnexion.Utils
{
    public static class TCPStreamUtils
    {
        public static async Task<Byte[]> ReadAsyncAndPrint(NetworkStream stream)
        {
            try {
                Byte[] response = new Byte[256];
                Int32 nbbytes = await stream.ReadAsync(response, 0, response.Length);
                Console.WriteLine($"Response received: {BitConverter.ToString(response, 0, nbbytes).Replace("-", "")}");

                return response;
            }
            catch
            {
                Console.WriteLine("\n--- Error when reading the stream TCP");
                throw;
            }
        }

        public static async Task<Byte[]> ReadAsync(NetworkStream stream)
        {
            try
            {
                Byte[] response = new Byte[256];
                await stream.ReadAsync(response, 0, response.Length);
                return response;
            }
            catch
            {
                Console.WriteLine("\n--- Error when reading the stream TCP");
                throw;
            }
        }

        public static async Task WriteAsyncAndPrint(NetworkStream stream, Byte[] question)
        {
            try
            {
                await stream.WriteAsync(question);
                await stream.FlushAsync();
                Console.WriteLine($"\nQuestion send: {BitConverter.ToString(question).Replace("-", "")}");
            }
            catch
            {
                Console.WriteLine("\n--- Error when writing into the stream TCP");
                throw;
            }
        }

        public static async Task WriteAsync(NetworkStream stream, Byte[] question)
        {
            try
            {
                await stream.WriteAsync(question);
                await stream.FlushAsync();
            }
            catch
            {
                Console.WriteLine("\n--- Error when writing into the stream TCP");
                throw;
            }

        }
    }
}
