using System.Collections.Generic;
using Game;
using I2.Loc;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ChooseProfMainPanel : MonoBehaviour
{
    private const string ChooseProfTitle = "UI_Icon_ChooseProf_";

    [SerializeField] private ChooseProfListView profListView;
    [Header("当前职业信息")] [SerializeField] private GameObject chooseProfInfoView;
    [SerializeField] private Localize profTitle_txt;
    [SerializeField] private Image chooseProfInfoIcon_img;
    [SerializeField] private Localize profDescribe_txt;
    [SerializeField] private CellGoodView startRelicInfo;
    [Header("当前已经选择职业")] [SerializeField] private CellProfView mainProf;
    [SerializeField] private CellProfView secondProf01;
    [SerializeField] private CellProfView secondProf02;
    [Header("按钮")] [SerializeField] private Button resetChoose_btn;
    [SerializeField] private Button startGame_btn;
    [SerializeField] private GameObject waringInfo;
    [SerializeField] private GameObject _giftsPanel;
    private string[] curCanSelectProfs;

    #region 内部自用

    private List<int> curSelectViewIndex;

    #endregion

    private string[] profDatas;

    private void Start()
    {
        Init();
        curCanSelectProfs = new string[] { "ALC", "ASS", "MAG", "BAR", "BLI", "CUR", "KNI" };
        UpdateView();
    }

    public void Init()
    {
        profListView.Init();
        resetChoose_btn.onClick.AddListener(ResetSelect);
        startGame_btn.onClick.AddListener(StartGame);
        chooseProfInfoView.gameObject.SetActive(false);
        curSelectViewIndex = new List<int>();
        profDatas = new string[4];
    }

    private void UpdateView()
    {
        profListView.SetData(curCanSelectProfs, CellClick, CellPointEnter, null, curSelectViewIndex);

        mainProf.CurSelectHeightLightActive(false);
        secondProf01.CurSelectHeightLightActive(false);
        secondProf02.CurSelectHeightLightActive(false);

        mainProf.SetData(profDatas[0]);
        secondProf01.SetData(profDatas[1]);
        secondProf02.SetData(profDatas[2]);
        Debug.Log("Gengxing!");
        if (string.IsNullOrEmpty(profDatas[0]))
        {
            mainProf.CurSelectHeightLightActive(true);
        }
        else if (string.IsNullOrEmpty(profDatas[1]))
        {
            secondProf01.CurSelectHeightLightActive(true);
        }
        else if (string.IsNullOrEmpty(profDatas[2]))
        {
            secondProf02.CurSelectHeightLightActive(true);
        }

        startGame_btn.interactable = curSelectViewIndex.Count >= 3;
        waringInfo.gameObject.SetActive(curSelectViewIndex.Count < 3);

        //Relic startRelic = RelicManager.Instance.GetById()
    }

    private void CellPointExit(CellChooseProfView arg1, string arg2)
    {
        chooseProfInfoView.gameObject.SetActive(false);
    }

    private void CellPointEnter(CellChooseProfView arg1, string arg2)
    {
        chooseProfInfoView.gameObject.SetActive(true);
        profTitle_txt.SetTerm(arg2);
        chooseProfInfoIcon_img.sprite =
            SpriteManager.Instance.GetIcon(SpriteManager.IconType.ChooseProf, $"{ChooseProfTitle}{arg2}");
        profDescribe_txt.SetTerm($"{arg2}_desc".ToLower());

        string relicId;
        Debug.Log(arg2);
        if (RelicData.ProfRelic.TryGetValue(arg2, out relicId))
        {
            Debug.Log(relicId);
            //startRelicInfo.gameObject.SetActive(true);
            Relic curRelicData = RelicManager.Instance.GetById(relicId);
            if (curRelicData != null)
            {
                startRelicInfo.SetData(0, 10, new Offer(curRelicData), null);
                startRelicInfo.gameObject.SetActive(true);
            }
            else
            {
                startRelicInfo.gameObject.SetActive(false);
            }
        }
        else
        {
            startRelicInfo.gameObject.SetActive(false);
        }
    }

    private void CellClick(CellChooseProfView cellChooseProfView, string curProfData, bool isActive)
    {
        //选择一个职业
        if (isActive)
        {
            for (int i = 0; i < profDatas.Length; i++)
            {
                if (string.IsNullOrEmpty(profDatas[i]))
                {
                    curSelectViewIndex.Add(GetProfIndexByData(curProfData));
                    profDatas[i] = curProfData;
                    break;
                }
            }
        }
        //移除一个职业
        else
        {
            for (int i = 0; i < profDatas.Length; i++)
            {
                if (profDatas[i] == curProfData)
                {
                    curSelectViewIndex.Remove(GetProfIndexByData(curProfData));
                    profDatas[i] = string.Empty;
                    break;
                }
            }
        }

        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
        UpdateView();
        //Debug.Log($"{cellChooseProfView}:{index}被点击");
    }

    private int GetProfIndexByData(string profData)
    {
        for (int i = 0; i < curCanSelectProfs.Length; i++)
        {
            if (curCanSelectProfs[i] == profData)
            {
                return i;
            }
        }

        return -1;
    }

    private void ResetSelect()
    {
        for (int i = 0; i < profDatas.Length; i++)
        {
            profDatas[i] = string.Empty;
        }

        curSelectViewIndex.Clear();
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
        UpdateView();
    }

    private void StartGame()
    {
        SecondaryData.DeleteSave();
        GameDataManager.Instance.SecondaryData = SecondaryData.GetOrCreate();
        profDatas[3] = "COM";
        GameDataManager.Instance.SecondaryData.Profs = profDatas;
        //FindObjectOfType<TransitionManager>().LoadScene("MainScene", "DiagonalRectangleGrid", .2f);
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
        _giftsPanel.SetActive(true);
    }
}