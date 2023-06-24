using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CellProfView : MonoBehaviour
{
    public const string Title = "UI_Icon_Prof_";

    [SerializeField] private Image icon_img;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;
    public GameObject curSelectHightLight;
    private string profIndex;

    private void Start()
    {
        CurSelectHeightLightActive(false);
        icon_img.enabled = false;
        //icon_img.gameObject.SetActive(false);
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
    }

    public void SetData(string profIndex)
    {
        if (string.IsNullOrEmpty(profIndex))
        {
            icon_img.enabled = false;
            //icon_img.gameObject.SetActive(false);
        }
        else
        {
            this.profIndex = profIndex;
            //icon_img.gameObject.SetActive(true);
            icon_img.enabled = true;
            icon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Prof, $"{Title}{profIndex}");
        }
    }

    private void OnPointEnter()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
            { describe = $"{profIndex.ToLower()}_desc", title = profIndex, screenPosition = transform.position });
    }

    private void OnPointExit()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        WindowManager.Instance.simpleInfoItemPanel.Close();
    }

    public void CurSelectHeightLightActive(bool active)
    {
        if (curSelectHightLight)
        {
            curSelectHightLight.SetActive(active);
        }
    }
}