using System;
using Game;
using Managers;
using UnityEngine;

namespace UI
{
    public class PlayerInfoPanel : UpdateablePanel<PlayerData>
    {
        [SerializeField]
        private SkillListView listView;


        protected void Start()
        {
            Open(GameManager.Instance.PlayerData);
            GameManager.Instance.GameLoaded += () =>
            {
                if (Data != null)
                {
                    Data.OnUpdated -= UpdateUI;
                    Data.OnSkillPointChanged -= OnSkillPointChanged;
                }

                Open(GameManager.Instance.PlayerData);
                //Data = GameManager.Instance.PlayerData;

                Data.OnUpdated += UpdateUI;
                Data.OnSkillPointChanged += OnSkillPointChanged;
                
                Text();
            };

        }

        private void OnSkillPointChanged()
        {
            throw new NotImplementedException();
        }

        private void Text() 
        {
            //Data = new PlayerData();
            SkillData skillData = new SkillData { Cooldown = 0, Id = "DZXY_ALC".ToLower(), CurLv = 1 };
            SkillData skillData02 = new SkillData { Cooldown = 0, Id = "YWLZ_ALC".ToLower(), CurLv = 1 };
            SkillData skillData03 = new SkillData { Cooldown = 0, Id = "YWLZ_ALC".ToLower(), CurLv = 1 };
            Debug.Log(Data.Skills.Count);
            for (int i = 0; i < 6; i++)
            {
                if (Data.Skills.Count >= 6)
                {
                    break;
                }
                Data.Skills.Add(null);
            }
            Data.Skills[0] = skillData;
            Data.Skills[1] = null;
            Data.Skills[2] = skillData02;
            Data.Skills[3] = skillData03;
            Debug.Log(Data.Skills.Count);
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            //Debug.Log(Data);
            if (Data != null) 
            {
                listView.SetData(Data, Data.Skills.ToArray());
            }

        }
    }
}