using CH.ObjectPool;
using DG.Tweening;
using Game;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GoldPanel : FighterUIPanel
    {
        public TMP_Text GoldText;
        public TMP_Text skillPoint_txt;

        [SerializeField] private Transform goldPanelParent;

        public TMP_Text CKey;
        public TMP_Text GoldKey;
        public TMP_Text SilverKey;


        [SerializeField] private Color rank01KeyColor;
        [SerializeField] private Color rank02KeyColor;
        [SerializeField] private Color rank03KeyColor;
        [SerializeField] private GameObject curGetKeyEffect;
        [SerializeField] private GameObject gainCoinEffect;
        [SerializeField] private Transform effectParent;
        private int _currentGoldCount;

        private int _pause;

        private int _targetGoldCount;
        private ObjectPool gainCoinEffectPool;
        private ObjectPool keyObjectPool;


        private void Start()
        {
            keyObjectPool = new ObjectPool(curGetKeyEffect);
            gainCoinEffectPool = new ObjectPool(gainCoinEffect);
        }

        private void Update()
        {
            //GoldText.text = _targetGoldCount.ToString();
            if (_targetGoldCount != _currentGoldCount)
            {
                if (_pause > 0)
                {
                    _pause -= 1;
                    return;
                }
                else
                {
                    var delta = (_targetGoldCount - _currentGoldCount) / 10;
                    delta += _targetGoldCount - _currentGoldCount > 0 ? 1 : -1;
                    _currentGoldCount += delta;

                    GoldText.text = _currentGoldCount.ToString();
                    _pause = 10;
                }
            }
        }


        public override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            _targetGoldCount = ((PlayerData)_master).Gold;
            CKey.text = ((PlayerData)_master).Keys[Rank.Normal].ToString();
            SilverKey.text = ((PlayerData)_master).Keys[Rank.Uncommon].ToString();
            GoldKey.text = ((PlayerData)_master).Keys[Rank.Rare].ToString();
            skillPoint_txt.text = GameDataManager.Instance.SecondaryData.SkillPoint.ToString();
        }

        public void PlayGetKeyEffect(Vector2 screenPosition, Rank keyRank)
        {
            Color curColor;
            Vector3 endPosition;
            switch (keyRank)
            {
                case Rank.Normal:
                    curColor = rank01KeyColor;
                    endPosition = CKey.transform.position;
                    break;
                case Rank.Uncommon:
                    curColor = rank02KeyColor;
                    endPosition = SilverKey.transform.position;
                    break;
                default:
                    curColor = rank03KeyColor;
                    endPosition = GoldKey.transform.position;
                    break;
            }

            GameObject curKey = keyObjectPool.CreatInstance();
            curKey.GetComponentInChildren<Image>().color = curColor;
            curKey.transform.SetParent(effectParent);
            curKey.transform.localScale = Vector3.zero;

            curKey.transform.position = screenPosition;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(curKey.transform.DOScale(Vector3.one, 0.3f))
                .Append(curKey.transform.DOMove(endPosition, 0.5f)).Append(curKey.transform.DOScale(Vector3.zero, 0.3f))
                .OnComplete(() => UnSpawnKeyEffect(curKey));
            sequence.Play();
        }

        private void UnSpawnKeyEffect(GameObject target)
        {
            keyObjectPool.UnSpawnInstance(target);
        }

        #region 金币特效

        public void PlayGetCoinEffect(int getValue)
        {
            //Debug.Log("获得金币！");
            GameObject cur = gainCoinEffectPool.CreatInstance(getValue);
            cur.transform.SetParent(goldPanelParent);
            cur.transform.localPosition = Vector3.zero;
            cur.transform.localScale = Vector3.one;
            if (getValue > 0)
            {
                cur.transform.GetComponentInChildren<TMP_Text>().text = $"+{getValue}";
            }
            else
            {
                cur.transform.GetComponentInChildren<TMP_Text>().text = $"{getValue}";
            }

            cur.AddComponent<InvokeTrigger>().Set(1.5f, () => UnspawnCoinSignEffect(cur));
        }

        private void UnspawnCoinSignEffect(GameObject targetObject)
        {
            gainCoinEffectPool.UnSpawnInstance(targetObject);
        }

        #endregion
    }
}