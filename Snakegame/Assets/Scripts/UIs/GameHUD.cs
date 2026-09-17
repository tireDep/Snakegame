using System;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    private GameManager gameManager;
    
    [SerializeField] private TMP_Text foodCountText;
    
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

    private void OnEnable()
    {
        if (gameManager == null)
        {
            return;
        }

        // 이벤트 등록
        gameManager.OnCountChanged += UpdateFoodCount;
    }

    private void Start()
    {
        if (gameManager == null)
        {
            return;
        }

        UpdateFoodCount(gameManager.FoodCount);
    }
    
    private void OnDisable()
    {
        if (gameManager == null)
        {
            return;   
        }

        // 이벤트 해제
        gameManager.OnCountChanged -= UpdateFoodCount;
    }

    private void UpdateFoodCount(int count)
    {
        if (foodCountText == null)
        {
            return;   
        }

        foodCountText.text = count.ToString();
    }
}
