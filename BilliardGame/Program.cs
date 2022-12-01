using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BilliardGame
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            OgreFrameworkApp app = new OgreFrameworkApp();
            app.Start();
        }
    }
}
