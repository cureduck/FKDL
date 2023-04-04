using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;
using Managers;

public class SkillListView : MonoBehaviour
{
    [SerializeField]
    private CellSkillView prefab;
    [SerializeField]
    private Transform listParent;
    [SerializeField]
    private CellSkillViewDragReceive deleteNode;

    private PlayerData playerData;

    private void Start()
    {
        deleteNode.onEndDrag = OnEndDragComplete;
        deleteNode.gameObject.SetActive(false);
    }

    //    public void Start()
    //    {
    //        StartCoroutine(Temp());
    //    }
    //    private IEnumerator Temp() 
    //    {
    //        yield return new WaitForSeconds(1.0f);
    //        SkillData[] skillDatas = new SkillData[]
    //{
    //            null,
    //            null,
    //            new SkillData { Cooldown = 0, Id = "YWLZ_ALC".ToLower(), CurLv = 1 },
    //            new SkillData { Cooldown = 2, Id = "JSLC_ALC".ToLower(), CurLv = 2 },
    //            null,
    //            null,
    //};
    //        SetData(skillDatas);
    //    }


    public void SetData(PlayerData playerData, SkillData[] curSkills)
    {
        this.playerData = playerData;
        for (int i = 0; i < curSkills.Length; i++)
        {
            CellSkillView cellSkillView;
            if (i >= listParent.childCount)
            {
                cellSkillView = Instantiate(prefab);
                cellSkillView.transform.SetParent(listParent);
                cellSkillView.transform.localScale = Vector3.one;
                cellSkillView.transform.localPosition = Vector3.zero;
            }
            else
            {
                cellSkillView = listParent.GetChild(i).GetComponent<CellSkillView>();
                cellSkillView.gameObject.SetActive(true);
            }
            cellSkillView.SetData(playerData, curSkills[i], i, OnStartDrag, OnEndDrag, OnEndDragComplete);
        }

        for (int i = curSkills.Length; i < listParent.childCount; i++)
        {
            listParent.GetChild(i).gameObject.SetActive(false);
        }

    }

    private void OnStartDrag(object data) 
    {
        deleteNode.gameObject.SetActive(true);
    }

    private void OnEndDrag(object data) 
    {
        deleteNode.gameObject.SetActive(false);
    }


    private void OnEndDragComplete(CellSkillView cellSkillView, CellSkillViewDragReceive cellSkillViewDragReceive)
    {
        if (cellSkillViewDragReceive.IsDeleteNode)
        {
            WindowManager.Instance.confirmPanel.Open(new ConfirmPanel.Args { curEvent = () => playerData.RemoveSkill(cellSkillView.Index), info = $"确定移除<color=yellow>{cellSkillView.Data.Id}</color>?" });
        }
        else
        {
            //if (cellSkillView.Index == cellSkillViewDragReceive.index) return;
            playerData.SwapSkill(cellSkillView.Index, cellSkillViewDragReceive.index);
            //Debug.Log(cellSkillView.Index + "与" + cellSkillViewDragReceive.index + "位置技能互换");
        }

    }

}
