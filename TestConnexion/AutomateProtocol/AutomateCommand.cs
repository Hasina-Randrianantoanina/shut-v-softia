using System.Net.Sockets;
using TestConnexion.Utils;

namespace TestConnexion.AutomateProtocol
{
    public class AutomateCommand
    {
        public Byte[] Question { get; set; }
        public Byte[] Response { get; set; }

        public async Task SendQuestion(NetworkStream stream, bool printConsole)
        {
            try
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
            catch
            {
                throw;
            }

        }

        public async Task<Byte[]> ReadResponse(NetworkStream stream, bool printConsole)
        {
            try
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
            catch
            {
                throw;
            }

        }

    }
}
