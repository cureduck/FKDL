using Game;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CellRelicView : MonoBehaviour
{
    private const string Title = "UI_Icon_Relic_";

    [SerializeField] private Image relicIcon;
    [SerializeField] private Image activeIcon;
    [SerializeField] private Text relic_count;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;

    private RelicData relicData;

    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        activeIcon.gameObject.SetActive(false);
    }

    private void OnPointExit()
    {
        if (relicData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }
    }

    private void OnPointEnter()
    {
        if (relicData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
            {
                describe = $"{relicData.Id}_desc",
                title = relicData.Id,
                screenPosition = transform.position,
                param1 = relicData.Bp.Param1.ToString(),
            });
        }
    }

    public void SetData(RelicData relicData)
    {
        if (this.relicData != null)
        {
            this.relicData.Activated -= OnTrigger;
        }

        this.relicData = relicData;
        if (this.relicData != null)
        {
            this.relicData.Activated += OnTrigger;
        }
        //Debug.Log($"{Title}{relicData.Id}");

        relic_count.gameObject.SetActive(relicData.Counter > 1);
        relic_count.text = $"{relicData.Counter}";
        relicIcon.sprite = relicData.Bp.Icon;
        activeIcon.sprite = relicData.Bp.Icon;

        //relicIcon.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Relic, $"{Title}{relicData.Id}");
    }

    private void OnTrigger()
    {
        activeIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(true);
    }
}