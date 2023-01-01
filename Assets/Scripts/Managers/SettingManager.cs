using UnityEngine;

namespace Managers
{
    public class SettingManager : Singleton<SettingManager>
    {

        public GameSettings GameSettings;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 120;
        }
    }
}