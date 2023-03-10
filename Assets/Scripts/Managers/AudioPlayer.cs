using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        public AudioSource Bgm;
        public AudioSource SoundEffect;

        public Dictionary<string, AudioClip> AudioClips;

        [ShowInInspector] private LinkedList<AudioClip> SEQueue;
        
        
        private GameSettings Settings => SettingManager.Instance.GameSettings;

        private void Start()
        {
            LoadVolumeSettings();
            SEQueue = new LinkedList<AudioClip>();
        }



        public void LoadVolumeSettings()
        {
            Bgm.volume = Settings.BgmMute ? 0 : Settings.BgmVolume;
            SoundEffect.volume = Settings.SEMute ? 0 : Settings.SEVolume;
        }

        IEnumerator Play()
        {
            if (!SoundEffect.isPlaying)
            {
                while (SEQueue.Count >= 0)
                {
                    if (SEQueue.First == null)
                    {
                        yield break;
                    }
                    var head = SEQueue.First.Value;
                    SEQueue.RemoveFirst();
                    SoundEffect.clip = head;
                    SoundEffect.Play();
                    yield return new WaitForSeconds(head.length);
                }
            }
        }
        
        
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
        public void PlaySoundEffect(string id, int times = 1)
        {
            if(AudioClips.TryGetValue(id, out var audioClip))
            {
                for (int i = 0; i < times; i++)
                {
                    SEQueue.AddLast(audioClip);
                }

                StartCoroutine(Play());
                SoundEffect.clip = audioClip;
                SoundEffect.SetScheduledEndTime(SoundEffect.clip.length * times);
                SoundEffect.Play();
            }
        }
    }
}