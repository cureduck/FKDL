using Game;
using I2.Loc;
using Managers;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                Load();
            };
        }

        private void Load()
        {
            if ((GameManager.Instance.PlayerData == null)||(Id.IsNullOrWhitespace()))
            {
                IdText.SetTerm("empty");
                GetComponent<Button>().interactable = false;
                LvText.gameObject.SetActive(false);
            }
            else
            {
                LvText.gameObject.SetActive(true);
                GetComponent<Button>().interactable = true;
                IdText.SetTerm(Id);
                LvText.text = D.Count.ToString();
            }
        }


        public void UsePotion()
        {
            GameManager.Instance.PlayerData.UsePotion(Index);
        }
        
        
        public void Activate()
        {
            GetComponent<Animation>().Play();
        }
        
        private void OnMouseOver()
        {
            
        }
        
        
    }
}