using Reseau.Utils;
using System.Net.Sockets;

namespace Reseau.AutomateProtocol
{
    public class AutomateCommand
    {
        public Byte[] Question { get; set; }
        public Byte[] Response { get; set; }

        public async Task<Byte[]> ReadQuestion(NetworkStream stream, string serverId, bool print)
        {
            if (print)
            {
                return await TCPStreamUtils.ReadAsyncAndPrint(stream, serverId);
            }
            else
            {
                return await TCPStreamUtils.ReadAsync(stream);
            }
        }

        public async Task SendResponse(NetworkStream stream, string serverId, bool print)
        {
            if (print)
            {
                await TCPStreamUtils.WriteAsyncAndPrint(stream, Response, serverId);
            }
            else
            {
                await TCPStreamUtils.WriteAsync(stream, Response);
            }
        }

    }
}
