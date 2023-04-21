using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        public string Id;


        [JsonIgnore] public override FighterData Enemy => enemy ?? GameManager.Instance.PlayerData;

        [JsonIgnore] public FighterData enemy;
        public EnemySaveData(string id) : base()
        {
            Id = id;
            /*
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
            */

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

            return ManageAttackRound();
        }


        private void Chase()
        {
            Player.DrawBack = true;
            ManageAttackRound();
            ((PlayerData)Enemy).DrawBack = false;
        }


        public override void Init()
        {
            base.Init();
            Status = Bp.Status;
            Gold = Bp.Gold;

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


        public override void OnFocus()
        {
            base.OnFocus();
            WindowManager.Instance.Display(this);
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
                Chase();
            }
        }


        public void OnReact(SkillData skill)
        {
            var playerAttack = ((PlayerData)Enemy).ManageAttackRound(skill);

            if (!IsAlive) PlanAttackRound(); 
            
            base.OnReact();
            //InformReactResult(new EnemyArgs() {PlayerAttack = playerAttack, EnemyAttack = enemyAttack});

            if (!IsAlive)
            {
                Destroyed();
            }
            
            ((PlayerData)Enemy).Engaging = false;
            DelayUpdate();
        }

        protected override void Destroyed()
        {
            Enemy.Gain(Gold, "trophy");
            
            base.Destroyed();
        }


        [JsonIgnore]
        public EnemyBp Bp
        {
            get
            {
                if (!EnemyManager.Instance.EnemyBps.ContainsKey(Id))
                {
                    Debug.LogWarning(Id +" not found");
                }
                return EnemyManager.Instance.EnemyBps[Id];
            }
            
        }

        public override string ToString()
        {
            return Id + "|" + base.ToString();
        }
    }
}