﻿using CH.ObjectPool;
using Game;
using UnityEngine;

public class BuffListView : MonoBehaviour
{
    [SerializeField] private Transform prefabParent;
    [SerializeField] private CellBuffView cellBuffView;
    [SerializeField] private bool isWorldObject = false;

    BuffAgent buffDatas;
    private UIViewObjectPool<CellBuffView, BuffData> objectPoolData;

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
        arg1.SetData(arg2, isWorldObject);
        arg1.transform.localRotation = Quaternion.identity;
        arg1.transform.localPosition = Vector3.zero;
    }
}