using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;
using TMPro;
using Game;

public class DamageSign : MonoBehaviour, IPoolObjectSetData
{
    public class Args 
    {
        public int value;
        public int damageType;//0：表示物理，1表示魔法，2表示真实
    }

    [SerializeField]
    private TMP_Text curSign;
    [SerializeField]
    private Color pdColor;
    [SerializeField]
    private Color mdColor;
    [SerializeField]
    private Color cdColor;

    public void InitOnSpawn()
    {
        
    }

    public void SetDataOnEnable(object data)
    {
        Args args = data as Args;
        Color targetColor;
        switch (args.damageType)
        {
            case 0:
                targetColor = pdColor;
                break;
            case 1:
                targetColor = mdColor;
                break;
            default:
                targetColor = cdColor;
                break;
        }
        curSign.color = targetColor;

        curSign.text = $"{args.value}";

    }
}
