﻿using System;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        [JsonIgnore] public FighterData enemy;
        public string Id;

        public EnemySaveData(string id) : base()
        {
            Id = id;
            /*
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
            */
        }

        [JsonIgnore] public Rank Rank => Bp.Rank;


        [JsonIgnore] public override FighterData Enemy => enemy ?? GameManager.Instance.Player;


        [JsonIgnore]
        public EnemyBp Bp
        {
            get
            {
                if (!EnemyManager.Instance.EnemyBps.ContainsKey(Id))
                {
                    Debug.LogWarning(Id + " not found");
                }

                return EnemyManager.Instance.EnemyBps[Id];
            }
        }


        public Attack? PlanAttackRound()
        {
            foreach (var skill in Skills)
            {
                if (CanCast(skill, out _))
                {
                    return ManageAttackRound(skill);
                }
            }

            DeathCheck();

            return ManageAttackRound();
        }


        public void Chase(out Attack? attack)
        {
            Player.DrawBack = true;
            attack = ManageAttackRound();

            DeathCheck();

            ((PlayerData)Enemy).DrawBack = false;
        }


        public override void Init()
        {
            base.Init();

            var bonus = 0f;
            if (Area > 4 && Area < 9) bonus = 0.1f;
            if (Area > 8 && Area < 13) bonus = 0.2f;
            if (Area > 12 && Area < 17) bonus = 0.3f;

            Status = Bp.Status;
            if (Bp.Rank <= Rank.Uncommon)
            {
                Status.MaxHp = (int)(Status.MaxHp * (1 + bonus));
                Status.CurHp = (int)(Status.CurHp * (1 + bonus));
            }

            if (Bp.Skills == null)
            {
                Bp.Skills = Array.Empty<SkillData>();
            }

            Skills = new SkillAgent(Bp.Skills);
            //Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);


            Buffs = new BuffAgent();
            foreach (var buff in Bp.Buffs)
            {
                Buffs.Add(buff);
            }

            foreach (var sk in Skills)
            {
                OnGet(sk);
            }
        }


        public override void Load()
        {
            base.Load();
            foreach (var sk in Skills)
            {
                Load(sk);
            }
        }

        protected override void RevealAround()
        {
            if (!Cloned)
            {
                base.RevealAround();
            }
        }


        public override void OnFocus()
        {
            base.OnFocus();
        }

        public override void OnReact()
        {
            OnReact(null);
        }

        public override void OnLeave()
        {
            WindowManager.Instance.EnemyPanel.gameObject.SetActive(false);
            base.OnLeave();
            if (IsAlive)
            {
                DeathCheck();
                Chase(out _);
                DeathCheck();
            }
        }


        public void OnReact(SkillData skill)
        {
            OnReact(skill, out _, out _);
        }


        public void OnReact(SkillData skill, out Attack? attack1, out Attack? attack2)
        {
            attack1 = ((PlayerData)Enemy).ManageAttackRound(skill);

            attack2 = IsAlive ? PlanAttackRound() : null;


            base.OnReact();
            //InformReactResult(new EnemyArgs() {PlayerAttack = playerAttack, EnemyAttack = enemyAttack});

            DeathCheck();

            ((PlayerData)Enemy).BattleRound += 1;
            DelayUpdate();
        }

        protected override void Destroyed()
        {
            if (!Cloned)
            {
                SData.RecentCollectedSouls += Bp.Souls;
                Player.KillEnemy(this);
            }

            base.Destroyed();
        }


        public override string ToString()
        {
            return Id + "|" + base.ToString();
        }
    }
}