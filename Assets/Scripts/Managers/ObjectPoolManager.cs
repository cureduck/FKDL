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
        [SerializeField]
        private UnpasstiveSkillTriggerSign skillTriggerSign;
        [SerializeField]
        private GameObject damageSignEffect;

        private ObjectPool attackEffectPool;
        private ObjectPool unpasstivePool;
        private ObjectPool damageSignEffectPool;

        protected override void Awake()
        {
            base.Awake();
            attackEffectPool = new ObjectPool(attackEffect);
            unpasstivePool = new ObjectPool(skillTriggerSign.gameObject);
            damageSignEffectPool = new ObjectPool(damageSignEffect);

        }
        #region 攻击特效
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
        #endregion

        #region 被动技能触发特效

        public GameObject SpawnUnPasstiveSkillSignEffect(Sprite icon)
        {
            GameObject cur = unpasstivePool.CreatInstance(icon);
            cur.AddComponent<InvokeTrigger>().Set(1.0f, () => UnSpawnUnPasstiveSkillSignEffect(cur));
            return cur;
        }

        private void UnSpawnUnPasstiveSkillSignEffect(GameObject targetObject)
        {
            unpasstivePool.UnSpawnInstance(targetObject);
        }
        #endregion

        #region 伤害显示
        public GameObject SpawnDamageSignEffect(int damage,int damageType)
        {
            GameObject cur = damageSignEffectPool.CreatInstance(new DamageSign.Args { value = damage, damageType = damageType });
            cur.AddComponent<InvokeTrigger>().Set(1.0f, () => UnSpawnDamageSignEffect(cur));
            return cur;
        }

        private void UnSpawnDamageSignEffect(GameObject targetObject)
        {
            damageSignEffectPool.UnSpawnInstance(targetObject);
        }


        #endregion


    }
}