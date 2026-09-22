using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;       // 효과음을 출력할 사운드 소스
    
    [Header("SFX")]
    [SerializeField] private AudioClip moveClip;            // 뱀 이동 효과음
    [SerializeField] private AudioClip foodClip;            // 음식 획득 효과음
    [SerializeField] private AudioClip collisionClip;       // 충돌 효과음
    [SerializeField] private AudioClip clearClip;           // 게임 클리어 효과음

    private bool soundEnable;                    // 사운드 사용 여부
    private bool hasValidReferences;    // 필수 사운드 참조 검증 완료 여부
    public bool SoundEnable => soundEnable;       // 현재 사운드 사용 여부

    // 저장된 사운드 설정과 필수 참조를 준비하는 함수
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

    // 사운드 사용 여부를 변경하고 저장하는 함수
    public void SetSoundEnable(bool isEnable)
    {
        soundEnable = isEnable;
        GameSaveData.SaveSoundEnable(isEnable);
    }

    // 뱀 이동 효과음을 재생하는 함수
    public void PlayMove()
    {
        PlaySfx(moveClip);
    }
    
    // 음식 획득 효과음을 재생하는 함수
    public void PlayFood()
    {
        PlaySfx(foodClip);
    }

    // 충돌 효과음을 재생하는 함수
    public void PlayCollision()
    {
        PlaySfx(collisionClip);
    }

    // 게임 클리어 효과음을 재생하는 함수
    public void PlayClear()
    {
        PlaySfx(clearClip);   
    }

    // 사운드 설정이 켜진 경우 지정한 효과음을 재생하는 함수
    private void PlaySfx(AudioClip clip)
    {
        if (!hasValidReferences || !soundEnable)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
