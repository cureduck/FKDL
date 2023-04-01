using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;

namespace Managers
{
    /// <summary>
    /// 用于处理对象池的管理器
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        [SerializeField]
        private GameObject attackEffect;
        private ObjectPool attackEffectPool;

        protected override void Awake()
        {
            base.Awake();
            attackEffectPool = new ObjectPool(attackEffect);

        }

        public GameObject SpawnAttackEffect() 
        {
            GameObject cur = attackEffectPool.CreatInstance();
            cur.AddComponent<InvokeTrigger>().Set(0.5f, () => UnSpawnAttackEffect(cur));
            return cur;
        }

        public void UnSpawnAttackEffect(GameObject targetObject) 
        {
            attackEffectPool.UnSpawnInstance(targetObject);
        }
    }
}