using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Range(0, 1)] public float BgmVolume;
        [Range(0, 1)] public float SEVolume;
        public bool BgmMute;
        public bool SEMute;

    }
}