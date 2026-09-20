using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("SFX")]
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip foodClip;
    [SerializeField] private AudioClip collisionClip;
    [SerializeField] private AudioClip clearClip;

    private bool soundEnable;
    public bool SoundEnable => soundEnable;

    private void Awake()
    {
        soundEnable = GameSaveData.LoadSoundEnabled();
    }

    public void SetSoundEnable(bool isEnable)
    {
        soundEnable = isEnable;
        GameSaveData.SaveSoundEnable(isEnable);
    }

    public void PlayMove()
    {
        PlaySfx(moveClip);
    }
    
    public void PlayFood()
    {
        PlaySfx(foodClip);
    }

    public void PlayCollision()
    {
        PlaySfx(collisionClip);
    }

    public void PlayClear()
    {
        PlaySfx(clearClip);   
    }

    private void PlaySfx(AudioClip clip)
    {
        if (!soundEnable)
        {
            return;
        }
        
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
