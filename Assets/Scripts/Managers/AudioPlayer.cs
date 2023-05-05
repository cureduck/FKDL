using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using CH.ObjectPool;

namespace Managers
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        public const string AudioNormalAttack = "Audio_Normal_Attack";
        public const string AudioOpenChest = "Audio_Open_Chest";
        public const string AudioCrystal = "Audio_Crystal";
        public const string AudioClearRock = "Audio_Clear_Rock";
        public const string AudioGainCoin = "Audio_Gain_Coin";
        public const string AuidoUIButtonClick = "Auido_UI_ButtonClick";
        [SerializeField]
        private CellAudioPrefab cellAudioPrefab;
        private ObjectPool objectPool;
        [SerializeField]
        private AudioClip[] normalBGMs;
        [SerializeField]
        private AudioClip[] bossBGMs;

        public LerpMoveAudio Bgm;
        public LerpMoveAudio bossBgm;
        public AudioSource SoundEffect;

        
        public Dictionary<string, AudioClip> AudioClips;

        [ShowInInspector] private LinkedList<AudioClip> SEQueue;


        public GameSettings Settings;

        private void Start()
        {
            objectPool = new ObjectPool(cellAudioPrefab.gameObject);

            LoadVolumeSettings();
            SEQueue = new LinkedList<AudioClip>();

            if (normalBGMs.Length > 0)
            {
                Bgm.SetData(normalBGMs[UnityEngine.Random.Range(0, normalBGMs.Length)]);
            }
            if (bossBGMs.Length > 0) 
            {
                bossBgm.SetData(bossBGMs[UnityEngine.Random.Range(0, bossBGMs.Length)]);
            }


            AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Audio");
            for (int i = 0; i < audioClips.Length; i++)
            {
                AudioClips.Add(audioClips[i].name, audioClips[i]);
            }
            SwitchBossOrNormalBGM(true);
        }

        //[ContextMenu("!!")]
        [Button]
        public void SwitchBossOrNormalBGM(bool toNormal) 
        {
            Bgm.isOn = toNormal;
            bossBgm.isOn = !toNormal;
        }

        public void SetSEVolume(float f)
        {
            Settings.SEVolume = f;
            SoundEffect.volume = Settings.SEMute ? 0 : Settings.SEVolume;
        }
        
        public void SetBGMVolume(float f)
        {
            Settings.BgmVolume = f;
            Bgm.targetVolume = Settings.BgmMute ? 0 : Settings.BgmVolume;
            bossBgm.targetVolume = Settings.BgmMute ? 0 : Settings.BgmVolume;
        }

        public void LoadVolumeSettings()
        {
            Bgm.targetVolume = Settings.BgmMute ? 0 : Settings.BgmVolume;
            bossBgm.targetVolume = Settings.BgmMute ? 0 : Settings.BgmVolume;
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
            AudioClip curClip;

            try
            {
                if(AudioClips.TryGetValue(id, out curClip))
                {
                    GameObject cur = objectPool.CreatInstance(new CellAudioPrefab.Args { audioClip = curClip, volume = Settings.SEVolume });
                    cur.AddComponent<InvokeTrigger>().Set(curClip.length + 0.5f, () => objectPool.UnSpawnInstance(cur));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{id} played error");
                Debug.LogError(e);
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
                /*SoundEffect.clip = audioClip;
                SoundEffect.SetScheduledEndTime(SoundEffect.clip.length * times);
                SoundEffect.Play();*/
            }
        }
    }
}