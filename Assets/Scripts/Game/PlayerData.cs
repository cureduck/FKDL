using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EasyTransition;
using Game;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = System.Object;

namespace Game
{
    public class PlayerData : FighterData
    {
        public string Id;
        public PotionData[] Potions;
        public RelicAgent Relics;


        public float LuckyChance;
        
        public bool Engaging;
        public bool DrawBack;
        public Dictionary<Rank, int> Keys;
        public Dictionary<Rank, int> skillPoint => GameDataManager.Instance.SecondaryData.SkillPoint;

        public string[] profInfo;

        [JsonIgnore] public override FighterData Enemy => enemy ?? (EnemySaveData) GameManager.Instance.Focus.Data;

        [JsonIgnore] public FighterData enemy;
        
        
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
                    AudioPlayer.Instance.PlaySoundEffect("potion");
                    DelayUpdate();
                }
            }
        }
        
        
        public bool TryTakeOffer(Offer offer, out Info info, string kw = "")
        {
            bool success;
            switch (offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    success = TryTakePotion(offer.Id, out info);
                    break;
                case Offer.OfferKind.Skill:
                    success = TryTakeSkill(offer.Id, out info);
                    break;
                case Offer.OfferKind.Gold:
                    Gain(offer.Cost.ActualValue);
                    success = true;
                    info = null;
                    break;
                case Offer.OfferKind.Relic:
                    success = TryTakeRelic(offer.Id, out info);
                    break;
                case Offer.OfferKind.Key:
                    success = TryTakeKey(offer.Rank, out info);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (success)
            {
                Cost(offer.Cost, kw);
                
                
                DelayUpdate();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryTakeKey(Rank rank, out Info info)
        {
            info = new SuccessInfo();
            var outRank = CheckChain<Rank>(Timing.OnGetKey, new object[] {rank, this});
            Keys[outRank] += 1;
            return true;
        }
        
        
        


        [Button]
        public void AddSkillSlot()
        {
            Skills.Add(SkillData.Empty);
        }
        
        

        public bool TryTakePotion(string id, out Info msg)
        {
            msg = new Info();
            if (PotionManager.Instance.TryGetById(id, out var sk))
            {
                for (var i = 0; i < Potions.Length; i++)
                {
                    if ((Potions[i].Id == id))
                    {
                        Potions[i].Count += 1;
                        DelayUpdate();
                        return true;
                    }
                }
                
                for (int i = 0; i < Potions.Length; i++)
                {
                    if (Potions[i].IsEmpty)
                    {
                        Potions[i].Id = id;
                        Potions[i].Count = 1;
                        DelayUpdate();
                        return true;
                    }
                }
            }

            return false;
        }


        public bool TryTakeRelic(string id, out Info msg)
        {
            msg = new SuccessInfo();
            if (RelicManager.Instance.TryGetById(id, out var sk))
            {
                var relic = new RelicData(sk);
                Relics.Add(relic);
                OnGet(relic);
            }
            return true;
        }


        public bool TryTakeSkill(string id, out Info info)
        {
            info = new Info();
            if (SkillManager.Instance.TryGetById(id, out var sk))
            {
                for (int i = 0; i < Skills.Count; i++)
                {
                    if (Skills[i] != null && (Skills[i].Id == id))
                    {
                        if (sk.MaxLv <= Skills[i].CurLv)
                        {
                            info = new FailureInfo(FailureReason.SkillAlreadyMax);
                            return false;
                        }
                        else
                        {
                            Skills[i].LvUp(this);
                            DelayUpdate();
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
                        Skills[i].Load(sk);
                        //Equip(Skills[i]);
                        OnGet(Skills[i]);
                        DelayUpdate();
                        return true;
                    }
                }
            }
            return false;
        }






        public event Action SkillPointChanged;

        [Button]
        public void SKillChanged()
        {
            SkillPointChanged?.Invoke();
        }
        
        
        public bool CanUpgrade(SkillData skillData, out Info info)
        {
            if (skillData.CurLv < skillData.Bp.MaxLv)
            {
                info = new SuccessInfo();
                return true;
            }
            info = new FailureInfo(FailureReason.SkillAlreadyMax);
            return false;
        }

        public void UpgradeWithPoint(SkillData skillData)
        {
            Upgrade(skillData);

            for (var i = skillData.Bp.Rank; i < Rank.God; i++)
            {
                if (SecondaryData.SkillPoint[i] > 0)
                {
                    SecondaryData.SkillPoint[i] -= 1;
                    break;
                }
            }
            DelayUpdate();
            SkillPointChanged?.Invoke();
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
            SkillPointChanged?.Invoke();
            
        }


        public void GetRemoveSkillPoint(int v = 1)
        {
            GameDataManager.Instance.SecondaryData.RemoveSkillPoint += v;
            SkillPointChanged?.Invoke();
            DelayUpdate();
        }

        public void RemoveSkill(ref SkillData skill)
        {
            skill = SkillData.Empty;
            SkillPointChanged?.Invoke();
            DelayUpdate();
        }

        public void RemoveSkill(int index) 
        {
            if (index >= 0 && index < Skills.Count)
            {
                GetSkillPoint(Skills[index].Bp.Rank);
                Skills[index] = null;
                SkillPointChanged?.Invoke();
                DelayUpdate();
                //RemoveSkill(ref temp);
            }
        }


        public bool CanBeRemoved(SkillData skill)
        {
            return true;
        }
        
        
        [Button]
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
                            var esr = cmd.Split(':');
                            var idr = esr[1];
                            if (idr.EndsWith("/random"))
                            {
                                var rr = (Rank) int.Parse(idr[0].ToString());
                                idr = RelicManager.Instance.RollT(rr)[0].Id;
                                TryTakeRelic(idr, out _);
                            }
                            else
                            {
                                var rr = (Rank) int.Parse(idr[0].ToString());
                                GameManager.Instance.RollForRelic(rr);
                            }
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
                                        Cost(CostInfo.HpCost(count1));
                                    }
                                    break;
                                case "curmp":
                                    if (count1 > 0)
                                    {
                                        Heal(BattleStatus.Mp(count1));
                                    }
                                    else
                                    {
                                        Cost(CostInfo.MpCost(count1));
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
                            var jid = cmd.Split(':')[1].ToLower();
                            var cc = CrystalManager.Instance.Lib[jid];
                            WindowManager.Instance.CrystalPanel.Open((this, cc));
                            break;
                        case "potion":
                            var es = cmd.Split(':');
                            var id = es[1];
                            if (id.EndsWith("/random"))
                            {
                                var rr = (Rank) int.Parse(id[0].ToString());
                                id = PotionManager.Instance.RollT(rr)[0].Id;
                            }
                            var c = es.Length > 1 ? int.Parse(es[2]) : 1;
                            for (int i = 0; i < c; i++)
                            {
                                TryTakePotion(id, out _);
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


        public override string ToString()
        {
            return "Player :" + base.ToString();
        }


        protected override void Destroyed()
        {
            base.Destroyed();
            if (File.Exists(Paths._savePath))
            {
                File.Delete(Paths._savePath);
            }
            
            GameObject.FindObjectOfType<TransitionManager>().LoadScene("StartScene", "DiagonalRectangleGrid", .2f);
        }
    }
}