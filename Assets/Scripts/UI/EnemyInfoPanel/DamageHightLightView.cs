using I2.Loc;
using UnityEngine;

public class DamageHightLightView : MonoBehaviour
{
    [SerializeField] private RectTransform pdDamage_view;
    [SerializeField] private RectTransform mdDamage_View;
    [SerializeField] private RectTransform tureDamage_view;
    [SerializeField] private RectTransform posionDamage_View;
    [SerializeField] private Localize damageText;
    [SerializeField] private LocalizationParamsManager damageTextParamsManager;

    //[SerializeField]
    //private Transform startPoint;
    //[SerializeField]
    //private Transform endPoint;
    public void SetData(int curLeftHealth, int pDamage, int PDCount, int mDamage, int MDCount, int tDamage, int TDCount,
        int dif, bool isEscape)
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

        //逃跑时，不会计算任何伤害数值
        if (isEscape)
        {
            damageText.SetTerm("UI_EnemyPanel_PlayerEscapeSign"); //= $"<color=red>(玩家逃跑！)</color>";
            return;
        }

        string curPDamageInfo;
        if (pDamage > 0)
        {
            curPDamageInfo = PDCount > 1 ? $"-{pDamage}X{PDCount}({{[PHYSICS]}})" : $"-{pDamage}({{[PHYSICS]}})";
        }
        else
        {
            curPDamageInfo = string.Empty;
        }

        string curMDamageInfo;
        if (mDamage > 0)
        {
            curMDamageInfo = MDCount > 1 ? $"-{mDamage}X{MDCount}({{[MAGIC]}})" : $"-{mDamage}({{[MAGIC]}})";
        }
        else
        {
            curMDamageInfo = string.Empty;
        }

        string curTDamageInfo;
        if (tDamage > 0)
        {
            curTDamageInfo = TDCount > 1 ? $"-{tDamage}X{TDCount}({{[REAL]}})" : $"-{tDamage}({{[REAL]}})";
        }
        else
        {
            curTDamageInfo = string.Empty;
        }

        string curBuffDamageInfo;
        if (dif > 0)
        {
            curBuffDamageInfo = $"-{dif}({{[BUFF]}})";
        }
        else
        {
            if (dif < 0)
            {
                curBuffDamageInfo = $"+{-dif}({{[RECOVER]}})";
            }
            else
            {
                curBuffDamageInfo = string.Empty;
            }
            //curBuffDamageInfo = string.Empty;
        }

        string curInfo = $"{curPDamageInfo}{curMDamageInfo}{curTDamageInfo}{curBuffDamageInfo}";

        if (curLeftHealth <= totalDamage)
        {
            curInfo += "<color=red>({[DEAD]})</color>";
        }

        damageText.SetTerm(curInfo);
        damageTextParamsManager.SetParameterValue("PHYSICS", "UI_EnemyPanel_PhysicsDamage");
        damageTextParamsManager.SetParameterValue("MAGIC", "UI_EnemyPanel_MagicDamage");
        damageTextParamsManager.SetParameterValue("REAL", "UI_EnemyPanel_RealDamage");
        damageTextParamsManager.SetParameterValue("BUFF", "UI_EnemyPanel_BuffDamage");
        damageTextParamsManager.SetParameterValue("RECOVER", "UI_EnemyPanel_RecoverDamage");
        damageTextParamsManager.SetParameterValue("DEAD", "UI_EnemyPanel_DeadSign");
        //string curMDamageInfo = MDCount>1?
        //damageText.text = 
    }
}