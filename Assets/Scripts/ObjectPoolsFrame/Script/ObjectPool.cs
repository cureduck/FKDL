using System.Collections.Generic;
using UnityEngine;
namespace CH.ObjectPool 
{
    public class ObjectPool
    {
        private static Transform PoolParent;
        private static Dictionary<ObjectPool, List<GameObject>> objectPoolsData;

        private Transform parent;
        private GameObject prefab;
        public ObjectPool(GameObject prefab)
        {
            if (PoolParent == null)
            {
                PoolParent = new GameObject("Pools").transform;
                objectPoolsData = new Dictionary<ObjectPool, List<GameObject>>();
            }
            this.prefab = prefab;

            //寻找有没有相同预制体的池子
            List<GameObject> curPool = null;
            foreach (var c in objectPoolsData)
            {
                if (c.Key.prefab == prefab)
                {
                    curPool = c.Value;
                    parent = c.Key.parent;
                }
            }
            //若没有相同预制体的池子
            if (curPool == null)
            {
                parent = new GameObject(prefab.name + "_Pool").transform;
                parent.SetParent(PoolParent);
                objectPoolsData.Add(this, new List<GameObject>());
            }
            //有相同预制体则共用一个池子
            else
            {
                objectPoolsData.Add(this, curPool);
            }
        }

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject CreatInstance(Vector3 position, Quaternion rotation, object initData)
        {
            List<GameObject> curPools = objectPoolsData[this];
            GameObject curInstance;



            if (curPools.Count > 0)
            {
                curInstance = curPools[0];
                curPools.RemoveAt(0);
                curInstance.SetActive(true);
                curInstance.transform.SetParent(null);
                MonoBehaviour[] curCompoent = curInstance.GetComponents<MonoBehaviour>();
                for (int i = 0; i < curCompoent.Length; i++)
                {
                    IPoolObjectSetData poolObjectSetData = curCompoent[i] as IPoolObjectSetData;
                    if (poolObjectSetData != null)
                    {
                        poolObjectSetData.SetDataOnEnable(initData);
                    }
                }
            }
            else
            {
                curInstance = GameObject.Instantiate(prefab);
                MonoBehaviour[] curCompoent = curInstance.GetComponents<MonoBehaviour>();
                for (int i = 0; i < curCompoent.Length; i++)
                {
                    IPoolObjectSetData poolObjectSetData = curCompoent[i] as IPoolObjectSetData;
                    if (poolObjectSetData != null)
                    {
                        poolObjectSetData.InitOnSpawn();
                        poolObjectSetData.SetDataOnEnable(initData);
                    }
                }
            }

            curInstance.transform.position = position;
            curInstance.transform.rotation = rotation;

            return curInstance;
        }

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <returns></returns>
        public GameObject CreatInstance()
        {
            return CreatInstance(prefab.transform.position, prefab.transform.rotation, null);
        }

        /// <summary>
        /// 创建一个实例并分配初始数据
        /// </summary>
        /// <returns></returns>
        public GameObject CreatInstance(object initData)
        {
            return CreatInstance(prefab.transform.position, prefab.transform.rotation, initData);
        }


        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnSpawnInstance(GameObject gameObject)
        {
            List<GameObject> curPools = objectPoolsData[this];
            curPools.Add(gameObject);
            gameObject.transform.SetParent(parent.transform);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 转换场景的时候调用，清楚所有的对象池缓存
        /// </summary>
        public static void PoolsClear()
        {
            PoolParent = null;
            objectPoolsData = null;
        }

        private static void Init() 
        {
            
        }
    }

}
