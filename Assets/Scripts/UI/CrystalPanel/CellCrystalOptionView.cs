using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using TMPro;
using I2.Loc;

public class CellCrystalOptionView : MonoBehaviour
{
    [SerializeField]
    private Localize optionDescribe;
    [SerializeField]
    private TMP_Text optionDescribe_txt;
    [SerializeField]
    private CanvasGroup optionDescribe_alpha;
    [SerializeField]
    private Button button;
    [SerializeField]
    private CanvasGroup outLineObject;


    private System.Action<PlayerData, Crystal.Option> onOptionClick;

    private PlayerData playerData;
    private Crystal.Option option;

    private float targetOutLineAlpha;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        outLineObject.alpha = 0;
    }


    void Update()
    {
        outLineObject.alpha = Mathf.Lerp(outLineObject.alpha, targetOutLineAlpha, Time.deltaTime * 10);
    }

    private void OnEnable()
    {
        targetOutLineAlpha = 0;
        outLineObject.alpha = 0;
    }

    public void SetData(PlayerData playerData,string craystalID,Crystal.Option option,System.Action<PlayerData,Crystal.Option> onOptionClick) 
    {
        this.onOptionClick = onOptionClick;
        this.playerData = playerData;
        this.option = option;

        optionDescribe.SetTerm($"{craystalID}_{option.Line}");
        if (playerData.CanAfford(option.CostInfo,out var info))
        {
            button.interactable = true;
            optionDescribe_alpha.alpha = 1;
        }
        else 
        {
            
            button.interactable = false;
            optionDescribe_alpha.alpha = 0.5f;
            
            optionDescribe_txt.text += $"(<color=yellow>{LocalizationManager.GetTranslation(info.ToString())}</color>)";
        }
    }

    private void OnClick() 
    {
        onOptionClick?.Invoke(playerData, option);
    }



    public void PointEnter() 
    {
        targetOutLineAlpha = 1;
    }

    public void PointExit() 
    {
        targetOutLineAlpha = 0;
    }
}
