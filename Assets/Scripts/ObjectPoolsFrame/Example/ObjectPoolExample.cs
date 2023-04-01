using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;

public class ObjectPoolExample : MonoBehaviour
{
    public GameObject curPrefab;

    public GameObject curPrefabSame;

    private ObjectPool objectPool01;
    private ObjectPool objectPool02;


    void Start()
    {
        StartCoroutine(TextIe());
        //objectPool.CreatInstance();
        //objectPool.CreatInstance();
    }


    private IEnumerator TextIe() 
    {
        //创建一个预制体对应的对象池
        objectPool01 = new ObjectPool(curPrefab);
        yield return new WaitForSeconds(1);
        //从对象池中取出一个预制体对象
        GameObject curSpawn01Prefab = objectPool01.CreatInstance(Vector3.zero,Quaternion.identity,null);
        yield return new WaitForSeconds(1);
        //回收一个预制体
        objectPool01.UnSpawnInstance(curSpawn01Prefab);
        yield return new WaitForSeconds(1);
        //再从对象池中取出一个对象池
        objectPool01.CreatInstance(Vector3.zero, Quaternion.identity, null);

      

    }

}
