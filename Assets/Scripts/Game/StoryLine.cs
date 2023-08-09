using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using Tools;
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
                // suffix means all words after :
                var suffix = s.RemoveBefore(':');

                switch (prefix)
                {
                    case "trigger":
                        conditions.Add(new TriggerCondition(suffix));
                        break;
                    case "switch":
                        conditions.Add(new SwitchCondition(suffix));
                        break;
                    case "prof":
                        conditions.Add(new ProfCondition(suffix));
                        break;
                    default:
                        Debug.LogError($"can't interpret word {prefix}-{suffix}");
                        break;
                }
            }

            return conditions.ToArray();
        }
    }


    public class SwitchCondition : Condition
    {
        public readonly string Switch;

        public SwitchCondition(string suffix)
        {
            Switch = suffix;
            StoryManager.Instance.Switches[Switch] = false;
        }

        public override bool Check()
        {
            return StoryManager.Instance.Switches[Switch] == true;
        }

        public override string ToString()
        {
            return $"switch:{Switch} on";
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

        public override string ToString()
        {
            return $"trigger:{Trigger}";
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

        public override string ToString()
        {
            return $"prof:{_prof}";
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
                var suffix = s.RemoveBefore(':');

                switch (prefix)
                {
                    case "story":
                        performances.Add(new StoryPreform(suffix));
                        break;
                    case "line":
                        performances.Add(new LinePerform(suffix));
                        break;
                    case "switch":
                        var words0 = suffix.Split(',');
                        performances.Add(new SwitchPerform(words0[0], bool.Parse(words0[1])));
                        break;
                    case "replace":
                        var words = suffix.Split(',');
                        performances.Add(new ReplacePerform(words[0], words[1]));
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

        public override string ToString()
        {
            return $"story:{Id}";
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
            WindowManager.Instance.PerformLine(Id);

            var term = WindowManager.Instance.Line;
            term.gameObject.SetActive(true);
            term.SetTerm(Id);
            term.GetComponent<TMP_Text>()
                .DOFade(0, 10f)
                .OnComplete((() => term.gameObject.SetActive(false)));
        }

        public override string ToString()
        {
            return $"line:{Id}";
        }
    }

    public class SwitchPerform : Performance
    {
        private string Switch;
        private bool Value;


        public SwitchPerform(string @switch, bool value)
        {
            Switch = @switch;
            Value = value;
        }

        public override void Perform()
        {
            StoryManager.Instance.Switches[Switch] = Value;
        }

        public override string ToString()
        {
            return $"switch:{Switch}:{Value}";
        }
    }


    public class ReplacePerform : Performance
    {
        private readonly string newId;
        private readonly string oldId;

        public ReplacePerform(string oldId, string newId)
        {
            this.oldId = oldId;
            this.newId = newId;
        }

        public override void Perform()
        {
            GameManager.Instance.Map.ReplaceAllEnemy(oldId, newId);
        }

        public override string ToString()
        {
            return $"replace:{oldId}:{newId}";
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