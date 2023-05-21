using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class StartSceneManager : MonoBehaviour
    {
        [SerializeField] private Button NewGameButton;
        [SerializeField] private Button LoadGameButton;

        private void Start()
        {
            print(Paths._savePath);
            GameObject.Find("Load").GetComponent<Button>().interactable = File.Exists(Paths._savePath);
            if (!File.Exists(Paths._savePath))
            {
                GameObject.Find("Load").GetComponentInChildren<TMP_Text>().color = Color.grey;
            }

            NewGameButton.onClick.AddListener((() =>
            {
                SceneSwitchManager.Instance.NewGame = false;
                CurtainManager.Instance.UpCurtain("ChooseScene");
            }));

            LoadGameButton.onClick.AddListener((() =>
            {
                SceneSwitchManager.Instance.NewGame = true;
                CurtainManager.Instance.UpCurtain("MainScene");
            }));
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}