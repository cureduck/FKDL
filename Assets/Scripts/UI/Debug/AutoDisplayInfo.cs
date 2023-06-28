using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class AutoDisplayInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ISimpleDataHolder _cellView;

        private void Awake()
        {
            _cellView = GetComponent<ISimpleDataHolder>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args()
            {
                title = _cellView.Id,
                describe = _cellView.Desc,
                curParams = new string[] { _cellView.Param },
                screenPosition = transform.position
            });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }
    }
}