using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyTransition;
using Game.PlayerCommands;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    public class PlayerData : FighterData
    {
        private const int _floorLimit = 5;
        [JsonIgnore] private float _luckChance;
        public int BattleRound = 0;
        public bool DrawBack;

        [JsonIgnore] public FighterData enemy;
        public string Id;
        public Dictionary<Rank, int> Keys;

        [JsonIgnore] public Action<float> OnLuckyChanceChanged;
        public PotionData[] Potions;

        public string[] profInfo;
        public RelicAgent Relics;

        public PlayerData()
        {
            Relics = new RelicAgent();
        }

        [JsonIgnore]
        public bool Engaging
        {
            get => BattleRound == 0;
            set
            {
                if (value)
                {
                    BattleRound = 0;
                }
            }
        }

        [SerializeField]
        public float LuckyChance
        {
            get => _luckChance;
            set
            {
                _luckChance = value;
                OnLuckyChanceChanged?.Invoke(_luckChance);
            }
        }


        public int skillPoint => GameDataManager.Instance.SecondaryData.SkillPoint;

        [JsonIgnore] public override FighterData Enemy => enemy ?? (EnemySaveData)GameManager.Instance.Focus.Data;

        public void March(string destination)
        {
            Debug.Log($"destination {destination}");

            Player.Heal(0.3f, "march");
            Player.Heal(BattleStatus.Mp((int)(Player.Status.MaxMp * 0.3f)), "march");

            ClearAllBuffs();
            CheckChain(Timing.OnMarch, new object[] { this });

            if (destination.Contains((_floorLimit + 1).ToString()))
            {
                Destroyed();
            }
        }

        private void ClearAllBuffs()
        {
            Buffs.RemoveAll(data => data.Bp.BuffType == BuffType.Blessing || data.Bp.BuffType == BuffType.Curse);
            DelayUpdate();
        }

        /// <summary>
        /// Use a potion from the player's inventory.
        /// </summary>
        /// <param name="index">The index of the potion to use.</param>
        /// <param name="info">The output Info message.</param>
        /// <returns>Returns true if the potion was used successfully, false otherwise.</returns>
        public bool UsePotion(int index, out Info info)
        {
            if (!Potions[index].IsEmpty)
            {
                if (Potions[index].Bp.Fs.TryGetValue(Timing.PotionEffect, out var f))
                {
                    CheckChain<PotionData>(Timing.OnUsePotion, new object[] { Potions[index], this });
                    f?.Invoke(Potions[index], new object[] { this });
                    Potions[index].Count -= 1;
                    if (Potions[index].Count <= 0)
                    {
                        Potions[index].Id = "";
                    }

                    AudioPlayer.Instance.PlaySoundEffect("potion");
                    DelayUpdate();
                    info = new SuccessInfo();
                    return true;
                }

                info = new FailureInfo(FailureReason.PotionNotNow);
                return false;
            }
            else
            {
                info = new FailureInfo(FailureReason.NoTarget);
                return false;
            }
        }


        public void ReactWith(MapData cell)
        {
            CheckChain(Timing.OnReact, new object[] { cell, this });
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
            var outRank = CheckChain<Rank>(Timing.OnGetKey, new object[] { rank, this });
            Keys[outRank] += 1;
            return true;
        }


        [Button]
        public void AddSkillSlot()
        {
            Skills.Add(SkillData.Empty);
            DelayUpdate();
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
                            Upgrade(Skills[i]);
                            info = new SuccessInfo();
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
                        info = new SuccessInfo();
                        return true;
                    }
                }
            }

            info = new FailureInfo(FailureReason.NotEnoughSkillSlot, true);
            return false;
        }


        public event Action SkillPointChanged;

        [Button]
        public void SKillChanged()
        {
            SkillPointChanged?.Invoke();
        }


        public bool CanUpgradeWithSkillPoint(SkillData skillData, out Info info, bool autoBroadCast = false)
        {
            if (skillData.CurLv < skillData.Bp.MaxLv && SData.SkillPoint > 0)
            {
                info = new SuccessInfo();
                return true;
            }

            if (skillData.Bp.MaxLv != 1 && skillData.Bp.MaxLv <= skillData.CurLv && SData.BreakoutPoint > 0)
            {
                info = new SuccessInfo();
                return true;
            }

            info = new FailureInfo(FailureReason.SkillAlreadyMax, autoBroadCast);
            return false;
        }

        public void UpgradeWithPoint(SkillData skillData)
        {
            if (skillData.CurLv >= skillData.Bp.MaxLv)
            {
                SData.BreakoutPoint -= 1;
            }
            else
            {
                SData.SkillPoint -= 1;
            }

            Upgrade(skillData);

            DelayUpdate();
            SkillPointChanged?.Invoke();
        }

        public void GetSkillPoint(int v = 1)
        {
            SData.SkillPoint += v;

            SkillPointChanged?.Invoke();
        }


        public void GetBreakoutPoint(int v = 1)
        {
            SData.BreakoutPoint += v;
            SkillPointChanged?.Invoke();
        }


        public void GetRemoveSkillPoint(int v = 1)
        {
            SData.RemoveSkillPoint += v;
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
                //GetSkillPoint(Skills[index].Bp.Rank);
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


        public void Execute(IEnumerable<PlayerCommand> commands)
        {
            foreach (var command in commands)
            {
                command?.Execute(this);
            }
        }

        public bool UpgradeRandomSkill(out Info info, bool breakOut = false, bool autoBroadCast = false)
        {
            Func<SkillData, bool> filter;
            if (breakOut)
            {
                filter = SkillData.CanBreakOut;
            }
            else
            {
                filter = SkillData.CanUpgrade;
            }

            var skills = Skills.Where(filter).ToList();
            if (skills.Count > 0)
            {
                var index = SData.CurGameRandom.Next(0, skills.Count);
                var skill = skills[index];
                Upgrade(skill);
                info = new SuccessInfo();
                return true;
            }
            else
            {
                info = new FailureInfo(FailureReason.SkillAlreadyMax, autoBroadCast);
                return false;
            }
        }

        internal void UpgradeRandomSkills(Random random, Rank rank, int count = 1, bool breakout = false)
        {
            Func<SkillData, bool> filter;

            if (breakout)
            {
                filter = data => SkillData.CanBreakOut(data) && (rank == Rank.All || data.Bp.Rank == rank);
            }
            else
            {
                filter = data => SkillData.CanUpgrade(data) && (rank == Rank.All || data.Bp.Rank == rank);
            }

            var skills = Skills.Where(filter);
            if (skills.Count() <= count)
            {
                foreach (var skill in skills)
                {
                    Upgrade(skill);
                }
            }
            else
            {
                skills = skills.ChooseRandom(count, random);
                foreach (var skill in skills)
                {
                    Upgrade(skill);
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
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(path, f);
        }


        [Button]
        private void SaveToInit()
        {
            Save(Paths._initPath);
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

            SecondaryData.DeleteSave();

            GameObject.FindObjectOfType<TransitionManager>().LoadScene("StartScene", "DiagonalRectangleGrid", .2f);
        }
    }
}