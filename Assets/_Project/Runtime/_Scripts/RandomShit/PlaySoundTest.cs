using System;
using UnityEngine;

public class PlaySoundTest : MonoBehaviour
{

    AudioManager audioManager;
    AudioClips audioClips;


    public ObjectPoolManager.PoolType poolType =  ObjectPoolManager.PoolType.GameObjects;


    private void Awake()
    {
        audioManager = AudioManager.Instance;
        audioClips = audioManager.audioSO;
    }


    public void PlaySoundEffect()
    {
        // Use this line in code to play audio clip
        audioManager.PlaySound(audioClips.PaperRip, 1, 1f, true, poolType);
    }


    public void PlayOnShot()
    {
        audioManager.PlayOneShot(audioClips.PaperRip, 1f, poolType);
    }
    
    
    
    
}
