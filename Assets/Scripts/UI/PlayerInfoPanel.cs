using Game;
using Managers;
using UnityEngine;

namespace UI
{
    public class PlayerInfoPanel : UpdateablePanel<PlayerData>
    {
        [SerializeField] private SkillListView listView;


        protected void Start()
        {
            Open(GameManager.Instance.Player);
            GameManager.Instance.GameLoaded += () =>
            {
                if (Data != null)
                {
                    Data.OnUpdated -= UpdateUI;
                    Data.SkillPointChanged -= SkillPointChanged;
                }

                Open(GameManager.Instance.Player);

                Data.OnUpdated += UpdateUI;
                Data.SkillPointChanged += SkillPointChanged;
            };
        }

        private void SkillPointChanged()
        {
            Debug.LogWarning("屏蔽警告");
            //throw new NotImplementedException();
        }


        protected override void UpdateUI()
        {
            Debug.Log(Data);
            if (Data != null)
            {
                listView.SetData(Data, Data.Skills.ToArray());
            }
        }
    }
}