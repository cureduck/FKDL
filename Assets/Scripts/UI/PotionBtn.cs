using Game;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PotionBtn : MonoBehaviour
    {
        public int Index;

        public string Id => GameManager.Instance.PlayerData.Potions[Index].Id;
        public Potion Target => PotionManager.Instance.Lib[Id];
        public PotionData D => GameManager.Instance.PlayerData.Potions[Index];
        
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
            }
            else
            {
                LvText.gameObject.SetActive(true);
                IdText.SetTerm(Id);
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