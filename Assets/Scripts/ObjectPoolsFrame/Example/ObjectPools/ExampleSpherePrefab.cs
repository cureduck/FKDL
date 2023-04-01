using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;
public class ExampleSpherePrefab : MonoBehaviour,IPoolObjectSetData
{
    public void InitOnSpawn()
    {
        Debug.Log("创建<color=yellow>球</color>时的初始化");
    }

    public void SetDataOnEnable(object data)
    {
        Debug.Log("激活<color=yellow>球</color>预制体的，并传入参数:" + data);
    }
}
