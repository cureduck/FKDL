using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Csv;
using Game;
using UnityEngine;

namespace Managers
{
    public class BuffManager : Singleton<BuffManager>
    {
        public Dictionary<string, Buff> Lib;

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            Lib = new Dictionary<string, Buff>();
            
            var csv = File.ReadAllText(Paths.BuffDataPath);
            
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var buff = Line2Buff(line);
                    Lib[buff.Id] = buff;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log($"buff load failed");
                }
            }
            FuncMatch();
        }

        private static Buff Line2Buff(ICsvLine line)
        {
            return new Buff
            {
                Id = line[0].ToLower()
            };
        }

        private void FuncMatch()
        {
            foreach (var v in Lib.Values)
            {
                v.Fs = new Dictionary<Timing, MethodInfo>();
            }
            
            foreach (var method in typeof(BuffData).GetMethods())
            {
                var attr = method.GetCustomAttribute<EffectAttribute>();

                if (attr!=null)
                {
                    Lib[attr.id.ToLower()].Fs[attr.timing] = method;
                }
            }
        }
    }
}