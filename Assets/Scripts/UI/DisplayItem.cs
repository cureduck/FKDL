using Game;
using UnityEngine;

namespace UI
{
    public abstract class DisplayItem : MonoBehaviour
    {
        public abstract void ChangeMaster(FighterData master);
    }
}