using Game;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 由fighterUI来接收事件，然后调用子UI更新
    /// </summary>
    public abstract class FighterUIPanel : MonoBehaviour
    {
        [ShowInInspector]
        protected FighterData _master;
        
        /// <summary>
        /// 调用一次UpdateData，并监听OnUpdate事件
        /// </summary>
        /// <param name="master"></param>
        public virtual void SetMaster(FighterData master)
        {
            if (master == _master) return;
            
            _master = master;
            UpdateData();
            master.OnUpdated += UpdateData;
        }

        protected virtual void UpdateData(){}
    }
}