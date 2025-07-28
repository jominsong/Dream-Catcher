using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public Sound[] sounds; // Inspector에서 추가
    private Dictionary<string, AudioClip> soundDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitSoundDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitSoundDictionary()
    {
        soundDict = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.name))
                soundDict.Add(sound.name, sound.clip);
        }
    }

    public void PlaySFX(string sfxName)
    {
        if (soundDict.ContainsKey(sfxName))
        {
            AudioClip clip = soundDict[sfxName];
            GameObject go = new GameObject(sfxName + "Sound");
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            Destroy(go, clip.length);
        }
        else
        {
            Debug.LogWarning($"'{sfxName}' 사운드가 SoundManager에 등록되어 있지 않습니다.");
        }
    }
}
