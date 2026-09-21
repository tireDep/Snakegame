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
    private bool hasValidReferences;    // 필수 오디오 참조 검증 완료 여부
    public bool SoundEnable => soundEnable;

    private void Awake()
    {
        soundEnable = GameSaveData.LoadSoundEnabled();
        hasValidReferences = ValidateReferences();
    }

    // 효과음 재생에 필요한 참조와 에셋을 검증하는 함수
    private bool ValidateReferences()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioManager::ValidateReferences AudioSource is required.", this);
            return false;
        }

        if (moveClip == null)
        {
            Debug.LogError("AudioManager::ValidateReferences MoveClip is required.", this);
            return false;
        }

        if (foodClip == null)
        {
            Debug.LogError("AudioManager::ValidateReferences FoodClip is required.", this);
            return false;
        }

        if (collisionClip == null)
        {
            Debug.LogError("AudioManager::ValidateReferences CollisionClip is required.", this);
            return false;
        }

        if (clearClip == null)
        {
            Debug.LogError("AudioManager::ValidateReferences ClearClip is required.", this);
            return false;
        }

        return true;
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
        if (!hasValidReferences || !soundEnable)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
