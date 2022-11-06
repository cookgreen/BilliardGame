using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGUI.Sharp;

namespace BilliardGame
{
    public class ItemShowMsg : Item
    {
        private StaticImage imageBox;
        public StaticImage ImageBox
        {
            get { return imageBox; }
            set { imageBox = value; }
        }
        public override void Action(GameState gameState, float initTime)
        {
            base.Action(gameState, initTime);

            imageBox.SetVisible(true);
            textBox.SetVisible(true);

            string winnerName = gameState.GetWinner().Name;
            textBox.SetCaption(winnerName);
        }
        public override void Update(float timeSinceLastFrame)
        {
            timeLeft -= timeSinceLastFrame;
            if (timeLeft > 0)
            { 
            }
            else
            {
                isPlaying = false;
                isEnd = true;
            }
        }
        public override void ActionEnd(GameState gameState)
        {
            imageBox.SetVisible(false);
            textBox.SetVisible(false);
            gameState.SetGameOver(true) ;
        }
    }
}
