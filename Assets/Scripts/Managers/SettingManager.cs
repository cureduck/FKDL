using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class SettingManager : Singleton<SettingManager>
    {

        public GameSettings GameSettings;
        public Slider DegreeSlider;

        private float maxDegree = -35;
        
        protected override void Awake()
        {
            base.Awake();
            DegreeSlider.value = GameSettings.Degree / maxDegree;
            GameSettings.FOV = GameSettings.FOV;
            Application.targetFrameRate = 120;
        }


        public void SetDegree(float f)
        {
            GameSettings.Degree = maxDegree * f;
        }
    }
}