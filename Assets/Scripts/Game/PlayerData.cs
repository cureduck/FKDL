using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    public class PlayerData : FighterData
    {
        public string Id;
        public PotionData[] Potions;
        
        public Dictionary<Rank, int> Keys;

        [JsonIgnore] public override FighterData Enemy => (EnemySaveData) GameManager.Instance.Focus.Data;

        public void March(string destination)
        {
            
        }


#if UNITY_EDITOR
        [Button]
#endif
        public void UsePotion(int index)
        {
            if (!Potions[index].IsEmpty)
            {
                if (Potions[index].Bp.Fs.TryGetValue(Timing.PotionEffect, out var f))
                {
                    CheckChain<PotionData>(Timing.OnUsePotion, new object[] {Potions[index], this});
                    f?.Invoke(Potions[index], new object[] {this});
                    Potions[index].Count -= 1;
                    if (Potions[index].Count <= 0)
                    {
                        Potions[index].Id = "";
                    }
                    PlaySoundEffect("potion");
                    Updated();
                }
            }
        }
        
        
        public bool TryTake(Offer offer)
        {
            switch (offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    return TryTakePotion(offer.Id);
                case Offer.OfferKind.Skill:
                    return TryTakeSkill(offer.Id);
                case Offer.OfferKind.Gold:
                    Gold += offer.Gold;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        [Button]
        public void AddSkillSlot()
        {
            Skills.Add(SkillData.Empty);
        }
        
        

        public bool TryTakePotion(string id)
        {
            if (PotionManager.Instance.Lib.TryGetValue(id, out var sk))
            {
                for (var i = 0; i < Potions.Length; i++)
                {
                    if ((Potions[i].Id == id))
                    {
                        Potions[i].Count += 1;
                        Updated();
                        return true;
                    }
                }
                
                for (int i = 0; i < Potions.Length; i++)
                {
                    if (Potions[i].IsEmpty)
                    {
                        Potions[i].Id = id;
                        Potions[i].Count = 1;
                        Updated();
                        return true;
                    }
                }
            }

            return false;
        }
        
        
        public bool TryTakeSkill(string id)
        {
            if (SkillManager.Instance.Lib.TryGetValue(id, out var sk))
            {
                for (int i = 0; i < Skills.Count; i++)
                {
                    if ((Skills[i].Id == id))
                    {
                        if (sk.MaxLv <= Skills[i].CurLv)
                        {
                            WindowManager.Instance.Warn("Skill Max!");
                            return false;
                        }
                        else
                        {
                            Skills[i].LvUp(this);
                            Updated();
                            return true;
                        }
                        
                    }
                }

                for (var i = 0; i < Skills.Count; i++)
                {
                    if (Skills[i].IsEmpty)
                    {
                        Skills[i].Id = id;
                        Skills[i].CurLv = 1;
                        Equip(Skills[i]);
                        Updated();
                        return true;
                    }
                }
            }
            return false;
        }


        
        
        public void Save()
        {
            Save(Paths._savePath);
        }

        public static PlayerData LoadFromInit()
        {
            return Load(Paths._initPath);
        }

        public static PlayerData LoadFromSave()
        {
            return Load(Paths._savePath);
        }
        
        
        [Button]
        public static PlayerData Load(string path)
        {
            return Load<PlayerData>(path);
        }

        public override string ToString()
        {
            return "player";
        }


        private static T Load<T>(string path)
        {
            string f = "";
#if UNITY_ANDROID
            var wread = new WWW(path);
            f = Encoding.UTF8.GetString(wread.bytes, 3, wread.bytes.Length - 3);
#else
            f = File.ReadAllText(path);
#endif
            return JsonConvert.DeserializeObject<T>(f, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        [Button]
        private void Save(string path)
        {
            var f = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(path, f);
        }
    }
}