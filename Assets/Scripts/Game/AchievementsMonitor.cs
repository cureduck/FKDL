using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public abstract class Achievement
    {
        [JsonIgnore] public abstract Sprite Icon { get; }
        public abstract string Id { get; }
        public virtual string Desc => Id + "_desc";

        public bool IsCompleted { get; protected set; }

        public abstract void Bind();
    }


    public class AchievementsMonitor
    {
        public AchievementsMonitor()
        {
        }

        public Achievement[] Achievements { get; private set; }

        public static AchievementsMonitor GetDefault()
        {
            var a = new AchievementsMonitor();
            a.Init();
            return a;
        }

        private void Init()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var achievements = new List<Achievement>();
            foreach (var type in types)
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(Achievement))) continue;
                var achievement = (Achievement)type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
                if (achievement != null)
                {
                    achievements.Add(achievement);
                }
            }

            Achievements = achievements.ToArray();
        }
    }
}