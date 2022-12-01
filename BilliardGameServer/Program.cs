using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGameServer
{
    public class Program
    {
        public static void Main()
        {
            GameServer gameServer = new GameServer(6690);
            gameServer.Go();
        }
    }
}
