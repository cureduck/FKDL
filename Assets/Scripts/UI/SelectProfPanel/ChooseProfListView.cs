using System.Collections.Generic;
using CH.ObjectPool;
using Managers;
using UnityEngine;

public class ChooseProfListView : MonoBehaviour
{
    [SerializeField] private CellChooseProfView profViewPrefab;
    [SerializeField] private Transform listViewParent;

    private System.Action<CellChooseProfView, string, bool> cellClick;
    private System.Action<string> onLockCompleteEvent;
    private System.Action<CellChooseProfView, string> onPointEnter;
    private System.Action<CellChooseProfView, string> onPointExit;

    private UIViewObjectPool<CellChooseProfView, string> uIViewObjectPool;

    public void Init()
    {
        uIViewObjectPool = new UIViewObjectPool<CellChooseProfView, string>(profViewPrefab, null);
    }

    public void SetData(string[] profDatas, System.Action<CellChooseProfView, string, bool> cellClick,
        System.Action<CellChooseProfView, string> onPointEnter, System.Action<CellChooseProfView, string> onPointExit,
        System.Action<string> onLockCompleteEvent, List<int> curHaveSelect)
    {
        this.cellClick = cellClick;
        this.onPointEnter = onPointEnter;
        this.onPointExit = onPointExit;
        this.onLockCompleteEvent = onLockCompleteEvent;
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
    }

    private void OnCellViewSet(CellChooseProfView arg1, string arg2)
    {
        arg1.SetData(arg2, cellClick, onPointEnter, onPointExit, onLockCompleteEvent);

        //Profile.Delete();
        string[] curUnlock = Profile.GetOrCreate().ProUnlocks;

        bool isLock = true;
        for (int i = 0; i < curUnlock.Length; i++)
        {
            if (curUnlock[i] == arg2)
            {
                isLock = false;
                break;
            }
        }

        arg1.SetLock(isLock);
    }
}