using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SquarePointEnter : MonoBehaviour
{
    private static SquarePointEnter curClick;
    [SerializeField]
    private Light2D targetLight;
    [SerializeField]
    private const float targetIntensity = 2.5f;
    [SerializeField]
    private const float changeSpeed = .7f;
    [SerializeField]
    private GameObject targetMask;
    //private float curTargetIntensity = 0;

    private Tween anim;
    
    private void Start()
    {
        targetLight.intensity = 0;
    }

    private void OnMouseEnter()
    {
        if (!targetMask.activeInHierarchy)
        {
            //curTargetIntensity = targetIntensity;
            anim.Kill();
            anim = DOTween.To(Getter, Setter, targetIntensity, changeSpeed);
        }

    }


    private float Getter()
    {
        return targetLight.intensity;
    }

    private void Setter(float f)
    {
        targetLight.intensity = f;
    }
    
    private void Update()
    {
        //targetLight.intensity = Mathf.Lerp(targetLight.intensity, curTargetIntensity, changeSpeed * Time.deltaTime);
    }

    private void OnMouseExit()
    {
        if (targetMask.activeInHierarchy) return;
        
        anim.Kill();
        anim = DOTween.To(Getter, Setter, 0, changeSpeed / 1.5f)
            .OnComplete(() => Setter(0f));
        //curTargetIntensity = 0;
    }

    private void OnMouseUp()
    {
        if (targetMask.activeInHierarchy) return;
        anim.Kill();
        
        anim = DOTween.To(Getter, Setter, 0, 0.1f)
            .OnComplete(() => Setter(0f));
        
        /*if (curClick != null) 
        {
            curClick.curTargetIntensity = 0;
        }
        curTargetIntensity = targetIntensity;
        curClick = this;*/

    }

}
