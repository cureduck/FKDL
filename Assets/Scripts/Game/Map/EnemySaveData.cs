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

        public override void UseSkill(SkillData skill)
        {
            throw new NotImplementedException();
        }

        public override bool TryUseSkill(SkillData skill, out Info info)
        {
            throw new NotImplementedException();
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


        public void Chase()
        {
            ManageAttackRound();
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
                Equip(sk);
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
            
            if (GameManager.Instance.PlayerData.Engaging)
            {
                //Debug.Log($"Engaging {Bp.Id}!");
            }
            GameManager.Instance.PlayerData.Engaging = false;
            if (!IsAlive)
            {
                Destroyed();
                enemyAttack = null;
            }
            else
            {
                enemyAttack = PlanAttackRound();
            }
            base.OnReact();
            InformReactResult(new EnemyArgs() {PlayerAttack = playerAttack, EnemyAttack = enemyAttack});
            Updated();
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
            return Id;
        }
    }
}