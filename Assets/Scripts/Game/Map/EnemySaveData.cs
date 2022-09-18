using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        public string Id;
        
        public EnemySaveData(string id) : base()
        {
            Id = id;
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            //Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
        }
        
        public override void Init()
        {
            base.Init();
            Status = Bp.Status;
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
                Destroy();
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