using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    private bool isInitialized;    // 필수 참조 검증 완료 여부
    
    [SerializeField] private TMP_Text foodCountText;    // 아이템 개수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최대 개수 텍스트
    [SerializeField] private TMP_Text soundButtonText;
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();

        isInitialized = ValidateReferences();
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

    private void Initialize()
    {
        if (!isInitialized)
        {
            return;
        }
        
        UpdateFoodCount(gameManager.FoodCount);
        UpdateBestCount(gameManager.BestCount);

        UpdateSoundText();
    }
    
    private void OnEnable()
    {
        if (!isInitialized)
        {
            return;
        }

        // 이벤트 등록
        gameManager.OnCountChanged += UpdateFoodCount;
        gameManager.OnBestCountChanged += UpdateBestCount;

        Initialize();
    }
    
    private void OnDisable()
    {
        if (!isInitialized)
        {
            return;   
        }

        // 이벤트 해제
        gameManager.OnCountChanged -= UpdateFoodCount;
        gameManager.OnBestCountChanged -= UpdateBestCount;
    }

    private void UpdateFoodCount(int count)
    {
        if (foodCountText == null)
        {
            return;   
        }
        
        foodCountText.text = count.ToString();
    }

    private void UpdateBestCount(int count)
    {
        if (bestCountText == null)
        {
            return;   
        }
        
        bestCountText.text = count.ToString();
    }
    
    public void ToggleSoundEnable()
    {
        if (!isInitialized)
        {
            return;
        }
        
        audioManager.SetSoundEnable(!audioManager.SoundEnable);
        UpdateSoundText();
    }
    
    private void UpdateSoundText()
    {
        if (!isInitialized)
        {
            return;
        }
        
        soundButtonText.text = audioManager.SoundEnable ? "MUTE" : "UNMUTE";
    }
}
