using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;

public class ExampleCubePrefab : MonoBehaviour, IPoolObjectSetData
{
    public void InitOnSpawn()
    {
        Debug.Log("创建时的初始化");
    }

    public void SetDataOnEnable(object data)
    {
        Debug.Log("激活当前<color=yellow>方块</color>预制体，并传入参数：" + data);
    }
}