using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OffersUIStartAnimationGroup : MonoBehaviour
{
    private OffersUIStartAnimation[] curGroup;

    [SerializeField]
    private float spaceTime = 0.1f;

    public void Set(OffersUIStartAnimation[] offersUIStartAnimations) 
    {
        curGroup = offersUIStartAnimations;
        OnOpenAnimation();
    }

    private void OnOpenAnimation() 
    {
        for (int i = 0; i < curGroup.Length; i++)
        {
            curGroup[i].PlayStartAnimation(spaceTime * i);
        }
        //WaitForSeconds waitForSeconds = new WaitForSeconds(spaceTime);
        //for (int i = 0; i < curGroup.Length; i++)
        //{
        //    curGroup[i].gameObject.SetActive(true);
        //    yield return waitForSeconds;
        //}

    }

    public void SelectTarget(int targetIndex) 
    {
        for (int i = 0; i < curGroup.Length; i++)
        {
            if (i == targetIndex) 
            {
                curGroup[i].HightLightTarget();
            }
            else
            {
                curGroup[i].HideTarget();
            }
        }
    }


}
