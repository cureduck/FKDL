using Game;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillItem : MonoBehaviour
    {
        public Localize IdText;
        public TMP_Text LvText;

        
        public void Load(SkillData data)
        {
            if (data.IsEmpty)
            {
                IdText.SetTerm("empty");
                LvText.gameObject.SetActive(false);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                LvText.gameObject.SetActive(true);
                IdText.SetTerm(data.Id);
                LvText.text = data.CurLv.ToString();
                GetComponent<Button>().interactable = data.Bp.Positive;
            }
        }
        
        
        [Button]
        public void Activate()
        {
            GetComponent<Animator>().SetTrigger("activate");
        }
    }
}