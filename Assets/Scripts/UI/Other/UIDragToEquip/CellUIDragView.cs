using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CellUIDragView : MonoBehaviour,IPointerDownHandler, IPointerUpHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
	public Transform dragParent;
    public UnityEvent onLeftClick;
    public UnityEvent onRightClick;


    public System.Action<object> beginDrag;
    public System.Action<object> endDrag;

    public bool enableDrag;

    private bool haveInit = false;
    private Transform originParent;
    private float clickSpaceTime;
    private object data;

    private bool isPointDown;

    public virtual void Init(object data) 
    {
        enabled = true;
        originParent = transform.parent;
        haveInit = true;
        this.data = data;
        //Debug.Log("初始化完毕！");
    }

    public T GetData<T>() 
    {
        return (T)data;
    }

    public void SetData(object data) 
    {
        this.data = data;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("开始拖拽");
        if (!haveInit) return;
        if (!enableDrag) return;
        beginDrag?.Invoke(data);
        //Debug.Log(dragParent.name);
        transform.SetParent(dragParent, false);
        transform.GetComponent<Image>().raycastTarget = false;
        OnBeginDragEvent();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDrag) return;
        transform.position = Input.mousePosition;//(Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update() 
    {
        if (isPointDown) 
        {
            clickSpaceTime += Time.deltaTime;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.LogError("!");
        if (!haveInit) return;
        if (!enableDrag) return;
        endDrag?.Invoke(data);
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        CellUIDragReceive cellUIDragReceive = null;
        if (target != null) 
        {
            Transform cur = target.transform;
            while (cur != null)
            {
                cellUIDragReceive = cur.GetComponent<CellUIDragReceive>();
                if (cellUIDragReceive != null)
                {
                    break;
                }
                cur = cur.parent;
                //Debug.Log(cur);
            }
        }


        transform.SetParent(originParent, false);
        transform.localPosition = Vector3.zero;
        transform.GetComponent<Image>().raycastTarget = true;
        //Debug.Log("结束拖拽");
        if (cellUIDragReceive)
        {
            cellUIDragReceive.DragOnThis(this);
            OnPointUpEvent(cellUIDragReceive);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointDown = true;
        clickSpaceTime = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointDown = false;
        if (!haveInit) return;
        if (clickSpaceTime <= 0.1f)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Done!");
                onLeftClick?.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                onRightClick?.Invoke();
            }
        }
    }

    protected virtual void OnBeginDragEvent() 
    {
        
    }

    /// <summary>
    /// 当拖拽完成后（数据已经被修改后的操作），这个函数一般用于刷新面板，只会在拖拽到Recive的接收器情况触发
    /// </summary>
    /// <param name="cellUIDragReceive"></param>
    protected virtual void OnPointUpEvent(CellUIDragReceive cellUIDragReceive) 
    {
        



    }
}
