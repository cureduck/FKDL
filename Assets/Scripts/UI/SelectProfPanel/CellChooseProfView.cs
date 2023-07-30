using I2.Loc;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CellChooseProfView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;

    public Toggle toggle;
    [SerializeField] private GameObject lockSign;

    [Header("可选")] [SerializeField] private Localize m_localize;

    private System.Action<CellChooseProfView, string, bool> cellViewClick;

    private System.Action<CellChooseProfView, string> onPointEnter;
    private System.Action<CellChooseProfView, string> onPointExit;

    private string profIndex;

    public string ProfIndex
    {
        get { return profIndex; }
    }

    private void Start()
    {
        if (pointEnterAndExit)
        {
            pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
            pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        }

        if (lockSign)
        {
            CellChoosePorfLockView cellChoosePorfLockView = lockSign.GetComponent<CellChoosePorfLockView>();
            if (cellChoosePorfLockView)
                cellChoosePorfLockView.Init();
        }

        toggle.onValueChanged.AddListener(OnClick);
    }

    public void SetData(string profIndex, System.Action<CellChooseProfView, string, bool> cellViewClick,
        System.Action<CellChooseProfView, string> onPointEnter, System.Action<CellChooseProfView, string> onPointExit,
        System.Action<string> onLockCompleteEvent = null)
    {
        this.profIndex = profIndex;
        if (m_localize)
        {
            m_localize.SetTerm(profIndex);
        }

        //Debug.Log(profIndex);
        this.cellViewClick = cellViewClick;
        this.onPointEnter = onPointEnter;
        this.onPointExit = onPointExit;
        toggle.interactable = true;

        icon.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Prof, $"{CellProfView.Title}{profIndex}");

        SetLock(true);

        if (lockSign)
        {
            CellChoosePorfLockView cellChoosePorfLockView = lockSign.GetComponent<CellChoosePorfLockView>();
            if (cellChoosePorfLockView)
                cellChoosePorfLockView.SetData(profIndex, onLockCompleteEvent);
        }
    }

    public void SetToggleInteractable(bool interactable)
    {
        toggle.interactable = interactable;
    }

    public void SetLock(bool isLock)
    {
        lockSign.gameObject.SetActive(isLock);
        if (m_localize)
        {
            if (isLock)
            {
                m_localize.SetTerm("???");
            }
            else
            {
                m_localize.SetTerm(profIndex);
            }
        }

        toggle.interactable = !isLock;
    }

    public void SetToggleOnState(bool isOn)
    {
        toggle.isOn = isOn;
    }

    private void OnClick(bool isActive)
    {
        cellViewClick?.Invoke(this, profIndex, isActive);
    }

    private void OnPointEnter()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        onPointEnter?.Invoke(this, profIndex);
        //WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { describe = $"{profIndex}职业描述", title = profIndex, screenPosition = transform.position });
    }

    private void OnPointExit()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        onPointExit?.Invoke(this, profIndex);
        //WindowManager.Instance.simpleInfoItemPanel.Close();
    }
}