using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectColliderPointEnterAndExit : MonoBehaviour
{
    public UnityEvent onPointEnter;
    public UnityEvent onPointExit;


    public void OnMouseEnter()
    {
        onPointEnter?.Invoke();
    }

    private void OnMouseExit()
    {
        onPointExit?.Invoke();
    }
}