using Connexion.Utils;
using System.Net.Sockets;

namespace Connexion.AutomateProtocol
{
    public class AutomateCommand
    {
        public Byte[] Question { get; set; }
        public Byte[] Response { get; set; }

        public async Task SendQuestion(NetworkStream stream, bool printConsole)
        {
            if (printConsole)
            {
                await TCPStreamUtils.WriteAsyncAndPrint(stream, Question);
            }
            else
            {
                await TCPStreamUtils.WriteAsync(stream, Question);
            }
        }

        public async Task<Byte[]> ReadResponse(NetworkStream stream, bool printConsole)
        {
            if (printConsole)
            {
                return await TCPStreamUtils.ReadAsyncAndPrint(stream);
            }
            else
            {
                return await TCPStreamUtils.ReadAsync(stream);
            }
        }

    }
}
