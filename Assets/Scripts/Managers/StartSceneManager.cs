using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class StartSceneManager : MonoBehaviour
    {
        private void Start()
        {
            print(Paths._savePath);
            GameObject.Find("Load").GetComponent<Button>().interactable = File.Exists(Paths._savePath);
            if (!File.Exists(Paths._savePath))
            {
                GameObject.Find("Load").GetComponentInChildren<TMP_Text>().color = Color.grey;
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}