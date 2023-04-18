using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;
using System;

public class ChooseProfListView : MonoBehaviour
{
    [SerializeField]
    private CellChooseProfView profViewPrefab;
    [SerializeField]
    private Transform listViewParent;

    private UIViewObjectPool<CellChooseProfView, string> uIViewObjectPool;

    private System.Action<CellChooseProfView, string,bool> cellClick;
    private System.Action<CellChooseProfView, string> onPointEnter;
    private System.Action<CellChooseProfView, string> onPointExit;


    public void Init() 
    {
        uIViewObjectPool = new UIViewObjectPool<CellChooseProfView, string>(profViewPrefab, null);
    }

    public void SetData(string[] profDatas,System.Action<CellChooseProfView,string, bool> cellClick, System.Action<CellChooseProfView, string> onPointEnter, System.Action<CellChooseProfView, string> onPointExit,List<int> curHaveSelect) 
    {
        this.cellClick = cellClick;
        this.onPointEnter = onPointEnter;
        this.onPointExit = onPointExit;
        uIViewObjectPool.SetDatas(profDatas, OnCellViewSet, listViewParent);

        int curActiveCount = uIViewObjectPool.ActiveCount();
        if (curHaveSelect.Count >= 3)
        {
            for (int i = 0; i < curActiveCount; i++)
            {
                Debug.Log(curHaveSelect.Contains(i));
                uIViewObjectPool.GetCurActiveMemberByIndex(i).SetToggleInteractable(curHaveSelect.Contains(i));
            }
        }

        for (int i = 0; i < curActiveCount; i++)
        {
            uIViewObjectPool.GetCurActiveMemberByIndex(i).SetToggleOnState(curHaveSelect.Contains(i));
        }

    }

    private void OnCellViewSet(CellChooseProfView arg1, string arg2)
    {
        arg1.SetData(arg2, cellClick, onPointEnter, onPointExit);
    }


}
