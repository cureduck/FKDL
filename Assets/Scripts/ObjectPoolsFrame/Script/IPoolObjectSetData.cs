using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.ObjectPool
{
    public interface IPoolObjectSetData
    {
        void InitOnSpawn();

        void SetDataOnEnable(object data);
    }
}