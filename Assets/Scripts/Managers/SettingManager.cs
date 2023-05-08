using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    //[System.Serializable]
    //public struct SettingData 
    //{
    //    public string languageType;
    //    public Vector2Int resolutionSize;
    //    public bool isFullScreen;
    //    public float viewAngle;
    //    public float soundEffectSize;
    //    public float BGMSize;
    //}

    public class SettingManager : Singleton<SettingManager>
    {
        private const float maxDegree = -35;
        //public static readonly Resolution[] CurCanUseResolution = Screen.resolutions;

        //private SettingData settingData;

        public GameSettings GameSettings;
        public Slider DegreeSlider;

        protected override void Awake()
        {
            base.Awake();
            DegreeSlider.value = GameSettings.Degree / maxDegree;
            GameSettings.FOV = GameSettings.FOV;
            Application.targetFrameRate = 120;
        }

        //public void TotalSet(SettingData settingData) 
        //{
        //    SetLanguage(settingData.languageType);
        //    SetResolutionSize(settingData.resolutionSize);
        //    SetIsFullScreen(settingData.isFullScreen);
        //    SetDegree(settingData.viewAngle);
        //    SetSoundEffectSize(settingData.soundEffectSize);
        //    SetBGM(settingData.BGMSize);
        //}

        // public void SetLanguage(string languageTypeIndex)
        // {
        //     //settingData.languageType = languageTypeIndex;
        //     GameSettings.LanguageType = languageTypeIndex;
        // }

        public void SetResolutionSize(Vector2Int targetResolution)
        {
            //settingData.resolutionSize = targetResolution;
            GameSettings.ScreenSize = targetResolution;
        }


        public void SetIsFullScreen(bool isFullScreen)
        {
            //settingData.isFullScreen = isFullScreen;
            //Screen.SetResolution(settingData.resolutionSize.x, settingData.resolutionSize.y, isFullScreen);
            GameSettings.IsFullScreen = isFullScreen;
        }


        public void SetDegree(float f)
        {
            GameSettings.Degree = maxDegree * f;
            //settingData.viewAngle = f;
        }

        //public void SetSoundEffectSize(float size)
        //{
        //    settingData.soundEffectSize = size;
        //    AudioPlayer.Instance.SetSEVolume(size);
        //}

        //public void SetBGM(float size)
        //{
        //    settingData.BGMSize = size;
        //    AudioPlayer.Instance.SetBGMVolume(size);
        //}
    }
}