using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        public AudioSource Bgm;
        public AudioSource SoundEffect;

        public Dictionary<string, AudioClip> AudioClips;

        [Button]
        public void Play(string id)
        {
            if(AudioClips.TryGetValue(id, out var audioClip))
            {
                Bgm.clip = AudioClips[id];
                Bgm.Play();
            }
        }

        [Button]
        public void PlaySoundEffect(string id)
        { 
            if(AudioClips.TryGetValue(id, out var audioClip))
            {
                SoundEffect.clip = AudioClips[id];
                SoundEffect.Play();
            }
        }
    }
}