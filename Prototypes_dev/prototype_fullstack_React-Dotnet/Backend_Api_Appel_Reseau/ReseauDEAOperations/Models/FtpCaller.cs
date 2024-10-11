using FluentFTP;

namespace ReseauDEAOperations.Models
{
    public class FtpCaller
    {
        public FtpClient Client { get; set; }

        public FtpCaller()
        {
            Client = new FtpClient();
            Client.Host = "localhost";
            Client.Port = 20;
        }
    }
}
