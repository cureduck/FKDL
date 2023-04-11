using System;
using UnityEngine.PlayerLoop;

namespace UI
{
    public interface IUpdateable
    {
        event Action OnUpdated;
        event Action OnDestroy;
    }
}