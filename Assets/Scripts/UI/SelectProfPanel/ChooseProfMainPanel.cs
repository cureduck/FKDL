﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using I2.Loc;
using Managers;
using System;

public class ChooseProfMainPanel : MonoBehaviour
{
    private const string ChooseProfTitle = "UI_Icon_ChooseProf_";

    [SerializeField]
    private ChooseProfListView profListView;
    [Header("当前职业信息")]
    [SerializeField]
    private GameObject chooseProfInfoView;
    [SerializeField]
    private Localize profTitle_txt;
    [SerializeField]
    private Image chooseProfInfoIcon_img;
    [SerializeField]
    private Localize profDescribe_txt;
    [Header("当前已经选择职业")]
    [SerializeField]
    private CellProfView mainProf;
    [SerializeField]
    private CellProfView secondProf01;
    [SerializeField]
    private CellProfView secondProf02;
    [Header("按钮")]
    [SerializeField]
    private Button resetChoose_btn;
    [SerializeField]
    private Button startGame_btn;

    private string[] curCanSelectProfs;

    private string[] profDatas;

    #region 内部自用
    private List<int> curSelectViewIndex;
    #endregion

    public void Init()
    {
        profListView.Init();
        resetChoose_btn.onClick.AddListener(ResetSelect);
        startGame_btn.onClick.AddListener(StartGame);
        chooseProfInfoView.gameObject.SetActive(false);
        curSelectViewIndex = new List<int>();
        profDatas = new string[3];
    }

    private void Start()
    {
        Init();
        curCanSelectProfs = new string[] { "ALC", "ASS", "BAR", "BLI", "CUR", "KNI", "MAG" };
        UpdateView();
    }

    private void UpdateView()
    {
        profListView.SetData(curCanSelectProfs, CellClick, CellPointEnter, CellPointExit, curSelectViewIndex);

        mainProf.SetData(profDatas[0]);
        secondProf01.SetData(profDatas[1]);
        secondProf02.SetData(profDatas[2]);

        startGame_btn.interactable = curSelectViewIndex.Count >= 3;

    }

    private void CellPointExit(CellChooseProfView arg1, string arg2)
    {
        chooseProfInfoView.gameObject.SetActive(false);
    }

    private void CellPointEnter(CellChooseProfView arg1, string arg2)
    {
        chooseProfInfoView.gameObject.SetActive(true);
        profTitle_txt.SetTerm(arg2);
        chooseProfInfoIcon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.ChooseProf, $"{ChooseProfTitle}{arg2}");
        profDescribe_txt.SetTerm($"{arg2}职业介绍");
    }

    private void CellClick(CellChooseProfView cellChooseProfView, string curProfData, bool isActive)
    {
        //选择一个职业
        if (isActive)
        {
            for (int i = 0; i < profDatas.Length; i++)
            {
                if (string.IsNullOrEmpty(profDatas[i]))
                {
                    curSelectViewIndex.Add(GetProfIndexByData(curProfData));
                    profDatas[i] = curProfData;
                    break;
                }
            }
        }
        //移除一个职业
        else
        {
            for (int i = 0; i < profDatas.Length; i++)
            {
                if (profDatas[i] == curProfData)
                {
                    curSelectViewIndex.Remove(GetProfIndexByData(curProfData));
                    profDatas[i] = string.Empty;
                    break;
                }
            }
        }

        UpdateView();
        //Debug.Log($"{cellChooseProfView}:{index}被点击");
    }

    private int GetProfIndexByData(string profData)
    {
        for (int i = 0; i < curCanSelectProfs.Length; i++)
        {
            if (curCanSelectProfs[i] == profData)
            {
                return i;
            }
        }
        return -1;
    }

    private void ResetSelect()
    {
        for (int i = 0; i < profDatas.Length; i++)
        {
            profDatas[i] = string.Empty;
        }
        curSelectViewIndex.Clear();
        UpdateView();
    }

    private void StartGame() 
    {
        GameDataManager.Instance.SecondaryData = new Game.SecondaryData();
        GameDataManager.Instance.SecondaryData.Prof = profDatas;
        SceneSwitchManager.Instance.SwitchScene("MainScene");
    }

}