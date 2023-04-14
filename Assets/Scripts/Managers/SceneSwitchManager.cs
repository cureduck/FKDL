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
        public bool NewGame { get; set; }


        public void SwitchScene(string id)
        {
            SceneManager.LoadScene(id);
        }
    }
}