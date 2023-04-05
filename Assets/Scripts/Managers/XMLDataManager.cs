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
using UnityEngine;
using Random = System.Random;

namespace Managers
{
    public abstract class XMLDataManager <T1, T2> : Singleton<XMLDataManager<T1, T2>> where T1 : CsvData
    {
        [ShowInInspector] private CustomDictionary<T1> Lib;

        protected abstract string CsvPath { get; }

        private void Start()
        {
            Load();
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

        public string[] Roll(Rank rank, int count = 1)
        {
            var s = new string[count];

            var selected = RollT(rank, count);
                
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = selected[i].Id;
            }
            return s;
        }

        public T1[] RollT(Rank rank, int count = 1)
        {
            return Lib.Values.Where((data => data.Rank == rank))
                .OrderBy((data => Lib.Random.Next()))
                .Take(count).ToArray();
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