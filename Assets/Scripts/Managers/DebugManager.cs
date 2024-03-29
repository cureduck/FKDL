﻿using Game;
using Sirenix.OdinInspector;
using TMPro;
using UI;

namespace Managers
{
    public class DebugManager : Singleton<DebugManager>
    {
        public TMP_InputField PotionDebugInput;
        public TMP_InputField BuffDebugInput;
        public TMP_InputField RelicDebugInput;
        public TMP_InputField SKillDebugInput;
        public TMP_InputField CrystalInput;

        private static PlayerData Player => GameDataManager.Instance.PlayerData;

        [Button]
        public void Apply(string id, int lv)
        {
            GameManager.Instance.Player.AppliedBuff(
                new BuffData(id, lv));
        }


        [Button]
        public void ShowAll()
        {
            foreach (var mapData in GameManager.Instance.Map.Floors[GameManager.Instance.Map.CurrentFloor].Squares)
            {
                if (mapData.SquareState == SquareState.UnRevealed)
                {
                    mapData.Reveal();
                }
            }
        }


        [Button]
        public void CreateCrystal()
        {
            CreateCrystal(CrystalInput.text);
        }

        public void CreateCrystal(string id)
        {
            if (CrystalManager.Instance.Lib.TryGetValue(id, out var crystal))
            {
                WindowManager.Instance.CrystalPanel.Open(
                    (GameManager.Instance.Player, crystal, "UI_MagicCrystal_Title")
                );
            }
        }


        [Button]
        public void AddPotion(string id)
        {
            GameManager.Instance.Player.TryTakeOffer(new Offer()
            {
                Id = id,
                Kind = Offer.OfferKind.Potion
            }, out _);
        }

        [Button]
        public void AddBuff(string id)
        {
            GameManager.Instance.Player.ApplySelfBuff(new BuffData(id, 1));
        }

        [Button]
        public void AddRelic(string id)
        {
            if (RelicManager.Instance.TryGetById(id, out var relic))
            {
                GameManager.Instance.Player.TryTakeOffer(new Offer(relic), out _);
            }
        }

        public void AddRelic()
        {
            AddRelic(RelicDebugInput.text);
        }


        public void AddSkill(string id)
        {
            if (SkillManager.Instance.TryGetById(id, out var skill))
            {
                GameManager.Instance.Player.TryTakeOffer(new Offer(skill), out _);
            }
        }

        public void AddSkill()
        {
            AddSkill(SKillDebugInput.text);
        }

        public void RollForRelic(int rank)
        {
            GameManager.Instance.RollForRelic((Rank)rank);
        }


        public void RollForPotion()
        {
        }


        [Button]
        public void AddBuff()
        {
            AddBuff(BuffDebugInput.text);
        }


        public void AddPotion()
        {
            GameManager.Instance.Player.TryTakeOffer(new Offer()
            {
                Id = PotionDebugInput.text,
                Kind = Offer.OfferKind.Potion
            }, out _);
        }

        public void Add100Gold()
        {
            GameManager.Instance.Player.Gain(100);
        }

        public void ResetStatus()
        {
            Player.Heal(BattleStatus.Hp(Player.Status.MaxHp));
            Player.Heal(BattleStatus.Mp(Player.Status.MaxMp));
        }

        public void Add1Atk()
        {
            GameManager.Instance.Player.Strengthen(new BattleStatus { PAtk = 100000 });
            PlayerMainPanel.Instance.PlayGetCharacterPointEffect(0);
        }

        public void Add1Def()
        {
            GameManager.Instance.Player.Strengthen(new BattleStatus { PDef = 1 });
            PlayerMainPanel.Instance.PlayGetCharacterPointEffect(2);
        }
    }
}