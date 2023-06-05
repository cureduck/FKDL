using CH.ObjectPool;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class BuffListView : MonoBehaviour
{
    [SerializeField] private Transform prefabParent;
    [SerializeField] private CellBuffView cellBuffView;
    [SerializeField] private RectTransform listRect;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private bool isWorldObject = false;

    BuffAgent buffDatas;
    private UIViewObjectPool<CellBuffView, BuffData> objectPoolData;

    public void Init()
    {
        if (objectPoolData == null)
        {
            objectPoolData = new UIViewObjectPool<CellBuffView, BuffData>(cellBuffView, null);
        }
    }

    public void SetData(BuffAgent buffDatas)
    {
        this.buffDatas = buffDatas;
        objectPoolData.SetDatas(buffDatas, OnCellBuffSet, prefabParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listRect);
        if (horizontalLayoutGroup)
        {
            RectTransform rectTransform = prefabParent.GetComponent<RectTransform>();
            Vector2 curSize = rectTransform.sizeDelta;
            curSize.x = horizontalLayoutGroup.preferredWidth;
            rectTransform.sizeDelta = curSize;
            horizontalLayoutGroup.CalculateLayoutInputHorizontal();
            horizontalLayoutGroup.SetLayoutHorizontal();
        }
    }

    private void OnCellBuffSet(CellBuffView arg1, BuffData arg2)
    {
        arg1.SetData(arg2, isWorldObject);
        arg1.transform.localRotation = Quaternion.identity;
        arg1.transform.localPosition = Vector3.zero;
    }
}