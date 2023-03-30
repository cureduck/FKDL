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
using UI;
using UnityEngine;
using Object = System.Object;

namespace Game
{
    public class PlayerData : FighterData
    {
        public string Id;
        public PotionData[] Potions;
        public RelicAgent Relics;
        

        public bool Engaging;
        public Dictionary<Rank, int> Keys;

        [JsonIgnore] public override FighterData Enemy => (EnemySaveData) GameManager.Instance.Focus.Data;

        public void March(string destination)
        {
            Debug.Log($"destination {destination}");
            CheckChain(Timing.OnMarch, new object[] {this});
        }


        public PlayerData()
        {
            Relics = new RelicAgent();
            
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
                    Gain(offer.Gold);
                    return true;
                case Offer.OfferKind.Relic:
                    return TryTakeRelic(offer.Id);
                
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


        public bool TryTakeRelic(string id)
        {
            if (RelicManager.Instance.Lib.TryGetValue(id, out var sk))
            {
                throw new NotImplementedException();
            }
            return false;
        }
        
        
        
        public bool TryTakeSkill(string id)
        {
            if (SkillManager.Instance.Lib.TryGetValue(id, out var sk))
            {
                for (int i = 0; i < Skills.Count; i++)
                {
                    if (Skills[i] != null && (Skills[i].Id == id))
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
                    if (Skills[i] == null)
                    {
                        Skills[i] = new SkillData();
                    }
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



        public override void UseSkill(SkillData skill)
        {
            if (GameManager.Instance.InBattle)
            {
                ((EnemySaveData)(Enemy)).OnReact(skill);
            }
            else
            {
                CastNonAimingSkill(skill);
            }
        }
        
        
        public override bool TryUseSkill(SkillData skill, out string info)
        {
            info = "";
            if (!CanCast(skill)) return false;
            UseSkill(skill);
            return true;

        }

        public bool TryUseSkill(int index)
        {
            var skill = Skills[index];
            if (skill == null || skill.IsEmpty) return false;
            return TryUseSkill(skill, out _);
        }


        public event Action OnSkillPointChanged;
        
        public bool CanUpgrade(SkillData skillData)
        {
            return false;
        }

        public void UpgradeWithPoint(SkillData skillData)
        {
            skillData.CurLv += 1;
            Updated();
            OnSkillPointChanged?.Invoke();
        }

        public void GetSkillPoint(Rank rank, int v = 1)
        {
            if (GameDataManager.Instance.SecondaryData.SkillPoint.ContainsKey(rank))
            {
                GameDataManager.Instance.SecondaryData.SkillPoint[rank] += v;
            }
            else
            {
                GameDataManager.Instance.SecondaryData.SkillPoint[rank] = v;
            }
            OnSkillPointChanged?.Invoke();
        }
        
        
        
        
        public void Execute(string cmds)
        {
            foreach (var cmd in cmds.Replace(" ","").Split('|'))
            {

                try
                {
                    var prefix = cmd.Split(':')[0];
                    switch (prefix)
                    {
                        case "buff":
                            var kind = cmd.Split(':')[1];
                            var stack = cmd.Split(':')[2];
                            AppliedBuff(new BuffData(kind, int.Parse(stack)));
                            break;
                        case "gold":
                            var count = int.Parse(cmd.Split(':')[1]);
                            Gain(count);
                            break;
                        case "skill":
                            var ess = cmd.Split(':');
                            var r = int.Parse(ess[1]);
                            GameManager.Instance.RollForSkill(r);
                            break;
                        case "relic":
                            break;
                        case "attr":
                            var type = cmd.Split(':')[1].ToLower();
                            var count1 = int.Parse(cmd.Split(':')[2]);
                            switch (type)
                            {
                                case "curhp":
                                    if (count1 > 0)
                                    {
                                        Heal(BattleStatus.HP(count1));
                                    }
                                    else
                                    {
                                        Cost(new CostInfo{Value = count1, CostType = CostType.Hp});
                                    }
                                    break;
                                case "curmp":
                                    if (count1 > 0)
                                    {
                                        Heal(BattleStatus.Mp(count1));
                                    }
                                    else
                                    {
                                        Cost(new CostInfo{Value = count1, CostType = CostType.Mp});
                                    }
                                    break;
                                case "maxhp":
                                    Strengthen(new BattleStatus{MaxHp = count1});
                                    break;
                                case "maxmp":
                                    Strengthen(new BattleStatus{MaxHp = count1});
                                    break;
                                case "matk":
                                    Strengthen(new BattleStatus{MAtk = count1});
                                    break;
                                case "patk":
                                    Strengthen(new BattleStatus{PAtk = count1});
                                    break;
                                case "mdef":
                                    Strengthen(new BattleStatus{MDef = count1});
                                    break;
                                case "pdef":
                                    Strengthen(new BattleStatus{PDef = count1});
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "jump":
                            break;
                        case "potion":
                            var es = cmd.Split(':');
                            var id = es[1];
                            var c = es.Length > 1 ? int.Parse(es[2]) : 1;
                            for (int i = 0; i < c; i++)
                            {
                                TryTakePotion(id);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{cmd} execute error");
                }
                
            }
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