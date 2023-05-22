using TMPro;
using UnityEngine;

public class DamageHightLightView : MonoBehaviour
{
    [SerializeField] private RectTransform pdDamage_view;
    [SerializeField] private RectTransform mdDamage_View;
    [SerializeField] private RectTransform tureDamage_view;
    [SerializeField] private RectTransform posionDamage_View;
    [SerializeField] private TMP_Text damageText;

    //[SerializeField]
    //private Transform startPoint;
    //[SerializeField]
    //private Transform endPoint;
    public void SetData(int curLeftHealth, int pDamage, int PDCount, int mDamage, int MDCount, int tDamage, int TDCount,
        int dif)
    {
        if (curLeftHealth <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        //Debug.Log(curLeftHealth);
        //Debug.Log(transform.position);
        //Debug.Log(endPoint.transform.position);
        float curWidth = GetComponent<RectTransform>().rect.width;
        //Debug.Log(endPoint.transform.localPosition);
        //Debug.Log(startPoint.transform.localPosition);
        //Debug.Log(curWidth);
        int totalDamage = pDamage + mDamage + tDamage + dif;
        //Debug.Log(curWidth);
        curWidth = totalDamage / (float)curLeftHealth * curWidth;

        //Debug.Log(totalDamage / (float)curLeftHealth);
        //Debug.Log(curWidth);


        Vector2 cur = pdDamage_view.sizeDelta;
        cur.x = pDamage / (float)totalDamage * curWidth;
        pdDamage_view.sizeDelta = cur;

        cur = mdDamage_View.sizeDelta;
        cur.x = mDamage / (float)totalDamage * curWidth;
        mdDamage_View.sizeDelta = cur;

        cur = tureDamage_view.sizeDelta;
        cur.x = tDamage / (float)totalDamage * curWidth;
        tureDamage_view.sizeDelta = cur;

        cur = posionDamage_View.sizeDelta;
        cur.x = dif / (float)totalDamage * curWidth;
        posionDamage_View.sizeDelta = cur;

        string curPDamageInfo;
        if (pDamage > 0)
        {
            curPDamageInfo = PDCount > 1 ? $"-{pDamage}X{PDCount}(物理)" : $"-{pDamage}(物理)";
        }
        else
        {
            curPDamageInfo = string.Empty;
        }

        string curMDamageInfo;
        if (mDamage > 0)
        {
            curMDamageInfo = MDCount > 1 ? $"-{mDamage}X{MDCount}(魔法)" : $"-{mDamage}(魔法)";
        }
        else
        {
            curMDamageInfo = string.Empty;
        }

        string curTDamageInfo;
        if (tDamage > 0)
        {
            curTDamageInfo = TDCount > 1 ? $"-{tDamage}X{TDCount}(真实)" : $"-{tDamage}(真实)";
        }
        else
        {
            curTDamageInfo = string.Empty;
        }

        string curBuffDamageInfo;
        if (dif > 0)
        {
            curBuffDamageInfo = $"-{dif}(状态)";
        }
        else
        {
            if (dif < 0)
            {
                curBuffDamageInfo = $"+{-dif}(恢复)";
            }
            else
            {
                curBuffDamageInfo = string.Empty;
            }
        }


        damageText.text = $"{curPDamageInfo}{curMDamageInfo}{curTDamageInfo}{curBuffDamageInfo}";
        if (curLeftHealth <= totalDamage)
        {
            damageText.text += "<color=red>(致死)</color>";
        }

        //string curMDamageInfo = MDCount>1?
        //damageText.text = 
    }
}