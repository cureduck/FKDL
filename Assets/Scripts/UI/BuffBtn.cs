using Game;
using Managers;
using UnityEngine;

namespace UI
{
    public class BuffBtn : MonoBehaviour
    {
        public int Index;
        public BuffData target => GameManager.Instance.PlayerData.Buffs[Index];
    }
}