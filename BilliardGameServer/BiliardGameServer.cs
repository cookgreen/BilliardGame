using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BilliardGameServer
{
    public class GameServer
    {
        private bool isQuit;
        private TcpListener listener; 
        private List<GameClient> clients;

        public GameServer(uint port)
        {
            isQuit = false;
            listener = new TcpListener(IPAddress.Any, (int)port);
            clients = new List<GameClient>();
        }

        public void Go()
        {
            while (!isQuit)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                GameClient gameClient = new GameClient(tcpClient);
                Console.WriteLine(string.Format("Client[{0}] has connected!", gameClient.ToString()));
                gameClient.Disconnect += () =>
                {
                    Console.WriteLine(string.Format("Client[{0}] has disconnected!", gameClient.ToString()));
                    clients.Remove(gameClient);
                };
                clients.Add(gameClient);
            }
        }
    }
}
