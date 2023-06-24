#region 模块信息

/***
 *    Title: "XFrameWork" PointEnterAndExit.cs
 *           主题：       
 *    Description: 
 *           功能：
 *    Date:  #CreateTime#
 *    Author:  #AuthorName#
 *    Version:  1.0版本
 *    Modify Recoder: 
 */

#endregion

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointEnterAndExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent onPointEnter;
    public UnityEvent onPointExit;
    public UnityEvent onPointLeftDown;
    public UnityEvent onPointLeftUp;
    public UnityEvent onPointLeftClick;

    public UnityEvent onPointRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        Selectable curButton = GetComponent<Selectable>();
        if (curButton)
        {
            if (curButton.interactable == true)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    onPointLeftClick.Invoke();
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    onPointRightClick.Invoke();
                }
            }
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onPointLeftClick.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                onPointRightClick.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Selectable>())
        {
            if (GetComponent<Selectable>().interactable == true)
            {
                onPointLeftDown.Invoke();
            }
        }
        else
        {
            onPointLeftDown.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Selectable>())
        {
            if (GetComponent<Selectable>().interactable == true)
            {
                onPointEnter.Invoke();
            }
        }
        else
        {
            onPointEnter.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Selectable>())
        {
            if (GetComponent<Selectable>().interactable == true)
            {
                onPointExit.Invoke();
            }
        }
        else
        {
            onPointExit.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GetComponent<Selectable>())
        {
            if (GetComponent<Selectable>().interactable == true)
            {
                onPointLeftUp.Invoke();
            }
        }
        else
        {
            onPointLeftUp.Invoke();
        }
    }
}