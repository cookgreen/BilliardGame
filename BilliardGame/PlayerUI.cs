using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGUI.Sharp;

namespace BilliardGame
{
    public class PlayerUI
    {
        public StaticText txtName;
        public StaticImage imageRed;
        public StaticImage imageGreen;
        public StaticText txtScores;
        public StaticText txtRoundScores;

        public PlayerUI() { }

        public void SetPlayerName(string name)
        {
            txtName.SetCaption(name);
        }

        public void SetTurn(bool isMyTurn) 
        {
            if (isMyTurn)
            {
                imageGreen.SetVisible(true);
                imageRed.SetVisible(false);
            }
            else
            {
                imageGreen.SetVisible(false);
                imageRed.SetVisible(true);
            }
        }

        public void SetScore(int score)
        {
            txtScores.SetCaption(score.ToString());
        }

        public void SetRoundScore(int score)
        {
            txtRoundScores.SetCaption(score.ToString());
        }
    }
}
