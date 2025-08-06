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
        [Range(0f, 1f)]
        public float volume = 1f;
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
        Sound sound = System.Array.Find(sounds, s => s.name == sfxName);
        if (sound != null)
        {
            GameObject go = new GameObject(sfxName + "Sound");
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.Play();
            Destroy(go, sound.clip.length);
        }
    }

}
