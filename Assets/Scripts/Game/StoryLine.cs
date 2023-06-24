using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;

namespace Game
{
    //trigger:lv_2|prof:ass
    public abstract class Condition
    {
        public abstract bool Check();

        public static Condition[] Interpret(string line)
        {
            var conditions = new List<Condition>();
            var phrases = line.Split('|');
            foreach (var s in phrases)
            {
                var prefix = s.Split(':')[0];
                var suffix = s.Split(':')[1];

                switch (prefix)
                {
                    case "trigger":
                        conditions.Add(new TriggerCondition(suffix));
                        break;
                    case "prof":
                        conditions.Add(new ProfCondition(suffix));
                        break;
                    default:
                        Debug.LogError($"can't interpret word {prefix}");
                        break;
                }
            }

            return conditions.ToArray();
        }
    }

    public class TriggerCondition : Condition
    {
        private readonly string Trigger;

        public TriggerCondition(string trigger)
        {
            Trigger = trigger;
        }


        public override bool Check()
        {
            return StoryManager.Instance.Triggering == Trigger;
        }
    }

    public class ProfCondition : Condition
    {
        private readonly string _prof;

        public ProfCondition(string prof)
        {
            _prof = prof;
        }

        public override bool Check()
        {
            return string.Equals(GameDataManager.Instance.SecondaryData.Profs[0], _prof,
                StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public abstract class Performance
    {
        public abstract void Perform();

        public static Performance[] Interpret(string line)
        {
            var performances = new List<Performance>();
            var phrases = line.Split('|');
            foreach (var s in phrases)
            {
                var prefix = s.Split(':')[0];
                var suffix = s.Split(':')[1];

                switch (prefix)
                {
                    case "story":
                        performances.Add(new StoryPreform(suffix));
                        break;
                    case "line":
                        performances.Add(new LinePerform(suffix));
                        break;
                    default:
                        Debug.LogError($"can't interpret word {prefix}");
                        break;
                }
            }

            return performances.ToArray();
        }
    }

    public class StoryPreform : Performance
    {
        private string Id;
        private Action OnClicked;

        public StoryPreform(string id, Action onClicked = null)
        {
            Id = id;
            OnClicked = onClicked;
        }

        public override void Perform()
        {
            WindowManager.Instance.startAndEndPanel.Open((Id + "_title", Id + "_content", null));
        }
    }


    public class LinePerform : Performance
    {
        private string Id;

        public LinePerform(string id)
        {
            Id = id;
        }

        public override void Perform()
        {
            var term = WindowManager.Instance.Line;
            term.gameObject.SetActive(true);
            term.SetTerm(Id);
            term.GetComponent<TMP_Text>()
                .DOFade(0, 10f)
                .OnComplete((() => term.gameObject.SetActive(false)));
        }
    }


    public struct Scenario
    {
        public Scenario(string name, string conditions, string performances)
        {
            Name = name;
            Conditions = Condition.Interpret(conditions);
            Performances = Performance.Interpret(performances);
        }

        public Scenario(string name, Condition[] conditions, Performance[] performances)
        {
            Name = name;
            Conditions = conditions;
            Performances = performances;
        }

        public string Name;
        public Condition[] Conditions;
        public Performance[] Performances;
    }
}