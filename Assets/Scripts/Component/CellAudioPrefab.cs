using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.ObjectPool;

public class CellAudioPrefab : MonoBehaviour, IPoolObjectSetData
{
    public class Args
    {
        public AudioClip audioClip;
        public float volume;
    }

    [SerializeField] private AudioSource audioSource;


    public void InitOnSpawn()
    {
    }

    public void SetDataOnEnable(object data)
    {
        Args args = data as Args;
        audioSource.clip = args.audioClip;
        audioSource.volume = args.volume;
        audioSource.Play();
    }
}