using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffersUIStartAnimation : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Animator targetAniamtor;
    [SerializeField] private CanvasGroup canvasGroup;

    public void PlayStartAnimation(float delayTime)
    {
        targetAniamtor.Play(0);
        gameObject.SetActive(true);
        StartCoroutine(StartAnimationIE(delayTime));
    }

    public void HideTarget()
    {
        targetAniamtor.SetTrigger("Close");
        canvasGroup.interactable = false;
    }

    public void HightLightTarget()
    {
        targetAniamtor.SetTrigger("HightLight");
        canvasGroup.interactable = false;
    }

    private IEnumerator StartAnimationIE(float delayTime)
    {
        canvasGroup.interactable = false;
        targetObject.gameObject.SetActive(false);
        Debug.LogWarning(delayTime);
        yield return new WaitForSeconds(delayTime);
        targetObject.gameObject.SetActive(true);
        canvasGroup.interactable = true;
    }
}