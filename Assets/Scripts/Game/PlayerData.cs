using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class PlayerData : SaveData
    {
        public string Id;
        public SkillData[] Skills;
        public PotionData[] Potions;

        public int Gold;
        
        public BattleStatus BattleStatus;
        public PlayerStatus PlayerStatus;

        private static readonly string _initPath = Path.Combine( Application.persistentDataPath, "Resources", "PlayerInit", "PlayerData.json");
        private static readonly string _savePath = Path.Combine( Application.persistentDataPath, "PlayerData.json");




        public bool TryTake(Offer offer)
        {
            switch (offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    return TryTakePotion(offer.Id);
                    break;
                case Offer.OfferKind.Skill:
                    return TryTakeSkill(offer.Id);
                    break;
                case Offer.OfferKind.Gold:
                    Gold += offer.Gold;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private bool TryTakePotion(string id)
        {
            if (PotionManager.Instance.Potions.TryGetValue(id, out var sk))
            {
                for (var i = 0; i < Potions.Length; i++)
                {
                    if ((Potions[i].Id == id))
                    {
                        Potions[i].Count += 1;
                        return true;
                    }
                }

                for (int i = 0; i < Potions.Length; i++)
                {
                    if (Potions[i].IsEmpty)
                    {
                        Potions[i].Id = id;
                        Potions[i].Count = 1;
                        return true;
                    }
                }
            }

            return false;
        }
        
        
        private bool TryTakeSkill(string id)
        {
            if (SkillManager.Instance.Skills.TryGetValue(id, out var sk))
            {
                for (var i = 0; i < Skills.Length; i++)
                {
                    if (Skills[i].IsEmpty)
                    {
                        Skills[i].Id = id;
                        Skills[i].CurLv = 1;
                        return true;
                    }
                }

                for (int i = 0; i < Skills.Length; i++)
                {
                    if ((Skills[i].Id == id)&&(sk.MaxLv > Skills[i].CurLv))
                    {
                        Skills[i].CurLv += 1;
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        
        public void Save()
        {
            Save(_savePath);
        }

        public static PlayerData LoadFromInit()
        {
            return Load(_initPath);
        }

        public static PlayerData LoadFromSave()
        {
            return Load(_savePath);
        }
        
        
        [Button]
        public static PlayerData Load(string path)
        {
            return Load<PlayerData>(path);
        }
    }
}