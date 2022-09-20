﻿using System;
using Game;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillBtn : MonoBehaviour
    {
        public int Index;

        public string Id => GameManager.Instance.PlayerData.Skills[Index].Id;
        public Skill Target => SkillManager.Instance.Lib[Id];
        public SkillData D => GameManager.Instance.PlayerData.Skills[Index];
        
        public Localize IdText;
        public TMP_Text LvText;


        private void Start()
        {
            GameManager.Instance.GameLoaded += () =>
            {
                GameManager.Instance.PlayerData.OnUpdated += Load;
                GameManager.Instance.PlayerData.Skills[Index].Activate += Activate;
                Load();
            };
        }

        private void Load()
        {
            if ((GameManager.Instance.PlayerData == null)||(Id == null))
            {
                IdText.SetTerm("empty");
                LvText.gameObject.SetActive(false);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                LvText.gameObject.SetActive(true);
                IdText.SetTerm(Id);
                LvText.text = D.CurLv.ToString();
                GetComponent<Button>().interactable = Target.Positive;
            }

        }
        
        
        private void Activate()
        {
            GetComponent<Animation>().Play();
        }
        
        private void OnMouseOver()
        {
            
        }
    }
}