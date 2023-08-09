using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Csv;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class StoryManager : Singleton<StoryManager>
    {
        [ShowInInspector] private Dictionary<string, Scenario> Lib;
        public Dictionary<string, int> Killed => GameManager.Instance.SecondaryData.Killed;
        private static string _path => Path.Combine(Application.streamingAssetsPath, "Storyline.csv");
        private static string _prof => GameDataManager.Instance.SecondaryData.Profs[0];
        public Dictionary<string, bool> Switches => GameManager.Instance.SecondaryData.Switches;

        public string Triggering { get; private set; }

        private void Start()
        {
            LoadStories();
            BindEvents();
            DeepestNightmareCheck();
        }

        private void DeepestNightmareCheck()
        {
            if (GameDataManager.Instance.SecondaryData.Nightmares.Length == 5)
            {
                var code = "deepest_nightmare";
                Trigger(code);
            }
        }

        private void Trigger(string code)
        {
            Triggering = code;
            TriggerCheck();
        }


        private void BindEvents()
        {
            GameManager.Instance.Player.Marching += OnMarching;
            GameManager.Instance.Player.OnKillEnemy += OnKillEnemy;
            GameManager.Instance.Player.OnCollectCard += OnCollectCard;
        }

        private void OnKillEnemy(string id)
        {
            if (!Killed.ContainsKey(id))
            {
                Killed[id] = 1;
            }
            else
            {
                Killed[id]++;
            }

            Triggering = "killed_" + id;

            if (id == "king" || id == "demon lord")
            {
                GameManager.Instance.GameOver();
            }

            TriggerCheck();
        }

        private void OnCollectCard(string id)
        {
            Triggering = "card_" + id;
            var profile = Profile.GetOrCreate();
            profile.UnlockedCards.Add(id);
            profile.Save();
            TriggerCheck();
        }

        private void LoadStories()
        {
            Lib = new Dictionary<string, Scenario>();

            var csv = File.ReadAllText(_path, Encoding.UTF8);

            foreach (var line in CsvReader.ReadFromText(csv))
            {
                var id = line["id"];
                var conditions = line["conditions"];
                var content = line["content"];

                try
                {
                    Lib[id] = new Scenario(id, conditions, content);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError($"story {id} load failed");
                }
            }
        }


        private void OnMarching(int level)
        {
            Triggering = "lv_" + level.ToString();
            TriggerCheck();
        }

        private void TriggerCheck()
        {
            Debug.Log($"{Triggering} is triggered");
            foreach (var s in Lib.Values)
            {
                var b = s.Conditions.All(condition => condition.Check());

                if (!b) continue;
                foreach (var p in s.Performances)
                {
                    p.Perform();
                }
            }
        }
    }
}