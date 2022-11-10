using System;
using Managers;

namespace UI
{
    public class HotKeyManager : Singleton<HotKeyManager>
    {
        public string[] SkillHotKeys;


        protected override void Awake()
        {
            base.Awake();
            SkillHotKeys = new string[13];
            for (var i = 0; i < 13; i++)
            {
                SkillHotKeys[i] = i.ToString();
            }
        }


        private void Update()
        {
            
        }
        
        
    }
}