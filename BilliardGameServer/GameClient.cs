using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BilliardGameServer
{
    public class GameClient
    {
        private string playerName;

        private TcpClient tcpClient;
        private Thread recvDataThread;
        private Thread recvDataProgressThread;
        private Queue<byte[]> bufferProgressQueue;

        public event Action Disconnect;
        public GameClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;

            bufferProgressQueue = new Queue<byte[]>();

            recvDataThread = new Thread(clientRecvData);
            recvDataThread.Start();

            recvDataProgressThread = new Thread(clientRecvDataProgress);
            recvDataProgressThread.Start();
        }

        private void clientRecvData()
        {
            try
            {
                while (true)
                {
                    List<byte> bufferList = new List<byte>();
                    int recvSize = tcpClient.Client.Receive(bufferList.ToArray());
                    byte[] buffer = new byte[recvSize];
                    for (int i = 0; i < recvSize; i++)
                    {
                        buffer[i] = bufferList[i];
                    }
                    bufferProgressQueue.Enqueue(buffer);
                }
            }
            catch
            {
                if (Disconnect != null) { Disconnect(); }
            }
        }

        private void clientRecvDataProgress()
        {
            while (true)
            {
                if (bufferProgressQueue.Count > 0)
                {
                    byte[] buffer = bufferProgressQueue.Dequeue();
                    //Progress code
                    byte flag = buffer[0];
                    GameServerMessageType messageType = (GameServerMessageType)flag;
                    if (messageType == GameServerMessageType.SERVER_MSG_SET_PALYER_NAME)
                    {
                        byte[] newStrBuffer = new byte[buffer.Length - 1];
                        for (int i = 1; i < buffer.Length; i++)
                        {
                            newStrBuffer[i - 1] = buffer[i];
                        }
                        playerName = Encoding.UTF8.GetString(newStrBuffer);
                    }
                    else if (messageType == GameServerMessageType.SERVER_MSG_GET_PALYER_NAME)
                    {
                        byte[] newStrBuffer = Encoding.UTF8.GetBytes(playerName);
                        tcpClient.Client.Send(newStrBuffer);
                    }
                }
            }
        }

        public string IPAddress
        {
            get { return ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address.ToString(); }
        }

        public int Port
        {
            get { return ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port; }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", IPAddress, Port);
        }
    }
}
