using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        public string Id;


        [JsonIgnore] public override FighterData Enemy => GameManager.Instance.PlayerData;
        [JsonIgnore] public bool IsAlive => Status.CurHp > 0;
        
        public EnemySaveData(string id) : base()
        {
            Id = id;
            /*
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
            */

        }



        public void PlanAttackRound()
        {
            ManageAttackRound();
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
            WindowManager.Instance.Display(this);
            base.OnFocus();
        }

        public override void OnReact()
        {
            WindowManager.Instance.Display(this);
            GameManager.Instance.PlayerData.ManageAttackRound();
            if (!IsAlive)
            {
                Destroyed();
                return;
            }
            else
            {
                PlanAttackRound();
            }
            base.OnReact();
        }


        public void OnReact(SkillData skill)
        {
            GameManager.Instance.PlayerData.ManageAttackRound(skill);
            if (Status.CurHp <= 0)
            {
                Destroyed();
                return;
            }
            else
            {
                PlanAttackRound();
            }
            base.OnReact();
        }

        protected override void Destroyed()
        {
            GameManager.Instance.PlayerData.Gain(Gold);
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