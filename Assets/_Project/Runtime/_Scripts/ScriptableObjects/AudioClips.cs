using UnityEngine;

[CreateAssetMenu(fileName = "AudioClips", menuName = "Scriptable_Objects/AudioClips")]
public class AudioClips : ScriptableObject
{
    #region Audio files

        [Header("Background Music")]
        [Tooltip("Theme Music")]                    public AudioClip BackgroundMusic;
        [Tooltip("Main Menu Music")]                public AudioClip MainMenuMusic;

        [Header("Sound Effects")]
        [Tooltip("Game End")]                       public AudioClip GameEnd;
        [Tooltip("Select SFX")]                     public AudioClip Select;
        
        [Header("Paper Effects")]
        [Tooltip("Paper Crunch 1")]                 public AudioClip PaperCrunch;
        [Tooltip("Paper Rip 1")]                    public AudioClip PaperRip;

        
        [Header("Attaching SFX")]
        [Tooltip("Attaching1")]                     public AudioClip Attaching1;
        [Tooltip("Attaching2")]                     public AudioClip Attaching2;
        
        
        [Header("Wiping SFX")]
        [Tooltip("Wiping 1")]                       public AudioClip Wipe1;
        [Tooltip("Wiping 2")]                       public AudioClip Wipe2;
        [Tooltip("Screen wipe")]                    public AudioClip ScreenWipe;
        
        [Header("Pickup SFX")]
        [Tooltip("Pickup 1")]                       public AudioClip Pickup1;
        [Tooltip("Pickup 2")]                       public AudioClip Pickup2;
        [Tooltip("Pickup 3")]                       public AudioClip Pickup3;
        [Tooltip("Pickup 4")]                       public AudioClip Pickup4;
        
        
        [Header("SceneTransition")]
        [Tooltip("Fade In")]                        public AudioClip FadeIn;
        [Tooltip("Fade Out")]                       public AudioClip FadeOut;
        
        [Header("EasterEgg")]
        [Tooltip("EasterEgg")]                      public AudioClip EasterEgg;
        
    #endregion
}
