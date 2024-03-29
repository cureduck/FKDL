﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.PlayerCommands;
using Managers;

namespace Game
{
    public class Crystal : CsvData
    {
        public int OptionLimit;
        public List<Option> Options;
        public string Title;


        public Crystal(Rank rank, int optionLimit = 999) : base(rank)
        {
            Options = new List<Option>();
            OptionLimit = optionLimit;
        }

        public Crystal()
        {
            Options = new List<Option>();
        }


        public IEnumerable<Option> GetOptions(Random random)
        {
            if (OptionLimit >= Options.Count)
            {
                return Options;
            }
            else
            {
                var selected = new List<Option>();
                var ordered = Options.GroupBy((option => option.Priority))
                    .OrderByDescending((options => options.Key));
                foreach (var item in ordered)
                {
                    if (selected.Count + item.Count() <= OptionLimit)
                    {
                        selected = selected.Concat(item).ToList();
                    }
                    else
                    {
                        var item2 = item.ToList();

                        while (selected.Count < OptionLimit)
                        {
                            var tmp = Tools.Tools.RandomElementByWeight(item2, option => option.Weight, new Random());
                            selected.Add(tmp);
                            item2.Remove(tmp);
                        }
                    }
                }

                return selected;
            }
        }

        public class Option
        {
            public PlayerCommand[] Commands;
            public CostInfo CostInfo;
            public string Effect;
            public string Line;
            public int Priority;
            public int Weight;


            public Option(string effect, string line, CostInfo costInfo, int priority = 0, int weight = 1)
            {
                Effect = effect;
                Line = line;
                CostInfo = costInfo;
                Priority = priority;
                Weight = weight;
                Commands = PlayerCommand.Interpret(effect);
            }
        }
    }
}