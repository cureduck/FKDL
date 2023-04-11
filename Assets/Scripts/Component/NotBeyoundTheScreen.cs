#region 模块信息
/***
 *    Title: "XFrameWork" NotBeyoundTheScreen.cs
 *           主题：       
 *    Description: 
 *           功能：
 *    Date:  #CreateTime#
 *    Author:  #AuthorName#
 *    Version:  1.0版本
 *    Modify Recoder: 
 */
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Obsolete("改成基于camrea的模式，所以这个代码不再适用")]
public class NotBeyoundTheScreen : MonoBehaviour
{

    private RectTransform uiTransform;
    public Canvas canvas;
    public bool followParentNode;//是否跟随父物体
    public bool followQuadrant;//是否根据当前所在象限进行设置
    private Vector2 parentLerp;
    public void Init(Canvas canvas)
    {
        this.canvas = canvas;
        uiTransform = GetComponent<RectTransform>();
        if (followParentNode)
        {
            parentLerp = transform.localPosition;
        }
    }

    void Update()
    {
        if (canvas)
        {
            AdjustPanel();
        }

    }

    private bool IsInRange(Vector2 pos,Vector2 range)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.y <= range.y && pos.x <= range.x;
    }

    public void AdjustPanel()
    {
        Debug.Log(transform.position);
        if (!canvas)
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        if (followParentNode)
        {
            if (followQuadrant)
            {
                Vector3 curRef = new Vector3(Mathf.Abs(parentLerp.x), Mathf.Abs(parentLerp.y));
                Vector3 screenCenter = new Vector3(uiTransform.rect.width / 2.0f, uiTransform.rect.height / 2.0f);
                //右(1,4)
                if (transform.parent.position.x > screenCenter.x)
                {
                    curRef.x = -curRef.x;
                }
                //左(2,3)
                else 
                {
                    //不变
                }
                //上(1,2)
                if (transform.parent.position.y > screenCenter.y)
                {
                    curRef.y = -curRef.y;

                }
                //下(3,4)
                else
                {
                    //不变
                }



                transform.position = transform.parent.position + (Vector3)curRef;
            }
            else
            {

                transform.position = transform.parent.position + (Vector3)parentLerp;
            }

        }
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenSize = new Vector2(canvasRect.rect.width * canvasRect.localScale.x, canvasRect.rect.height * canvasRect.localScale.y);
        //Debug.Log(screenSize);
        Vector3 position = transform.position;
        Vector2 panelSize = new Vector2(uiTransform.sizeDelta.x * canvas.transform.localScale.x, uiTransform.sizeDelta.y * canvas.transform.localScale.y);
        Vector2 pointOffset = uiTransform.pivot;

        Vector2 leftDownPoint = (Vector2)position + new Vector2(-panelSize.x * pointOffset.x, -panelSize.y * pointOffset.y);
        Vector2 rightUpPoint = (Vector2)position + new Vector2(panelSize.x * (1 - pointOffset.x), panelSize.y * (1 - pointOffset.y));

        //Debug.Log(leftDownPoint);
        //Debug.Log(rightUpPoint);

        //Vector2 localLeftDownPoint = (Vector2)transform.localPosition - panelSize / 2.0f;
        //Vector2 localRightUpPoint = (Vector2)transform.localPosition + panelSize / 2.0f;
        if (IsInRange(leftDownPoint, screenSize) && IsInRange(rightUpPoint, screenSize))
        {
            return;
        }
        else
        {
            Vector2 lerpValue = Vector2.zero;
            if (leftDownPoint.x < 0)
            {
                lerpValue.x -= leftDownPoint.x;
            }
            if (leftDownPoint.y < 0)
            {
                lerpValue.y -= leftDownPoint.y;
            }

            if (rightUpPoint.x > screenSize.x)
            {
                float lerp = rightUpPoint.x - screenSize.x;
                lerpValue.x -= lerp;
            }

            if (rightUpPoint.y > screenSize.y)
            {
                float lerp = rightUpPoint.y - screenSize.y;
                lerpValue.y -= lerp;
            }
            transform.position = position + (Vector3)lerpValue;
        }
    }


}