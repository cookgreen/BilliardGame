using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class Player
    {
        private int score;
        private int scalar;
        private int curScalar;
        private int hitTimes;
        private int gameLevel;
        private int tolerantWhiteIn;

        private bool isHoldTurn;
        private bool isCurHoldTurn;

        private string name;

        private Dictionary<EColorType, int> hitMap;
        private Dictionary<EColorType, int> inBallMap;

        public PlayerUI playerUI;

        public string Name
        {
            get { return name; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int Scalar
        {
            get { return scalar; }
            set { scalar = value; }
        }

        public int HitTimes
        {
            get { return hitTimes; }
            set { hitTimes = value; }
        }

        public bool IsHoldTurn
        {
            get { return isHoldTurn; }
            set { isHoldTurn = value; } 
        }

        public int TolerantWhiteIn
        {
            get { return tolerantWhiteIn; }
            set { tolerantWhiteIn = value; }
        }

        public Player(string name)
        {
            hitMap = new Dictionary<EColorType, int>();
            inBallMap = new Dictionary<EColorType, int>();

            this.name = name;

            score = 0;
            scalar = 1;
            curScalar = 1;
            hitTimes = 0;
            gameLevel = 0;
            isHoldTurn = false;
            isCurHoldTurn = false;
            tolerantWhiteIn = 0;
        }

        public void SetRoundScore(int score) { playerUI.SetRoundScore(score); }
        public void SetCurScalar(int scalar) { curScalar = scalar; }
        public void SetCurHoldTurn(bool isHoldTurn) { isCurHoldTurn = isHoldTurn; }
        public void SetGameLevel(int level) { this.gameLevel = level; }

        public void HitBall(EColorType color)
        {
            if(hitMap.ContainsKey(color))
            {
                hitMap[color] += 1;
            }
            else
            {
                hitMap.Add(color, 1);
            }
        }

        public void KickInBall(EColorType color)
        {
            if (inBallMap.ContainsKey(color))
            {
                inBallMap[color] += 1;
            }
            else
            {
                inBallMap.Add(color, 1);
            }
        }

        public void ClearHitballMap()
        {
            hitMap.Clear();
        }

        public void ClearInballMap()
        {
            inBallMap.Clear();
        }

        public void Reset()
        {
            ClearHitballMap();
            ClearInballMap();
            hitTimes = 0;
        }

        public void SetTurn(bool turn)
        {
            playerUI.SetTurn(turn);
        }

        public bool Judge(GameState gameState)
        {
            Console.WriteLine("Enter Judge");

            if (inBallMap.ContainsKey(EColorType.WHITE))
            {
                if (tolerantWhiteIn > 0)
                {
                    tolerantWhiteIn -= 1;
                    Reset();
                    return false;
                }
                Reset();
                Score = score += Ball.GetScoreOf(EColorType.WHITE);

                if (isHoldTurn)
                {
                    Player nextPlayer = gameState.NextPlayer;
                    nextPlayer.IsHoldTurn = true;
                }

                return true;
            }
            else
            {
                if (hitMap.Count == 0)
                {
                    Reset();
                    Score = Score - 1;

                    if (isCurHoldTurn)
                    {
                        return false;
                    }
                    else
                    {
                        if (tolerantWhiteIn > 0)
                        {
                            ItemManager.Instance.ClearActionItem(EColorType.BLACK, gameState);
                        }
                        return true;
                    }
                }
                else if (inBallMap.Count == 0)
                {
                    Reset();

                    if (isCurHoldTurn)
                    {
                        return false;
                    }
                    else
                    {
                        if (tolerantWhiteIn > 0)
                        {
                            ItemManager.Instance.ClearActionItem(EColorType.BLACK, gameState);
                        }
                        return true;
                    }
                }
            }

            if (Globals.Instance.NumPlayer == 1)
            {
                foreach (var kpl in inBallMap)
                {
                    score += 1;
                    Score = score;
                }
            }
            else
            {
                if (gameLevel == 0)
                {
                    foreach (var kpl in inBallMap)
                    {
                        if (kpl.Key == EColorType.RED)
                        {
                            int numRed = kpl.Value;
                            int scoreBase = Ball.GetScoreOf(EColorType.RED);
                            for (int i = 0; i < numRed; i++)
                            {
                                score += curScalar * scoreBase;
                                Score = score;
                                scoreBase *= 2;
                            }

                            if (gameState.GetNumRedBall() == 0)
                            {
                                gameState.SetGameLevel(1);
                            }
                        }
                        else if (kpl.Key == EColorType.GREEN)
                        {
                            ItemManager.Instance.Action(EColorType.GREEN, gameState, 60);
                        }
                        else if (kpl.Key == EColorType.BROWN)
                        {
                            ItemManager.Instance.Action(EColorType.BROWN, gameState, 60);
                        }
                        else if (kpl.Key == EColorType.BLUE)
                        {
                            ItemManager.Instance.Action(EColorType.BLUE, gameState, 60);
                        }
                        else if (kpl.Key == EColorType.BLACK)
                        {
                            ItemManager.Instance.Action(EColorType.BLACK, gameState, 600);
                        }
                    }
                }
                else if (gameLevel == 1)
                {
                    foreach(var kpl in inBallMap)
                    {
                        EColorType color = kpl.Key;

                        score += 2;
                        Score = score;

                        if (color == EColorType.BROWN)
                        {
                            ItemManager.Instance.Action(EColorType.BLUE, gameState, 60);
                        }
                    }
                }
            }

            hitTimes += 1;
            Reset();
            return false;
        }
    }
}
