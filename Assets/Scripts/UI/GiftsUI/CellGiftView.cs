using System;
using Game;
using I2.Loc;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CellGiftView : BasePanel<Gift>
    {
        private static CellGiftView curClick;

        [SerializeField] private Image _icon;
        [SerializeField] private Localize _name;
        [SerializeField] private Localize _description;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _currentLvText;
        [SerializeField] private Localize _levelUpCost;
        [SerializeField] private Localize m_HoldToLevelUp;


        [SerializeField] private CellGiftLevelUpView levelUpView;
        [SerializeField] private Toggle toggle;
        [SerializeField] private PointEnterAndExit pointEvent;
        [SerializeField] private float triggerTime;
        private float _holdTime;
        private bool _isHold;

        public Action<Gift> UpgradeCallback;

        private void Start()
        {
            _icon.sprite = Data?.Icon;
            pointEvent.onPointLeftDown.AddListener(OnPointLeftDown);
            pointEvent.onPointLeftUp.AddListener(OnPointLeftUp);
            levelUpView.Init(LevelUpComplete);
        }


        void Update()
        {
            if (_isHold && Data.MaxLevel > Data.CurrentLevel)
            {
                _holdTime += Time.deltaTime;
                if (_holdTime >= triggerTime)
                {
                    toggle.interactable = false;
                    levelUpView.isProgress = true;
                }
                else
                {
                    toggle.interactable = true;
                    levelUpView.isProgress = false;
                }
            }
            else
            {
                _holdTime = 0;
            }


            if (curClick == this && Input.GetMouseButtonUp(0))
            {
                _isHold = false;
                _holdTime = 0;
                Debug.Log("UP!");
                toggle.interactable = true;
                levelUpView.isProgress = false;
                curClick = null;
            }
        }

        protected override void UpdateUI()
        {
            _name.SetTerm(Data.GetType().Name + "_title");
            _description.SetTerm(Data.GetType().Name);
            _description.SetLocalizeParam("CurLv", Data.CurrentLevel.ToString());
            _description.SetLocalizeParam("PointCost", (Data.PointCost).ToString());
            _description.Calculate();
            _levelUpCost.SetTerm($"升级花费:{Data.LevelUpCost.ToString()}");
            //_upgradeButton.onClick.AddListener((() => Data.Upgrade()));
            _currentLvText.text = Data.CurrentLevel >= Data.MaxLevel
                ? "<color=yellow>Max</color>"
                : $"<color=yellow>Lv.{Data.CurrentLevel}</color>";
            if (Data.MaxLevel <= Data.CurrentLevel)
            {
                _upgradeButton.gameObject.SetActive(false);
                _levelUpCost.gameObject.SetActive(false);
                m_HoldToLevelUp.gameObject.SetActive(false);
            }
        }

        public void UpdateView()
        {
            UpdateUI();
        }

        private void LevelUpComplete()
        {
            UpgradeCallback?.Invoke(Data);
            _holdTime = 0;
            _isHold = false;
            UpdateUI();
        }

        private void OnPointLeftDown()
        {
            curClick = this;
            _isHold = true;
        }

        private void OnPointLeftUp()
        {
            //_isHold = false;
        }
    }
}