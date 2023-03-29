using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SquarePointEnter : MonoBehaviour
{
    private static SquarePointEnter curClick;
    [SerializeField]
    private Light2D targetLight;
    [SerializeField]
    private float targetIntensity = 1.2f;
    [SerializeField]
    private float changeSpeed = 1.0f;
    [SerializeField]
    private GameObject targetMask;
    private float curTargetIntensity = 0;


    private void Start()
    {
        targetLight.intensity = 0;
    }

    private void OnMouseEnter()
    {
        if (!targetMask.activeInHierarchy)
        {
            curTargetIntensity = targetIntensity;
        } 

    }

    private void Update()
    {
        targetLight.intensity = Mathf.Lerp(targetLight.intensity, curTargetIntensity, changeSpeed * Time.deltaTime);
    }

    private void OnMouseExit()
    {
        if (targetMask.activeInHierarchy) return;

        curTargetIntensity = 0;
    }

    private void OnMouseUp()
    {
        if (targetMask.activeInHierarchy) return;

        if (curClick != null) 
        {
            curClick.curTargetIntensity = 0;
        }
        curTargetIntensity = targetIntensity;
        curClick = this;

    }

}
