using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class CurtainManager : Singleton<CurtainManager>
    {
        public Image Curtain;

        
        private void Start()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Curtain.gameObject.SetActive(true);
            Curtain.DOFade(0f, 2f)
                .OnComplete((() => Curtain.gameObject.SetActive(false)));
        }

        public void UpCurtain()
        {
            Curtain.gameObject.SetActive(true);
            Curtain.DOFade(1f, 2f);
        }
    }
}