using CH.ObjectPool;
using TMPro;
using UnityEngine;

public class DamageSign : MonoBehaviour, IPoolObjectSetData
{
    [SerializeField] private TMP_Text curSign;
    [SerializeField] private Color pdColor;
    [SerializeField] private Color mdColor;
    [SerializeField] private Color cdColor;
    [SerializeField] private GameObject cantDamageSign;

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
        cantDamageSign.gameObject.SetActive(args.value <= 0);
    }

    public class Args
    {
        public int damageType; //0：表示物理，1表示魔法，2表示真实
        public int value;
    }
}