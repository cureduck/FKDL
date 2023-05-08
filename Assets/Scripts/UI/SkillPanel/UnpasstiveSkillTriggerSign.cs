using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CH.ObjectPool;

public class UnpasstiveSkillTriggerSign : MonoBehaviour, IPoolObjectSetData
{
    [SerializeField] private Image icon;

    public void InitOnSpawn()
    {
    }

    public void SetDataOnEnable(object data)
    {
        Sprite cur = data as Sprite;
        icon.sprite = cur;
    }
}