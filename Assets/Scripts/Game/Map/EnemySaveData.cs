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


        [JsonIgnore] public override FighterData Enemy => GameManager.Instance.PlayerData;
        
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
            Player.DrawBack = false;
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
            WindowManager.Instance.Display(this);
            
            Debug.Log(this);
            Debug.Log(Player);
            
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
            var playerAttack = GameManager.Instance.PlayerData.ManageAttackRound(skill);
            Attack? enemyAttack;
            
            if (!IsAlive)
            { 
                enemyAttack = null;
            }
            else
            {
                enemyAttack = PlanAttackRound();
            }
            base.OnReact();
            InformReactResult(new EnemyArgs() {PlayerAttack = playerAttack, EnemyAttack = enemyAttack});

            if (!IsAlive)
            {
                Destroyed();
            }
            
            GameManager.Instance.PlayerData.Engaging = false;
            DelayUpdate();
        }

        protected override void Destroyed()
        {
            GameManager.Instance.PlayerData.Gain(Gold, "trophy");
            
            UI.EnemyPanel.Instance.gameObject.SetActive(false);
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