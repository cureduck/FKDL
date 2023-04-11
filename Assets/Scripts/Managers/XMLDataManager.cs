using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv;
using Game;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Tools;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Managers
{
    public interface IProvider<out T> where T : CsvData
    {
        T GetById(string id);
        
        /// <summary>
        /// 根据最低生成等级rank，有luckyChance概率生成更高一级物品，
        /// 有luckyChance/2概率生成再高一级，依次类推。
        /// 按照此生成方法，生成count个不同的物品
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="luckyChance"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        T[] GenerateT(Rank rank, float luckyChance, int count);

        /// <summary>
        /// 生成count个随机不同物品
        /// </summary>
        /// <param name="rank">物品生成等级</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        T[] RollT(Rank rank, int count);
    }
    
    
    public abstract class XMLDataManager <T1, T2> : Singleton<XMLDataManager<T1, T2>> ,IProvider<T1> where T1 : CsvData
    {
        [ShowInInspector] private CustomDictionary<T1> Lib;

        protected abstract string CsvPath { get; }

        private void Start()
        {
            Load();
        }

        protected virtual Sprite GetIcon(string id)
        {
            SpriteManager.Instance.BuffIcons.TryGetValue(id, out var icon);
            return icon;
        }
        
        protected virtual void Load()
        {
            Lib = new CustomDictionary<T1>();

            var csv = File.ReadAllText(CsvPath, Encoding.UTF8);

            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var t = Line2T(line);
                    if (t != null)
                    {
                        Lib[t.Id] = t;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debug.LogError($"{line} read error");
                }
            }
            
            FuncMatch();
        }
        


        [CanBeNull] protected abstract T1 Line2T(ICsvLine line);

        private void FuncMatch()
        {
            foreach (var method in typeof(T2).GetMethods())
            {
                var attr = method.GetCustomAttribute<EffectAttribute>();
                if (attr != null)
                {
                    
                    if (Lib.TryGetValue(attr.id.ToLower(), out  var v))
                    {
                        v.Fs[attr.timing] = method;
                    }
                    else
                    {
#if UNITY_EDITOR
                        /*var t = new T1 { Id = attr.id};
                        t.Fs[attr.timing] = method;
                        Lib[attr.id.ToLower()] = t;*/
#endif
                    }
                }
            }
        }

        
        [CanBeNull]
        public T1 GetById(string id)
        {
            
            if (id == null) return null;
            
            Lib.TryGetValue(id, out var v);
            return v;
        }
        


        protected virtual IEnumerable<T1> GetCandidates(Rank rank)
        {
            return Lib.Values.Where((data => data.Rank == rank));
        }

        
        /// <summary>
        /// 注意如果所需数目太大超过所有可能选项，则只会产生所有可能选项，即达不到输入数目
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T1[] RollT(Rank rank, int count = 1)
        {
            if (count == 0)
            {
                return new T1[0];
            }
            
            var candidates = GetCandidates(rank);
            if (candidates.Count() <= count)
            {
                return candidates.ToArray();
            }
            else
            {
                return candidates
                    .OrderBy((data => Lib.Random.Next()))
                    .Take(count).ToArray();
            }
        }
        
        
        private const float LuckPassRate = .5f;
        
        /// <summary>
        /// 随机卡牌过程如下，首先尝试生成最高级卡牌，再依次向下尝试生成
        /// </summary>
        /// <param name="rank">最低生成等级</param>
        /// <param name="luckyChance"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Button]
        public T1[] GenerateT(Rank rank, float luckyChance, int count = 1)
        {
            IEnumerable<T1> s = new LinkedList<T1>();
            var slotLeft = count;
            var random = Lib.Random;
            for (var i = Lib.RankLevels -2; i >= (int)rank; i--)
            {
                var rand = random.NextDouble();
                var p = luckyChance * Math.Pow(LuckPassRate, i - (int)rank);
                
                //二项分布
                var bin = new MathNet.Numerics.Distributions.Binomial(p, slotLeft, random);
                var k = 0;
                //二项分布的概率累计函数
                while (rand > bin.CumulativeDistribution(k))
                {
                    k += 1;
                }

                var chosen = RollT((Rank) i+1, k);
                s = s.Concat(chosen);
                
                slotLeft -= k;
                if (slotLeft == 0) break;
            }

            s = s.Concat(RollT(rank, slotLeft));

            return s.ToArray();
        }
        
        public bool ContainsKey(string id)
        {
            return Lib.ContainsKey(id);
        }

        public void SetRandom(Random random)
        {
            Lib.Random = random;
        }

        public bool TryGetById(string id, out T1 v)
        {
            return Lib.TryGetValue(id, out v);
        }
    }
}