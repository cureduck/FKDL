using System;
using System.Collections.Generic;
using Game;
using UI;
using UnityEngine;
using UI.BuffUI;
using CH.ObjectPool;

public class BuffListView : MonoBehaviour
{
    [SerializeField]
    private Transform prefabParent;
    [SerializeField]
    private CellBuffView cellBuffView;
    private UIViewObjectPool<CellBuffView, BuffData> objectPoolData;

    BuffAgent buffDatas;
    public void Init()
    {
        if (objectPoolData == null) 
        {
            objectPoolData = new UIViewObjectPool<CellBuffView, BuffData>(cellBuffView, null);
        }

    }

    public void SetData(BuffAgent buffDatas)
    {
        this.buffDatas = buffDatas;
        objectPoolData.SetDatas(buffDatas, OnCellBuffSet, prefabParent);
    }

    private void OnCellBuffSet(CellBuffView arg1, BuffData arg2)
    {
        arg1.SetData(arg2);
    }



}