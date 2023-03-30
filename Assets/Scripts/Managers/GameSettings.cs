using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game", order = 0)]
    public class GameSettings : SerializedScriptableObject
    {
        [Range(0, 1)] public float BgmVolume;
        [Range(0, 1)] public float SEVolume;
        public bool BgmMute;
        public bool SEMute;

        
        [JsonIgnore, ShowInInspector] public float Degree
        {
            get => _degree;
            set
            {
                var r = Camera.main.transform.rotation.eulerAngles;
                r.x = value;
                Debug.Log(r);
                Camera.main.transform.rotation = Quaternion.Euler(r);
                _degree = value;
            }
        }

        private float _degree;
        
        [JsonIgnore, ShowInInspector] public float FV
        {
            get => _fv;
            set
            {
                _fv = value;
                Camera.main.fieldOfView = value;
            }
        }
        private float _fv;

    }
}