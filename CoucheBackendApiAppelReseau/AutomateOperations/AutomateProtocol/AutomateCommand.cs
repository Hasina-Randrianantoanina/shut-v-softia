using System.Net.Sockets;
using AutomateOperations.Utils;

namespace AutomateOperations.AutomateProtocol
{
    public class AutomateCommand
    {
        public Byte[] Question { get; set; }
        public Byte[] Response { get; set; }

        public async Task<(bool statusWriting, Erreur? erreur)> SendQuestion(NetworkStream stream)
        {
                return await TCPStreamUtils.WriteAsync(stream, Question);
        }

        public async Task<(Byte[]? response, Erreur? erreur)> ReadResponse(NetworkStream stream)
        {
                return await TCPStreamUtils.ReadAsync(stream);
        }

    }
}
