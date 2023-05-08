using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using I2.Loc;

public class TargetBattleView : MonoBehaviour
{
    [SerializeField] private Localize title_txt;
    [SerializeField] private Slider healthBar;
    [SerializeField] private DamageHightLightView damageHightLightView;
    [SerializeField] private Text healthValue_txt;
    [SerializeField] private Slider magicBar;
    [SerializeField] private Text magicValue_txt;
    [SerializeField] private Localize pd_txt;
    [SerializeField] private LocalizationParamsManager pdParamsManager;
    [SerializeField] private Localize md_txt;
    [SerializeField] private LocalizationParamsManager mdParamsManager;
    [SerializeField] private Localize pr_txt;
    [SerializeField] private LocalizationParamsManager prParamsManager;
    [SerializeField] private Localize mr_txt;
    [SerializeField] private LocalizationParamsManager mrParamsManager;
    [SerializeField] private CellSkillView curSkillView;
    [SerializeField] private BuffListView buffListView;

    private FighterData fighterData;

    public void Init()
    {
        if (curSkillView)
        {
            curSkillView.canInteractive = false;
        }

        buffListView.Init();
    }

    public void SetData(FighterData fighterData)
    {
        EnemySaveData enemySaveData = fighterData as EnemySaveData;
        if (enemySaveData != null)
        {
            title_txt.SetTerm(enemySaveData.Id);
        }
        else
        {
            title_txt.SetTerm("PLAYER");
        }

        this.fighterData = fighterData;

        healthBar.value = fighterData.Status.CurHp;
        healthBar.maxValue = fighterData.Status.MaxHp;
        healthValue_txt.text = $"{fighterData.Status.CurHp}/{fighterData.Status.MaxHp}";

        magicBar.value = fighterData.Status.CurMp;
        magicBar.maxValue = fighterData.Status.MaxMp;
        magicValue_txt.text = $"{fighterData.Status.CurMp}/{fighterData.Status.MaxMp}";

        pd_txt.SetTerm("PD_Title");
        pdParamsManager.SetParameterValue("VALUE", fighterData.Status.PAtk.ToString());
        md_txt.SetTerm("MD_Title");
        mdParamsManager.SetParameterValue("VALUE", fighterData.Status.MAtk.ToString());

        pr_txt.SetTerm("PR_Title");
        prParamsManager.SetParameterValue("VALUE", fighterData.Status.PDef.ToString());
        mr_txt.SetTerm("MR_Title");
        mrParamsManager.SetParameterValue("VALUE", fighterData.Status.MDef.ToString());

        if (curSkillView)
        {
            if (fighterData is PlayerData)
            {
                curSkillView.SetData(fighterData, null, 0, null, null, null);
            }
            else
            {
                if (fighterData.Skills.Count > 0)
                {
                    curSkillView.SetData(fighterData, fighterData.Skills[0], 0, null, null, null);
                }
                else
                {
                    curSkillView.SetData(fighterData, null, 0, null, null, null);
                }
            }
        }

        buffListView.SetData(fighterData.Buffs);
    }

    public void SetResult(int pd, int pdCount, int md, int mdCount, int td, int tdCount, int posionCount)
    {
        damageHightLightView.SetData(fighterData.Status.CurHp, pd, pdCount, md, mdCount, td, tdCount, posionCount);
    }
}