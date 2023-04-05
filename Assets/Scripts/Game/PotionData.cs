using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace Game
{
    public struct PotionData
    {
        public string Id;
        public int Count;
        
        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();
        [JsonIgnore] public Potion Bp => PotionManager.Instance.GetById(Id);



        #region 具体效果

        [Effect("hppotion", Timing.PotionEffect)]
        public void HpPotion(FighterData player)
        {
            player.Heal(new BattleStatus{CurHp = (int)Bp.Param1});
        }
        
        [Effect("mppotion", Timing.PotionEffect)]
        public void MpPotion(FighterData player)
        {
            player.Heal(new BattleStatus{CurMp = (int)Bp.Param1});
        }
                
        [Effect("angerpotion", Timing.PotionEffect)]
        public void AngerPotion(FighterData player)
        {
            player.AppliedBuff(new BuffData{CurLv = (int)Bp.Param1, Id = "anger"});
        }

        [Effect("firepotion", Timing.PotionEffect)]
        public void FirePotion(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, id: "firepotion");
                GameManager.Instance.PlayerData.Enemy.Settle(attack, player);
            }
        }
        
        [Effect("poisionpotion", Timing.PotionEffect)]
        public void PoisonPotion(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                GameManager.Instance.PlayerData.Enemy.AppliedBuff(new BuffData{CurLv = (int)Bp.Param1, Id = "poison"});
            }
        }
        
        [Effect("hppotion+", Timing.PotionEffect)]
        public void HpPotionP(FighterData player)
        {
            player.Heal(new BattleStatus{CurHp = (int)(player.Status.MaxHp*Bp.Param1)});
        }
        
                
        [Effect("mppotion+", Timing.PotionEffect)]
        public void MpPotionP(FighterData player)
        {
            player.Heal(new BattleStatus{CurMp = (int)(player.Status.MaxMp*Bp.Param1)});
        }
                
        [Effect("angerpotion+", Timing.PotionEffect)]
        public void AngerPotionP(FighterData player)
        {
            player.AppliedBuff(new BuffData{CurLv = (int)Bp.Param1, Id = "anger"});
        }
        
        [Effect("firepotion+", Timing.PotionEffect)]
        public void FirePotionP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, id: "firepotion");
                GameManager.Instance.PlayerData.Enemy.Settle(attack, player);
            }
        }
        
        
        [Effect("poisionpotion+", Timing.PotionEffect)]
        public void PoisonPotionP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                GameManager.Instance.PlayerData.Enemy.AppliedBuff(new BuffData{CurLv = (int)Bp.Param1, Id = "poison"});
            }
        }
                
        [Effect("hppotion++", Timing.PotionEffect)]
        public void HpPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus{MaxHp = (int)Bp.Param1});
        }
                
        [Effect("mppotion++", Timing.PotionEffect)]
        public void MpPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus{MaxMp = (int)Bp.Param1});
        }
        
        
        [Effect("firepotion++", Timing.PotionEffect)]
        public void FirePotionPP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, id: "firepotion");
                GameManager.Instance.PlayerData.Enemy.Settle(attack, player);
            }
        }
        
        
        [Effect("poisionpotion++", Timing.PotionEffect)]
        public void PoisonPotionPP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                GameManager.Instance.PlayerData.Enemy.AppliedBuff(new BuffData{CurLv = (int)Bp.Param1, Id = "poison"});
            }
        }
        
        
        [Effect("patkpotion", Timing.PotionEffect)]
        public void AngerPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus{PAtk = (int)Bp.Param1});
        }

        [Effect("matkpotion", Timing.PotionEffect)]
        public void MatkPotion(FighterData player)
        {
            player.Strengthen(new BattleStatus{MAtk = (int)Bp.Param1});
        }

        [Effect("mdefpotion", Timing.PotionEffect)]
        public void MDef(FighterData player)
        {
            player.Strengthen(new BattleStatus{MDef = (int)Bp.Param1});
        }
        
        [Effect("pdefpotion", Timing.PotionEffect)]
        public void PDef(FighterData player)
        {
            player.Strengthen(new BattleStatus{PDef = (int)Bp.Param1});
        }
        
        [Effect("fullpotion", Timing.PotionEffect)]
        public void FullPotion(FighterData player)
        {
            player.Heal(new BattleStatus
            {
                CurMp = 10000,
                CurHp = 10000
            });
        }

        [Effect("skillpotion", Timing.PotionEffect)]
        public void SkillPotion(FighterData player)
        {
            ((PlayerData) player).GetSkillPoint(Rank.Normal);
        }
        
        
        [Effect("skillpotion+", Timing.PotionEffect)]
        public void SkillPotionP(FighterData player)
        {
            ((PlayerData) player).GetSkillPoint(Rank.Uncommon);
        }
        
        [Effect("skillpotion++", Timing.PotionEffect)]
        public void SkillPotionPP(FighterData player)
        {
            ((PlayerData) player).GetSkillPoint(Rank.Rare);
        }
        
        
        #endregion
    }
}