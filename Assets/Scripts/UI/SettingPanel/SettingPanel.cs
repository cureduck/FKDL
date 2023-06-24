using I2.Loc;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingPanel : BasePanel<GameSettings>
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Slider bgmSizeSilder;

    [SerializeField] private Slider soundEffectSizeSilder;

    //[SerializeField] private TMP_Dropdown languageSelecter;
    [SerializeField] private TMP_Dropdown resoltionSelecter;
    [SerializeField] private ResolutionSettingSelecter resolutionSetting;
    [SerializeField] private Toggle isFullScreenSelecter;
    [SerializeField] private Slider viewAngleSelecter;
    [SerializeField] private Button close_btn;

    //[FormerlySerializedAs("CameraAtoFollowToggle")] 
    [FormerlySerializedAs("CameraAutoFollowToggle")] [SerializeField]
    private TMP_Dropdown cameraFollowDropdown;

    [SerializeField] private LocalizeDropdown localizeDropdown;

    //[FormerlySerializedAs("back_to_start_btn")] 
    [SerializeField] private Button backToStartBtn;

    private void Start()
    {
        resolutionSetting.Init();
        Init();
    }

    public override void Init()
    {
        if (localizeDropdown._Terms.Count < 3)
        {
            localizeDropdown._Terms.Add("UI_SettingPanel_CameraAutoFollow_Always");
            localizeDropdown._Terms.Add("UI_SettingPanel_CameraAutoFollow_OnlyEnemy");
            localizeDropdown._Terms.Add("UI_SettingPanel_CameraAutoFollow_Never");
        }

        //LanguageSliderInit();
        cameraFollowDropdown.value = (int)gameSettings.CameraFollow;
        bgmSizeSilder.value = gameSettings.BgmVolume;
        soundEffectSizeSilder.value = gameSettings.SEVolume;
        viewAngleSelecter.value = gameSettings.Degree / GameSettings.MaxDegree;

        resoltionSelecter.value = 0;
        for (int i = 0; i < resoltionSelecter.options.Count; i++)
        {
            //Debug.Log(resoltionSelecter.options[i].text + $"{Screen.width}X{Screen.height}");
            //Debug.Log(resoltionSelecter.options[i].text == $"{Screen.width}X{Screen.height}");
            if (resoltionSelecter.options[i].text == $"{Screen.width}X{Screen.height}")
            {
                resoltionSelecter.value = i;
                break;
            }
        }

        isFullScreenSelecter.isOn = gameSettings.IsFullScreen;

        bgmSizeSilder.onValueChanged.AddListener(BGMSliderValueChange);
        soundEffectSizeSilder.onValueChanged.AddListener(SESliderValueChange);
        resoltionSelecter.onValueChanged.AddListener(ResoltionSelecterOnValueChange);
        isFullScreenSelecter.onValueChanged.AddListener(IsFullScreenToggleValueChange);
        viewAngleSelecter.onValueChanged.AddListener(ViewAngleSelectorSliderValueChange);
        close_btn.onClick.AddListener(ClosePanelButtonClick);
        cameraFollowDropdown.onValueChanged.AddListener(v =>
        {
            if (AudioPlayer.Instance)
            {
                AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
            }

            gameSettings.CameraFollow = (CameraSetting)v;
        });
        if (backToStartBtn)
        {
            backToStartBtn?.onClick.AddListener(() =>
            {
                GameManager.Instance.Save();
                CurtainManager.Instance.UpCurtain("StartScene");
            });
        }

        Camera.main.fieldOfView = gameSettings.FOV;
        Camera.main.transform.eulerAngles = new Vector3(gameSettings.Degree, 0, 0);
    }

    private void BGMSliderValueChange(float value)
    {
        gameSettings.BgmVolume = value;
        //if (AudioPlayer.Instance)
        //{
        //    AudioPlayer.Instance.SetBGMVolume(value);
        //}
    }

    private void SESliderValueChange(float value)
    {
        gameSettings.SEVolume = value;
        //if (AudioPlayer.Instance)
        //{
        //    AudioPlayer.Instance.SetSEVolume(value);
        //}
    }


    private void ResoltionSelecterOnValueChange(int index)
    {
        string[] temp = resoltionSelecter.options[index].text.Split('X');
        gameSettings.ScreenSize = new Vector2Int(int.Parse(temp[0]), int.Parse(temp[1]));
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
    }

    private void IsFullScreenToggleValueChange(bool isOn)
    {
        gameSettings.IsFullScreen = isOn;
        //AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
    }

    private void ViewAngleSelectorSliderValueChange(float angle)
    {
        gameSettings.Degree = angle * -35f;
    }

    private void ClosePanelButtonClick()
    {
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
        Close();
    }


    protected override void UpdateUI()
    {
    }
}