using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class SwitchManager : Singleton<SwitchManager>
    {
        public Animation Curtain;
        public bool NewGame { get; set; }
        
        
        private void Start()
        {
            DontDestroyOnLoad(transform.parent);
            DontDestroyOnLoad(Curtain.transform.parent);
            //DontDestroyOnLoad(Curtain);
        }


        public void SwitchScene(string scene)
        {
            Curtain.Play("FadeIn");
            StartCoroutine(StartLoading_4(scene));
        }
        
        
        private IEnumerator StartLoading_4(string scene) {
            int displayProgress = 0;
            int toProgress = 0;
            //Curtain.Play("FadeIn");
            AsyncOperation op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;
            while(op.progress < 0.9f) {
                toProgress = (int)op.progress * 100;
                while(displayProgress < toProgress) {
                    ++displayProgress;
                    Debug.Log(displayProgress);
                    
                    Debug.Log(Curtain.GetComponent<Image>().color.a);
                    yield return new WaitForEndOfFrame();
                }
            }
 
            toProgress = 100;
            Debug.Log(Curtain.GetComponent<Image>().color.a);
            Curtain.Play("FadeOut");
            while(displayProgress < toProgress){
                ++displayProgress;
                yield return new WaitForEndOfFrame();
            }
            op.allowSceneActivation = true;
            yield return new WaitForEndOfFrame();

            if ((scene == "MainScene"))
            {
                if (NewGame)
                {
                    GameManager.Instance.LoadFromInit();
                }
                else
                {
                    GameManager.Instance.LoadFromSave();
                }
            }
            
        }
    }
}