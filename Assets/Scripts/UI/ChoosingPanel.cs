using System;
using System.Collections.Generic;
using EasyTransition;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class ChoosingPanel : Singleton<ChoosingPanel>
    {
        public Image MainImage;
        public Image[] Images;

        [ShowInInspector] public Dictionary<string, Sprite> Icons;

        public List<Toggle> ToggleList;
        public List<Toggle> SelectedList;

        public const int RoleCount = 3;

        public Button GoAheadBtn;

        private void Start()
        {
            foreach (var toggle in ToggleList)
            {
                toggle.onValueChanged.AddListener(
                    (b) =>
                    {
                        Debug.Log(b);
                        if (b)
                        {
                            if (SelectedList.Count > RoleCount)
                            {
                                toggle.isOn = false;
                                return;
                            }

                            SelectedList.Add(toggle);

                            if (SelectedList.Count == RoleCount)
                            {
                                foreach (var toggle1 in ToggleList)
                                {
                                    if (!toggle1.isOn)
                                    {
                                        toggle1.interactable = false;
                                    }
                                }

                                GoAheadBtn.interactable = true;
                            }
                        }
                        else
                        {
                            SelectedList.Remove(toggle);
                            foreach (var toggle1 in ToggleList)
                            {
                                toggle1.interactable = true;
                            }

                            GoAheadBtn.interactable = false;
                        }

                        SetMainImage(toggle.name);
                    });
            }
        }

        [Button]
        public void SetMainImage(string id)
        {
            Debug.Log(id);
            MainImage.sprite = Icons[id];
            SetChosenProf();
        }


        private void SetChosenProf()
        {
            for (int i = 0; i < SelectedList.Count; i++)
            {
                Images[i].gameObject.SetActive(true);
                Images[i].sprite = Icons[SelectedList[i].name];
            }

            for (int i = SelectedList.Count; i < RoleCount; i++)
            {
                Images[i].gameObject.SetActive(false);
            }
        }

        public void ClearChosen()
        {
            SelectedList.Clear();
            foreach (var toggle in ToggleList)
            {
                toggle.isOn = false;
            }

            SetChosenProf();
        }


        public void GoAhead()
        {
            SceneSwitchManager.Instance.NewGame = true;
            FindObjectOfType<TransitionManager>().LoadScene("MainScene", "DiagonalRectangleGrid", .2f);
        }
    }
}