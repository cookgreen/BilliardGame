using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ItemHoldTurn : Item
    {
        public ItemHoldTurn() { }

        public override void Action(GameState gameState, float initTime)
        {
            base.Action(gameState, initTime);

            gameState.CurrentPlayer.IsHoldTurn = true;
        }
        public override void Update(float timeSinceLastFrame)
        {
            timeLeft -= timeSinceLastFrame;
            if (timeLeft > 0)
            {
                textBox.SetCaption(timeLeft.ToString());
            }
            else
            {
                isPlaying = false;
                isEnd = true;
            }
        }
        public override void ActionEnd(GameState gameState)
        {
            gameState.CurrentPlayer.IsHoldTurn = false;
        }
    }
}
