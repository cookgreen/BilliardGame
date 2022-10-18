using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilliardGame
{
    public static class MainGame
    {
        [STAThread]
        public static void Main()
        {
            OgreFrameworkApp app = new OgreFrameworkApp();
            app.Start();
        }
    }
}
