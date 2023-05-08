using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class CellSkillViewDragReceive : CellUIDragReceive
{
    public int index;
    [SerializeField] private bool isDeleteNode;

    public bool IsDeleteNode
    {
        get => isDeleteNode;
    }

    public System.Action<CellSkillView, CellSkillViewDragReceive> onEndDrag;

    protected override void OnDragOnThis(CellUIDragView sender)
    {
        CellSkillView skillView = sender.GetData<CellSkillView>();
        if (skillView != null)
        {
            Debug.Log(name);
            onEndDrag?.Invoke(skillView, this);
        }
    }
}