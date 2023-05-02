using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class SceneSwitchManager : Singleton<SceneSwitchManager>
    {
        private void Start()
        {
            DontDestroyOnLoad(this);
        }


        public void SetNewGame(bool b)
        {
            NewGame = b;
        }
        
        /// <summary>
        /// TRUE就是新游戏，FALSE就是加载游戏
        /// </summary>
        public bool NewGame;
        
    }
}