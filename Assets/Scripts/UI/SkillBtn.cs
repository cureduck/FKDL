using System;
using System.Threading.Tasks;
using Game;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillBtn : MonoBehaviour
    {
        public int Index;

        private string Id => GameManager.Instance.PlayerData.Skills[Index].Id;
        private Skill Target => SkillManager.Instance.Lib[Id];
        private SkillData D => GameManager.Instance.PlayerData.Skills[Index];
        
        public Localize IdText;
        public TMP_Text LvText;
        
        public enum BtnMode
        {
            Cast,
            Remove,
            Upgrade,
        }

        public BtnMode SkillMode;
        

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
        
        
        [Button]
        private void Activate()
        {
            GetComponent<Animator>().SetTrigger("activate");
        }


        public void Click(Action<SkillData> OnClick)
        {
            switch (SkillMode)
            {
                case BtnMode.Cast:
                    break;
                case BtnMode.Remove:
                    
                    break;
                case BtnMode.Upgrade:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnClick?.Invoke(D);
        }
        
    }
}