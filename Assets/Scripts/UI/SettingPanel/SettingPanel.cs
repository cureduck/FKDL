using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Managers;
using TMPro;
using I2.Loc;

public class SettingPanel : BasePanel<GameSettings>
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Slider bgmSizeSilder;
    [SerializeField] private Slider soundEffectSizeSilder;
    [SerializeField] private TMP_Dropdown languageSelecter;
    [SerializeField] private TMP_Dropdown resoltionSelecter;
    [SerializeField] private Toggle isFullScreenSelecter;
    [SerializeField] private Slider viewAngleSelecter;

    [SerializeField] private bool isInitOnStart = false;
    [SerializeField] private Button close_btn;

    private void Start()
    {
        if (isInitOnStart)
        {
            Init();
        }
    }

    public override void Init()
    {
        LanguageSliderInit();

        bgmSizeSilder.value = gameSettings.BgmVolume;
        soundEffectSizeSilder.value = gameSettings.SEVolume;
        viewAngleSelecter.value = gameSettings.Degree;
        bool haveFound = false;
        for (int i = 0; i < languageSelecter.options.Count; i++)
        {
            if (languageSelecter.options[i].text == gameSettings.LanguageType)
            {
                languageSelecter.value = i;
                break;
            }
        }

        if (!haveFound)
        {
            languageSelecter.value = 0;
        }


        for (int i = 0; i < resoltionSelecter.options.Count; i++)
        {
            if (resoltionSelecter.options[i].text == $"{gameSettings.ScreenSize.x}X{gameSettings.ScreenSize.y}")
            {
                resoltionSelecter.value = i;
                break;
            }
        }

        if (!haveFound)
        {
            resoltionSelecter.value = 0;
        }

        isFullScreenSelecter.isOn = gameSettings.IsFullScreen;


        bgmSizeSilder.onValueChanged.AddListener(BGMSilderValueChange);
        soundEffectSizeSilder.onValueChanged.AddListener(SESilderValueChange);
        resoltionSelecter.onValueChanged.AddListener(ResoltionSelecterOnValueChange);
        isFullScreenSelecter.onValueChanged.AddListener(IsFullScreenToggleValueChange);
        viewAngleSelecter.onValueChanged.AddListener(ViewAngleSelectorSliderValueChange);
        close_btn.onClick.AddListener(ClosePanelButtonClick);
        //Debug.LogError("chushihua!");
    }


    private void LanguageSliderInit()
    {
        var dropdown = languageSelecter;
        if (dropdown == null)
            return;

        var currentLanguage = LocalizationManager.CurrentLanguage;
        if (LocalizationManager.Sources.Count == 0) LocalizationManager.UpdateSources();
        var languages = LocalizationManager.GetAllLanguages();

        // Fill the dropdown elements
        dropdown.ClearOptions();
        dropdown.AddOptions(languages);

        dropdown.value = languages.IndexOf(currentLanguage);
        dropdown.onValueChanged.AddListener(LanguageSelectOnValueChanged);
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


    private void LanguageSelectOnValueChanged(int index)
    {
        var dropdown = languageSelecter;
        if (index < 0)
        {
            index = 0;
            dropdown.value = index;
        }

        gameSettings.LanguageType = dropdown.options[index].text;
        //LocalizationManager.CurrentLanguage = dropdown.options[index].text;
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