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

        public StaticText textBox;

        public Item() { }
        public virtual void Action(GameState gameState, float initTime) { }
        public virtual void Update(float timeSinceLastFrame) { }
        public virtual void ActionEnd(GameState gameState) { }
    }
}
