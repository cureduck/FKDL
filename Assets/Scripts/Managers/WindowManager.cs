using DG.Tweening;
using I2.Loc;
using TMPro;
using UI;
using UnityEngine;

namespace Managers
{
    public class WindowManager : Singleton<WindowManager>
    {
        public OfferWindow OffersWindow;
        public ShopPanel ShopPanel;
        public CrystalPanel CrystalPanel;
        public GameObject MenuWindow;
        public GameObject CheatWindow;
        public EnemyInfoPanel EnemyPanel;
        public ConfirmPanel confirmPanel;
        public FightPredictPanel FightPredictPanel;
        public WarningInfoDebugPanel warningInfoPanel;
        public SettingPanel settingPanel;
        public StartAndEndPanel startAndEndPanel;
        public GameOverWindow GameOverWindow;
        public Localize Line;

        public SquareInfoPanel SquareInfoPanel;
        public SkillInfoPanel skillInfoPanel;
        public SimpleInfoItemPanel simpleInfoItemPanel;
        public UIEffectPanel effectPanel;

        public Transform dragViewParent;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            confirmPanel?.gameObject.SetActive(false);
            if (dragViewParent != null)
            {
                dragViewParent.transform.SetAsLastSibling();
            }

            CrystalPanel?.Init();
            ShopPanel?.Init();
            EnemyPanel?.Init();
            simpleInfoItemPanel?.Init();
            settingPanel?.Init();
            startAndEndPanel?.Init();
        }

        public void PerformLine(string id)
        {
            var term = WindowManager.Instance.Line;
            term.gameObject.SetActive(true);
            term.SetTerm(id);
            var seq = DOTween.Sequence();
            seq.Append(term.GetComponent<TMP_Text>()
                    .DOFade(1f, 5f))
                .Append(term.GetComponent<TMP_Text>()
                    .DOFade(0, 10f))
                .OnComplete((() => term.gameObject.SetActive(false)));
        }
    }
}