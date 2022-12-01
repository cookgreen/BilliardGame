using MyGUI.Sharp;

namespace BilliardGame
{
    public class Item
    {
        protected bool isPlaying;
        protected bool isEnd;
        protected float timeLeft;

        public bool IsPlaying { get { return isPlaying; } }
        public bool IsEnd { get { return isEnd; } }

        protected StaticText textBox;
        public StaticText TextBox 
        { 
            get { return textBox; } 
            set { textBox = value; }
        }

        public Item() { }
        public virtual void Action(BilliardGame gameState, float initTime) { }
        public virtual void Update(float timeSinceLastFrame) { }
        public virtual void ActionEnd(BilliardGame gameState) { }
    }
}
