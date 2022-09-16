using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        public string Id;

        public event Action Destroy;
        
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
            base.OnFocus();
        }

        public override void OnReact()
        {
            base.OnReact();
        }


        [JsonIgnore] public EnemyBp Bp => EnemyManager.Instance.EnemyBps[Id];
    }
}