using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CellUIDragReceive : MonoBehaviour
{
    public void DragOnThis(CellUIDragView sender)
    {
        OnDragOnThis(sender);
    }

    protected virtual void OnDragOnThis(CellUIDragView sender)
    {
    }
}