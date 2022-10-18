using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ItemTolerantInWhite : Item
    {
        public Player curPlayer;

        public ItemTolerantInWhite() { }

        public override void Action(GameState gameState, float initTime)
        {
            base.Action(gameState, initTime);

            gameState.CurrentPlayer.SetTolerantWhiteIn(2);
            curPlayer = gameState.CurrentPlayer;
        }
        public override void Update(float timeSinceLastFrame)
        {
            textBox.SetCaption(curPlayer.GetTolerantWhiteIn().ToString());
        }
        public override void ActionEnd(GameState gameState)
        {
            gameState.CurrentPlayer.SetTolerantWhiteIn(0);
            textBox.SetCaption(curPlayer.GetTolerantWhiteIn().ToString());
        }
    }
}
