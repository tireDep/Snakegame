using System;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    private GameManager gameManager;
    
    [SerializeField] private TMP_Text foodCountText;    // 아이템 개수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최대 개수 텍스트
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameHUD:: GameManager not found!");
            return;   
        }

        if (foodCountText == null)
        {
            Debug.LogError("GameHUD:: FoodCountText not found!");
            return;   
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (gameManager == null)
        {
            return;
        }
        
        UpdateFoodCount(gameManager.FoodCount);
        UpdateBestCount(gameManager.BestCount);
    }
    
    private void OnEnable()
    {
        if (gameManager == null)
        {
            return;
        }

        // 이벤트 등록
        gameManager.OnCountChanged += UpdateFoodCount;
        gameManager.OnBestCountChanged += UpdateBestCount;
    }
    
    private void OnDisable()
    {
        if (gameManager == null)
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
}
