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
        
        public EnemySaveData(string id) : base()
        {
            Id = id;

            /*
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
            */
            
        }
        
        public override void Init()
        {
            base.Init();
            Status = Bp.Status;
            
            Skills = new SkillData[Bp.Skills.Length];
            Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
            Buffs = new List<BuffData>();
            foreach (var buff in Bp.Buffs)
            {
                Buffs.Add(buff);
            }
            
            foreach (var sk in Skills)
            {
                OnEquip(sk);
            }
        }


        public override void Load()
        {
            base.Load();
            foreach (var sk in Skills)
            {
                OnLoad(sk);
            }
        }


        public override void OnFocus()
        {
            WindowManager.Instance.Display(this);
            base.OnFocus();
        }

        public override void OnReact()
        {
            BattleManager.Instance.Fight(this);
            if (Status.CurHp <= 0)
            {
                Destroyed();
            }
            base.OnReact();
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
    }
}