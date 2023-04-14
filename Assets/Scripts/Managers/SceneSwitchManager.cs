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
        /// <summary>
        /// TRUE就是新游戏，FALSE就是加载游戏
        /// </summary>
        public bool NewGame;


        public void SwitchScene(string id)
        {
            SceneManager.LoadScene(id);
        }
    }
}