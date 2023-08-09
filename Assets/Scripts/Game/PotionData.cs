using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace Game
{
    public class PotionData : BpData<Potion>
    {
        public int Count;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();
        [JsonIgnore] public override Potion Bp => PotionManager.Instance.GetById(Id);


        [JsonIgnore] public bool CanBeUpgrade => Bp != null && Bp.Upgrade.IsNullOrWhitespace();

        public void SetEmpty()
        {
            Id = "";
        }

        public void SetPotion(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }

        public bool Upgrade(out Info info)
        {
            if (CanBeUpgrade)
            {
                Id = Bp.Upgrade;
                info = new SuccessInfo();
                return true;
            }
            else
            {
                info = new FailureInfo(FailureReason.PotionCannotUpgrade);
                return false;
            }
        }

        #region 具体效果

        [Effect("hppotion", Timing.PotionEffect)]
        private void HpPotion(FighterData player)
        {
            player.Heal(new BattleStatus { CurHp = (int)Bp.Param1 });
        }

        [Effect("mppotion", Timing.PotionEffect)]
        private void MpPotion(FighterData player)
        {
            player.Heal(new BattleStatus { CurMp = (int)Bp.Param1 });
        }

        [Effect("angerpotion", Timing.PotionEffect)]
        private void AngerPotion(FighterData player)
        {
            player.AppliedBuff(BuffData.PPlus((int)Bp.Param1));
        }

        [Effect("firepotion", Timing.PotionEffect)]
        private void FirePotion(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, kw: "firepotion");
                GameManager.Instance.Player.Enemy.SingleDefendSettle(attack, player);
            }
            else
            {
                player.ApplySelfBuff(BuffData.Flaming((int)Bp.Param2));
            }
        }

        [Effect("poisonpotion", Timing.PotionEffect)]
        private void PoisonPotion(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var buff = player.ApplyBuff(BuffData.Poison((int)Bp.Param1));
                player.Enemy.AppliedBuff(buff);
            }
            else
            {
                ((PlayerData)player).RemoveBuff("poison", (int)Bp.Param1);
            }
        }

        [Effect("hppotion+", Timing.PotionEffect)]
        private void HpPotionP(FighterData player)
        {
            player.Heal(new BattleStatus { CurHp = (int)(player.Status.MaxHp * Bp.Param1 / 100) });
        }


        [Effect("mppotion+", Timing.PotionEffect)]
        private void MpPotionP(FighterData player)
        {
            player.Heal(new BattleStatus { CurMp = (int)(player.Status.MaxMp * Bp.Param1 / 100) });
        }

        [Effect("angerpotion+", Timing.PotionEffect)]
        private void AngerPotionP(FighterData player)
        {
            player.AppliedBuff(BuffData.PPlus((int)Bp.Param1));
        }

        [Effect("firepotion+", Timing.PotionEffect)]
        private void FirePotionP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, kw: "firepotion");
                player.Enemy.SingleDefendSettle(attack, player);
            }
            else
            {
                player.ApplySelfBuff(BuffData.Flaming((int)Bp.Param2));
            }
        }


        [Effect("poisonpotion+", Timing.PotionEffect)]
        private void PoisonPotionP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                player.Enemy.AppliedBuff(BuffData.Poison((int)Bp.Param1));
            }
            else
            {
                player.RemoveBuff("poison", (int)Bp.Param1);
            }
        }

        [Effect("hppotion++", Timing.PotionEffect)]
        private void HpPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus { MaxHp = (int)Bp.Param1 });
        }

        [Effect("mppotion++", Timing.PotionEffect)]
        private void MpPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus { MaxMp = (int)Bp.Param1 });
        }


        [Effect("firepotion++", Timing.PotionEffect)]
        private void FirePotionPP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                var attack = new Attack(mAtk: (int)Bp.Param1, kw: "firepotion");
                player.Enemy.SingleDefendSettle(attack, player);
            }
            else
            {
                player.ApplySelfBuff(BuffData.Flaming((int)Bp.Param2));
            }
        }


        [Effect("poisonpotion++", Timing.PotionEffect)]
        private void PoisonPotionPP(FighterData player)
        {
            if (GameManager.Instance.InBattle)
            {
                player.Enemy.AppliedBuff(BuffData.Poison((int)Bp.Param1));
            }
            else
            {
                player.RemoveBuff("poison", (int)Bp.Param1);
            }
        }


        [Effect("patkpotion", Timing.PotionEffect)]
        private void AngerPotionPP(FighterData player)
        {
            player.Strengthen(new BattleStatus { PAtk = (int)Bp.Param1 });
        }

        [Effect("matkpotion", Timing.PotionEffect)]
        private void MatkPotion(FighterData player)
        {
            player.Strengthen(new BattleStatus { MAtk = (int)Bp.Param1 });
        }

        [Effect("mdefpotion", Timing.PotionEffect)]
        private void MDef(FighterData player)
        {
            player.Strengthen(new BattleStatus { MDef = (int)Bp.Param1 });
        }

        [Effect("pdefpotion", Timing.PotionEffect)]
        private void PDef(FighterData player)
        {
            player.Strengthen(new BattleStatus { PDef = (int)Bp.Param1 });
        }

        [Effect("fullpotion", Timing.PotionEffect)]
        private void FullPotion(FighterData player)
        {
            player.Heal(new BattleStatus
            {
                CurMp = 10000,
                CurHp = 10000
            });
        }

        [Effect("skillpotion", Timing.PotionEffect)]
        private void SkillPotion(FighterData player)
        {
            ((PlayerData)player).GetSkillPoint();
        }


        [Effect("skillpotion+", Timing.PotionEffect)]
        private void SkillPotionP(FighterData player)
        {
            ((PlayerData)player).GetSkillPoint();
        }

        [Effect("skillpotion++", Timing.PotionEffect)]
        private void SkillPotionPP(FighterData player)
        {
            ((PlayerData)player).GetBreakoutPoint(1);
        }


        [Effect("slotpotion", Timing.PotionEffect)]
        private void SlotPotion(FighterData player)
        {
            if (player is PlayerData p)
            {
                p.AddSkillSlot();
                p.Weaken(new BattleStatus(maxHp: (int)Bp.Param1, maxMp: (int)Bp.Param2));
            }
        }

        #endregion
    }
}