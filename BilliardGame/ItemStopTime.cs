using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ItemStopTime : Item
    {
        public ItemStopTime() { }

        public override void Action(BilliardGame gameState, float initTime)
        {
            base.Action(gameState, initTime);
            gameState.FreezeTime(true);
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
        public override void ActionEnd(BilliardGame gameState)
        {
            gameState.FreezeTime(false);
        }
    }
}
