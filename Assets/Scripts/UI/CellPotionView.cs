using Game;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CellPotionView : MonoBehaviour
    {
        private int index;

        private string Id => data.Id;
        [JsonIgnore] public Potion Target => PotionManager.Instance.GetById(Id);
        public PotionData data;

        public Localize IdText;
        public TMP_Text LvText;

        public Image BottleIcon;
        public Image PotionIcon;

        [SerializeField]
        private PointEnterAndExit pointEvent;
        [SerializeField]
        private GameObject curSelectSign;

        public void Start() 
        {
            curSelectSign.SetActive(true);
            pointEvent.onPointEnter.AddListener(OnPointEnter);
            pointEvent.onPointExit.AddListener(OnPointExit);
        }

        private void OnEnable()
        {
            curSelectSign.gameObject.SetActive(false);
        }


        public void SetData(int index, PotionData data)
        {
            this.index = index;
            this.data = data;
            Load();
        }

        private void Load()
        {
            if ((GameManager.Instance.PlayerData == null)||(Id.IsNullOrWhitespace()))
            {
                IdText.SetTerm("empty");
                GetComponent<Button>().interactable = false;
                curSelectSign.gameObject.SetActive(false);
                BottleIcon.gameObject.SetActive(false);
                LvText.gameObject.SetActive(false);
            }
            else
            {
                LvText.gameObject.SetActive(true);
                GetComponent<Button>().interactable = true;
                IdText.SetTerm(Id);
                
                BottleIcon.gameObject.SetActive(true);

                BottleIcon.sprite = data.Bp.Icon;// SpriteManager.Instance.PotionBottleIcon[data.Bp.Rank];
                
                LvText.text = data.Count.ToString();
            }
        }


        public void UsePotion()
        {
            //Debug.Log("!!!!");
            GameManager.Instance.PlayerData.UsePotion(index);
            if (data.Count <= 0) 
            {
                OnPointExit();
            }
        }

        public void OnPointEnter() 
        {
            //Debug.Log("Enter!");
            //return;
            //curSelectSign.SetActive(true);
            if (!data.Id.IsNullOrWhitespace())
            {
                curSelectSign.SetActive(true);
            }
            Potion potion = PotionManager.Instance.GetById(data.Id);
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { describe = potion.Des, screenPosition = transform.position, title = data.Id });
        }


        public void OnPointExit()
        {
            //Debug.Log("Exit!");
            //curSelectSign.SetActive(false);
            //return;
            if (!data.Id.IsNullOrWhitespace())
            {
                curSelectSign.SetActive(false);
            }
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }

    }
}