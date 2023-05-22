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
    [SerializeField] private Toggle isFullScreenSelecter;
    [SerializeField] private Slider viewAngleSelecter;

    [SerializeField] private bool isInitOnStart = false;
    [SerializeField] private Button close_btn;

    [FormerlySerializedAs("CameraAtoFollowToggle")] [SerializeField]
    private Toggle CameraAutoFollowToggle;

    [FormerlySerializedAs("back_to_start_btn")] [SerializeField]
    private Button backToStartBtn;

    private void Start()
    {
        if (isInitOnStart)
        {
            Init();
        }
    }

    public override void Init()
    {
        //LanguageSliderInit();
        CameraAutoFollowToggle.isOn = gameSettings.AutoGoToFocus;
        bgmSizeSilder.value = gameSettings.BgmVolume;
        soundEffectSizeSilder.value = gameSettings.SEVolume;
        viewAngleSelecter.value = gameSettings.Degree / GameSettings.MaxDegree;


        for (int i = 0; i < resoltionSelecter.options.Count; i++)
        {
            if (resoltionSelecter.options[i].text == $"{gameSettings.ScreenSize.x}X{gameSettings.ScreenSize.y}")
            {
                resoltionSelecter.value = i;
                break;
            }
        }

        resoltionSelecter.value = 0;

        isFullScreenSelecter.isOn = gameSettings.IsFullScreen;


        bgmSizeSilder.onValueChanged.AddListener(BGMSilderValueChange);
        soundEffectSizeSilder.onValueChanged.AddListener(SESilderValueChange);
        resoltionSelecter.onValueChanged.AddListener(ResoltionSelecterOnValueChange);
        isFullScreenSelecter.onValueChanged.AddListener(IsFullScreenToggleValueChange);
        viewAngleSelecter.onValueChanged.AddListener(ViewAngleSelectorSliderValueChange);
        close_btn.onClick.AddListener(ClosePanelButtonClick);
        CameraAutoFollowToggle.onValueChanged.AddListener((v => gameSettings.AutoGoToFocus = v));
        backToStartBtn?.onClick.AddListener(() =>
        {
            GameManager.Instance.Save();
            CurtainManager.Instance.UpCurtain("StartScene");
        });

        if (!isInitOnStart)
        {
            Camera.main.fieldOfView = gameSettings.FOV;
            Camera.main.transform.eulerAngles = new Vector3(gameSettings.Degree, 0, 0);
        }
    }

    private void BGMSilderValueChange(float value)
    {
        gameSettings.BgmVolume = value;
        if (AudioPlayer.Instance)
        {
            AudioPlayer.Instance.SetBGMVolume(value);
        }
    }

    private void SESilderValueChange(float value)
    {
        gameSettings.SEVolume = value;
        if (AudioPlayer.Instance)
        {
            AudioPlayer.Instance.SetSEVolume(value);
        }
    }


    private void ResoltionSelecterOnValueChange(int index)
    {
        string[] temp = resoltionSelecter.options[index].text.Split('X');
        gameSettings.ScreenSize = new Vector2Int(int.Parse(temp[0]), int.Parse(temp[1]));
    }

    private void IsFullScreenToggleValueChange(bool isOn)
    {
        gameSettings.IsFullScreen = isOn;
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
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