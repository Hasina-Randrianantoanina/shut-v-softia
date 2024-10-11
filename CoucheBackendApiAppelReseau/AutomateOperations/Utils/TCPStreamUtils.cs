using System.Net.Sockets;

namespace AutomateOperations.Utils
{
    public static class TCPStreamUtils
    {
        public static async Task<(Byte[]? response, Erreur? erreur)> ReadAsync(NetworkStream stream)
        {
            try
            {
                Byte[] response = new Byte[256];
                await stream.ReadAsync(response, 0, response.Length);
                return (response, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleTCPError(201, ex));
            }
        }

        public static async Task<(bool statusWriting, Erreur? erreur)> WriteAsync(NetworkStream stream, Byte[] question)
        {
            try
            {
                await stream.WriteAsync(question);
                await stream.FlushAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleTCPError(202, ex));
            }

        }
    }
}
