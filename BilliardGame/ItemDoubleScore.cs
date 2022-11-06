using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ItemDoubleScore : Item
    {
        public ItemDoubleScore() { }

        public override void Action(GameState gameState, float initTime)
        {
            base.Action(gameState, initTime);

            List<Player> playerList = gameState.Players;
            foreach(var player in playerList)
            {
                player.Scalar = 2;
            }
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
            List<Player> playerList = gameState.Players;
            foreach (var player in playerList)
            {
                player.Scalar = 1;
            }
        }
    }
}
