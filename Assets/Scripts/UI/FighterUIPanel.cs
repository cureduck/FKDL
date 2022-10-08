using Game;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public abstract class FighterUIPanel : MonoBehaviour
    {
        [ShowInInspector]
        protected FighterData _master;
        
        public void SetMaster(FighterData master)
        {
            if (master == _master) return;
            
            _master = master;
            UpdateData();
            master.OnUpdated += UpdateData;
        }

        protected abstract void UpdateData();
    }
}