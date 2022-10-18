using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class Globals
    {
        private static Globals instance;
        public static Globals Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Globals();
                }
                return instance;
            }
        }

        public int TotalRound;
        public int CurRound;
        public int NumPlayer;
        public List<int> RoundScore;

        public Globals()
        {
            RoundScore = new List<int>();
        }
    }
}
