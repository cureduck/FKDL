using CH.ObjectPool;
using UnityEngine;

public class CellAudioPrefab : MonoBehaviour, IPoolObjectSetData
{
    [SerializeField] private AudioSource audioSource;


    public void InitOnSpawn()
    {
    }

    public void SetDataOnEnable(object data)
    {
        Args args = data as Args;
        SetData(args);
    }

    public void SetData(Args args)
    {
        audioSource.clip = args.audioClip;
        audioSource.volume = args.volume;
        audioSource.Play();
    }

    public class Args
    {
        public AudioClip audioClip;
        public float volume;
    }
}