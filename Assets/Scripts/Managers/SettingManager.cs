using UnityEngine;

namespace Managers
{
    public class SettingManager : Singleton<SettingManager>
    {
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 30;
        }
    }
}