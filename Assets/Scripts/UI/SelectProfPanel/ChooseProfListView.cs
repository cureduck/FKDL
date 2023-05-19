using System.Collections.Generic;
using CH.ObjectPool;
using Managers;
using UnityEngine;

public class ChooseProfListView : MonoBehaviour
{
    [SerializeField] private CellChooseProfView profViewPrefab;
    [SerializeField] private Transform listViewParent;

    private System.Action<CellChooseProfView, string, bool> cellClick;
    private System.Action<CellChooseProfView, string> onPointEnter;
    private System.Action<CellChooseProfView, string> onPointExit;

    private UIViewObjectPool<CellChooseProfView, string> uIViewObjectPool;


    public void Init()
    {
        uIViewObjectPool = new UIViewObjectPool<CellChooseProfView, string>(profViewPrefab, null);
    }

    public void SetData(string[] profDatas, System.Action<CellChooseProfView, string, bool> cellClick,
        System.Action<CellChooseProfView, string> onPointEnter, System.Action<CellChooseProfView, string> onPointExit,
        List<int> curHaveSelect)
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
                //Debug.Log(curHaveSelect.Contains(i));
                uIViewObjectPool.GetCurActiveMemberByIndex(i).SetToggleInteractable(curHaveSelect.Contains(i));
            }
        }

        for (int i = 0; i < curActiveCount; i++)
        {
            uIViewObjectPool.GetCurActiveMemberByIndex(i).SetToggleOnState(curHaveSelect.Contains(i));
        }

        string[] curUnlocks = Profile.GetOrCreate().Unlocks;
        for (int i = 0; i < curUnlocks.Length; i++)
        {
            for (int x = 0; x < profDatas.Length; x++)
            {
                CellChooseProfView cellChooseProfView = uIViewObjectPool.GetCurActiveMemberByIndex(x);
                if (cellChooseProfView.ProfIndex == curUnlocks[i])
                {
                    cellChooseProfView.SetLock(false);
                    break;
                }
            }
        }
    }

    private void OnCellViewSet(CellChooseProfView arg1, string arg2)
    {
        arg1.SetData(arg2, cellClick, onPointEnter, onPointExit);
    }
}