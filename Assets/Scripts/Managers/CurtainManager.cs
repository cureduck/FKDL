using System;
using System.Collections;
using DG.Tweening;
using EasyTransition;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class CurtainManager : Singleton<CurtainManager>
    {
        public TransitionManager transitionManager;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void UpCurtain(string id)
        {
            transitionManager.LoadScene(id, "DiagonalRectangleGrid", .2f);
        }
    }
}