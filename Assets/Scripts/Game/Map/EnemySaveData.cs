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

        public override bool TryUseSkill(SkillData skill, out string info)
        {
            throw new NotImplementedException();
        }


        public void PlanAttackRound()
        {
            foreach (var skill in Skills)
            {
                if (CanCast(skill))
                {
                    ManageAttackRound(skill);
                    return;
                }
            }

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
            GameManager.Instance.PlayerData.ManageAttackRound(skill);
            

            if (GameManager.Instance.PlayerData.Engaging)
            {
                Debug.Log($"Engaging {Bp.Id}!");
            }
            GameManager.Instance.PlayerData.Engaging = false;
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
            Updated();
        }

        protected override void Destroyed()
        {
            GameManager.Instance.PlayerData.Gain(Gold);

            UI.EnemyPanel.Instance.gameObject.SetActive(false);
            base.Destroyed();
            GameManager.Instance.GetByData(this).UpdateFace();
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