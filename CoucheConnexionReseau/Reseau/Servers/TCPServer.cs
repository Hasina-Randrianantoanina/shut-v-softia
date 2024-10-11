using Reseau.AutomateProtocol;
using Reseau.Entities;
using System.Net;
using System.Net.Sockets;

namespace Reseau.Servers
{
    public class TCPServer
    {
        private readonly string _serverId;
        private Station _station;
        private readonly int _port;
        private TcpListener _listener;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        public TCPServer(string serverId, Station station, int port)
        {
            _serverId = serverId;
            _station = station;
            _port = port;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Parse(_station.AdresseIP), _port);
            _listener.Start();
            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine($"{_serverId} started on {_station.AdresseIP}:{_port}");

            Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _listener.Stop();
            Console.WriteLine($"{_serverId} stopped.");
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    // Connexion to server successfull
                    Console.WriteLine($"\n[{this._serverId}] Incoming connexion");

                    _ = HandleClientAsync(client, cancellationToken);
                }
                catch (ObjectDisposedException)
                {
                    // Listener has been stopped
                    Console.WriteLine($"\n[{this._serverId}] Listener has been stopped");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using NetworkStream stream = client.GetStream();

            Byte[] endingArray = new Byte[256]; // array of 0

            IAutomateProtocol automateProtocol;

            switch (_station.Initiales)
            {
                case "BP": // Premium "" RU
                    automateProtocol = new BPProtocol();
                    break;
                case "EN": // Premium D13 RU
                    automateProtocol = new ENProtocol();
                    break;
                case "GA": // M580 D15 RU
                    automateProtocol = new GAProtocol();
                    break;
                case "HE": // M580 D16 RU
                    automateProtocol = new HEProtocol();
                    break;
                case "RC": // Premium "" RU with Defauts
                    automateProtocol = new RCProtocol();
                    break;
                case "172": // M580 D15 RO
                    automateProtocol = new _172Protocol();
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    automateProtocol = new _178Protocol();
                    break;
                default:
                    automateProtocol = null;
                    break;
            }

            while (_isRunning && !cancellationToken.IsCancellationRequested && automateProtocol != null)
            {
                // Waiting for a command
                AutomateCommand automateCommand = new AutomateCommand();

                automateCommand.Question = await automateCommand.ReadQuestion(stream, _serverId, true);

                if (automateCommand.Question.SequenceEqual(endingArray))
                {
                    Console.WriteLine("\nBreaking signal received - end of discussion");
                    break;
                }

                // Creating the response
                automateCommand.Response = automateProtocol.CalculResponse(BitConverter.ToString(automateCommand.Question).Replace("-", ""));

                if (automateCommand.Response == null)
                {
                    Console.WriteLine("\nUnknow question - end of dicussion");
                    break;
                }

                // Response
                await automateCommand.SendResponse(stream, _serverId, true);

            }

        }

    }
}
