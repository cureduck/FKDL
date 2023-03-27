using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffersUIStartAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;
    [SerializeField]
    private Animator targetAniamtor;

    public void PlayStartAnimation(float delayTime) 
    {
        gameObject.SetActive(true);
        StartCoroutine(StartAnimationIE(delayTime));
    }

    public void HideTarget() 
    {
        targetAniamtor.SetTrigger("Close");
    }

    public void HightLightTarget() 
    {
        targetAniamtor.SetTrigger("HightLight");
    }

    private IEnumerator StartAnimationIE(float delayTime) 
    {
        targetObject.gameObject.SetActive(false);
        Debug.LogWarning(delayTime);
        yield return new WaitForSeconds(delayTime);
        targetObject.gameObject.SetActive(true);
    }


}
