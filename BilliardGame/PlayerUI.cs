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
        private StaticText txtName;
        private StaticImage imageRed;
        private StaticImage imageGreen;
        private StaticText txtScores;
        private StaticText txtRoundScores;

        public StaticText TxtName
        {
            get { return txtName; }
            set { txtName = value; }
        }
        public StaticImage ImageRed
        {
            get { return imageRed; }
            set { imageRed = value; }
        }
        public StaticImage ImageGreen
        {
            get { return imageGreen; }
            set { imageGreen = value; }
        }
        public StaticText TxtScores
        {
            get { return txtScores; }
            set { txtScores = value; }
        }
        public StaticText TxtRoundScores
        {
            get { return txtRoundScores; }
            set { txtRoundScores = value; }
        }

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
