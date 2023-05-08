using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;
using Game;
using System;

public class PlayerRelicListView : MonoBehaviour
{
    [SerializeField] private CellRelicView relicViewPrefab;
    [SerializeField] private Transform listParent;

    private UIViewObjectPool<CellRelicView, RelicData> objectPool;

    public void Init()
    {
        if (objectPool == null)
        {
            objectPool = new UIViewObjectPool<CellRelicView, RelicData>(relicViewPrefab, null);
        }
    }

    public void SetData(RelicAgent relicDatas)
    {
        //Debug.LogWarning(relicDatas.Count);
        //for (int i = 0; i < relicDatas.Count; i++)
        //{
        //    Debug.Log($"{relicDatas[i].Id}");
        //}
        objectPool.SetDatas(relicDatas, CellRelicViewSet, listParent);
        //Debug.LogError("Pause");
    }


    private void CellRelicViewSet(CellRelicView arg1, RelicData arg2)
    {
        arg1.SetData(arg2);
    }
}