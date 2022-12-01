using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGUI.Sharp;

namespace BilliardGame
{
    public class ItemManager
    {
        private static ItemManager instance;
        public static ItemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemManager();
                }
                return instance;
            }
        }

        public List<Item> ActionItemList;
        public Dictionary<EColorType, Item> itemMap;

        public ItemManager()
        {
            ActionItemList = new List<Item>();
            itemMap = new Dictionary<EColorType, Item>();
        }

        public Item GetItemByColor(EColorType color)
        {
            if(itemMap.ContainsKey(color))
            {
                return itemMap[color];
            }
            else
            {
                return null;
            }
        }

        public void CreateItem(EColorType color, StaticText textBox)
        {
            if (GetItemByColor(color) != null)
            {
                return;
            }

            Item item = null;
            switch(color)
            {
                case EColorType.BROWN:
                    item = new ItemStopTime(); 
                    break;
                case EColorType.BLUE:
                    item = new ItemDoubleScore();
                    break;
                case EColorType.GREEN:
                    item = new ItemHoldTurn();
                    break;
                case EColorType.BLACK:
                    item = new ItemTolerantInWhite();
                    break;
                case EColorType.MSG:
                    item = new ItemShowMsg();
                    break;
                default:
                    item = null;
                    break;
            }

            if (item != null)
            {
                item.TextBox = textBox;
                itemMap.Add(color, item);
            }
        }

        public void PushItem(EColorType color, Item item)
        {
            itemMap.Add(color, item);
        }

        public void Action(EColorType color, BilliardGame gameState, int initTime)
        {
            Item item = GetItemByColor(color);
            if (item != null)
            {
                int idex = ActionItemList.IndexOf(item);
                if (idex == -1)
                {
                    item.Action(gameState, initTime);
                    ActionItemList.Add(item);
                }
            }
        }

        public void Update(float timeSinceLastFrame, BilliardGame gameState)
        {
            for (int i = ActionItemList.Count - 1; i > 0; i--)
            {
                if (!ActionItemList[i].IsEnd)
                {
                    ActionItemList[i].Update(timeSinceLastFrame);
                }
                else
                {
                    ActionItemList[i].ActionEnd(gameState);
                    ActionItemList.RemoveAt(i);
                }
            }
        }

        public void ClearActionItem(EColorType color, BilliardGame gameState)
        {
            Item item = GetItemByColor(color);
            int idx = ActionItemList.IndexOf(item);
            if (idx != -1)
            {
                ActionItemList.RemoveAt(idx);
            }
        }

        public void Reset()
        {
            itemMap.Clear();
            ActionItemList.Clear();
        }
    }
}
