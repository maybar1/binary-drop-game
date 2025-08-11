using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;    
    public AudioSource sfxSource;      

    [Header("Audio Clips")]
    public AudioClip backgroundMusic; 
    public AudioClip successSFX;       

    [Header("UI Elements")]
    public UnityEngine.UI.Image soundButtonIcon; 
    public Sprite soundOnIcon;                 
    public Sprite soundOffIcon;                 

    private bool isMuted = false; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }


    public void PlaySuccessSFX()
    {
        if (sfxSource != null && successSFX != null)
        {
            StartCoroutine(PlaySuccessWithDuck());
        }
        else
        {
           
        }
    }

    private IEnumerator PlaySuccessWithDuck()
    {
       
        float originalVolume = musicSource.volume;
        musicSource.volume = originalVolume * 0.4f;   
        sfxSource.PlayOneShot(successSFX);   
        yield return new WaitForSeconds(successSFX.length);
        musicSource.volume = originalVolume;
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;

        if (soundButtonIcon != null)
            soundButtonIcon.sprite = isMuted ? soundOffIcon : soundOnIcon;
    }
}
