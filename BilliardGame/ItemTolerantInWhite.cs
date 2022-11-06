using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ItemTolerantInWhite : Item
    {
        private Player curPlayer;
        public Player CurPlayer
        {
            get { return curPlayer; }
            set { curPlayer = value; }
        }

        public ItemTolerantInWhite() { }

        public override void Action(GameState gameState, float initTime)
        {
            base.Action(gameState, initTime);

            gameState.CurrentPlayer.TolerantWhiteIn = 2;
            curPlayer = gameState.CurrentPlayer;
        }
        public override void Update(float timeSinceLastFrame)
        {
            textBox.SetCaption(curPlayer.TolerantWhiteIn.ToString());
        }
        public override void ActionEnd(GameState gameState)
        {
            gameState.CurrentPlayer.TolerantWhiteIn = 0;
            textBox.SetCaption(curPlayer.TolerantWhiteIn.ToString());
        }
    }
}
