using System;
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
        public Skill Target => SkillManager.Instance.Skills[Id];
        public SkillData D => GameManager.Instance.PlayerData.Skills[Index];
        
        public Localize IdText;
        public TMP_Text LvText;


        private void Start()
        {
            GameManager.Instance.GameLoaded += () =>
            {
                GameManager.Instance.PlayerData.OnUpdated += Load;
                Load();
            };
        }

        private void Load()
        {
            IdText.SetTerm(Id);
            LvText.text = D.CurLv.ToString();
        }
        
        private void OnMouseOver()
        {
            
        }
    }
}