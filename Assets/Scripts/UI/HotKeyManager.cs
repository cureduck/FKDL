using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

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
            if (Input.GetKeyDown(KeyCode.F1))
            {
                WindowManager.Instance.MenuWindow.SetActive(value:!WindowManager.Instance.MenuWindow.active);
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                WindowManager.Instance.CheatWindow.SetActive(value:!WindowManager.Instance.CheatWindow.active);
            }
            
            
        }
    }
}