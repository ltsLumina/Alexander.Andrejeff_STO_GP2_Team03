using System;
using System.Collections;
using Lumina.Essentials.Modules.Singleton;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    #region Components

        public AudioClips audioSO;
        [SerializeField] GameObject audioPrefab;
        private bool canPlay = true;

    #endregion
    
    
    private void Start()
    {
        if (audioSO == null || audioPrefab == null)
        {
            Debug.LogWarning("Does not have an audio scriptable object and/or audio prefab");
            canPlay = false;
        }
    }

    /// <summary>
    /// Plays oneShot of audio clip
    /// </summary>
    /// <param name="clip">Audio clip</param>
    /// <param name="objectPool">The type of sound category SFX,MUSIC,OTHER</param>
    /// <returns>gameObject of the audio source</returns>
    public GameObject PlayOneShot(AudioClip clip, float volume = 1f, ObjectPoolManager.PoolType objectPool = ObjectPoolManager.PoolType.SFX)
        {
            if (!canPlay)
            {
                Debug.LogWarning("Does not have an audio scriptable object and/or audio prefab");
                return null;
            }
            
            var soundObject = AddToPool(audioPrefab, objectPool);
            var source = soundObject.GetComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.PlayOneShot(clip);
            StartCoroutine(WaitForEndOfAudio(source, soundObject, objectPool));

            return soundObject;
        }

    /// <summary>
    /// Creates a sound object to keep the audio in separate object; this is for looping audio
    /// </summary>
    /// <param name="clip">Audio Clip</param>
    /// <param name="volume">Volume: default 1f</param>
    /// <param name="pitch">Pitch: default 1f</param>
    /// <param name="loop">Loop audio</param>
    /// <param name="objectPool">The type of sound category SFX,MUSIC,GameObject</param>
        public GameObject PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, ObjectPoolManager.PoolType objectPool = ObjectPoolManager.PoolType.SFX)
        {
            if (!canPlay)
            {
                Debug.LogWarning("Does not have an audio scriptable object and/or audio prefab");
                return null;
            }
            
            var soundObject = AddToPool(audioPrefab, objectPool);
            var source = soundObject.GetComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;

            source.Play();
            if(!loop)
                StartCoroutine(WaitForEndOfAudio(source, soundObject, objectPool));

            return soundObject;
        }
    private GameObject AddToPool(GameObject audioObject, ObjectPoolManager.PoolType objectPool)
    {
        var soundObject = ObjectPoolManager.SpawnObject(audioObject, transform.position, Quaternion.identity, objectPool);
        return soundObject;
    }

    private IEnumerator WaitForEndOfAudio(AudioSource source, GameObject audioObject, ObjectPoolManager.PoolType objectPool)
    {

        while (source.isPlaying)        // wait for the audio clip to end
        {
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(audioObject, objectPool);

    }

}
