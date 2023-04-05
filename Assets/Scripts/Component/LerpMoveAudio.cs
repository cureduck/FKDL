using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMoveAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;


    public float targetVolume;
    public bool isOn;

    public void SetData(AudioClip audioClip)
    {
        this.audioSource.clip = audioClip;
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime);
        }
        else 
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
        }
    }
}
