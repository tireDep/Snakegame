using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;       // 게임 관리자
    [SerializeField] private AudioManager audioManager;     // 사운드 관리자
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부
    
    [SerializeField] private TMP_Text foodCountText;    // 현재 점수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최고 점수 텍스트
    [SerializeField] private TMP_Text soundButtonText;   // 사운드 버튼 텍스트
    
    // HUD 표시에 필요한 참조를 준비하는 함수
    private void Awake()
    {
        hasValidReferences = ValidateReferences();
    }

    // 게임 HUD 표시에 필요한 참조를 검증하는 함수
    private bool ValidateReferences()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameHUD::ValidateReferences GameManager is required.", this);
            return false;
        }

        if (foodCountText == null)
        {
            Debug.LogError("GameHUD::ValidateReferences FoodCountText is required.", this);
            return false;
        }

        if (bestCountText == null)
        {
            Debug.LogError("GameHUD::ValidateReferences BestCountText is required.", this);
            return false;
        }

        if (soundButtonText == null)
        {
            Debug.LogError("GameHUD::ValidateReferences SoundButtonText is required.", this);
            return false;
        }

        if (audioManager == null)
        {
            Debug.LogError("GameHUD::ValidateReferences AudioManager is required.", this);
            return false;
        }

        return true;
    }

    // 현재 점수와 사운드 설정을 HUD에 반영하는 함수
    private void Initialize()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        UpdateFoodCount(gameManager.FoodCount);
        UpdateBestCount(gameManager.BestCount);

        UpdateSoundText();
    }
    
    // HUD 이벤트를 구독하고 표시를 초기화하는 함수
    private void OnEnable()
    {
        if (!hasValidReferences)
        {
            return;
        }

        gameManager.OnCountChanged += UpdateFoodCount;
        gameManager.OnBestCountChanged += UpdateBestCount;

        Initialize();
    }
    
    // HUD 이벤트 구독을 해제하는 함수
    private void OnDisable()
    {
        if (!hasValidReferences)
        {
            return;   
        }

        gameManager.OnCountChanged -= UpdateFoodCount;
        gameManager.OnBestCountChanged -= UpdateBestCount;
    }

    // 현재 점수 텍스트를 갱신하는 함수
    private void UpdateFoodCount(int count)
    {
        if (foodCountText == null)
        {
            return;   
        }
        
        foodCountText.text = count.ToString();
    }

    // 최고 점수 텍스트를 갱신하는 함수
    private void UpdateBestCount(int count)
    {
        if (bestCountText == null)
        {
            return;   
        }
        
        bestCountText.text = count.ToString();
    }
    
    // 사운드 사용 여부를 전환하는 함수
    public void ToggleSoundEnable()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        audioManager.SetSoundEnable(!audioManager.SoundEnable);
        UpdateSoundText();
    }
    
    // 현재 사운드 설정에 맞춰 버튼 텍스트를 갱신하는 함수
    private void UpdateSoundText()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        soundButtonText.text = audioManager.SoundEnable ? "MUTE" : "UNMUTE";
    }
}
