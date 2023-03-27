using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Game
{
    public class MountainSaveData : MapData
    {
        public const int MaxTimes = 3;
        
        public int TimesLeft;
        public Rank Rank;
        public int Cost;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            {{Rank.Normal, 3}, {Rank.Uncommon, 6}, {Rank.Rare, 9}, {Rank.Ultra, 12}};

        public override void Init()
        {
            base.Init();
            Rank = _rank;
            TimesLeft = MaxTimes;
            Cost = _cost[Rank];
        }


        public override void OnReact()
        {
            base.OnReact();
            if (GameManager.Instance.PlayerData.CurHp >= Cost)
            {
                GameManager.Instance.PlayerData.Cost(new BattleStatus{CurMp = Cost});
                if (Random.Range(0f, 1f)> .5f)
                {
                    PlaySoundEffect("casino_win");
                    GameManager.Instance.RollForSkill(Rank);
                }
                else
                {
                    PlaySoundEffect("casino_lose");
                    WindowManager.Instance.Warn("You Lose");
                }
                
                TimesLeft -= 1;
                if (TimesLeft <= 0)
                {
                    Destroyed();
                }
                Updated();
            }
            else
            {
                WindowManager.Instance.Warn("Not Enough Sp");
            }
        }
    }
}