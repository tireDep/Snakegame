using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BoardManager boardManager;
    private FoodManager foodManager;
    private SnakeController snakeController;
    private BoardCamera boardCamera;
    
    private GameState gameState;
    public GameState GameState => gameState;

    private int foodCount = 0;  // 아이템 획득 횟수
    public int FoodCount => foodCount;

    private int bestCount = 0;  // 최대 카운트
    public int BestCount => bestCount;
    
    // UI 구독 함수
    public event Action<int> OnCountChanged;
    public event Action<int> OnBestCountChanged;
    public event Action<GameState> OnGameStateChanged;
    
    private void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardCamera:: BoardManager not found!");
            return;   
        }

        foodManager = FindAnyObjectByType<FoodManager>();
        if (foodManager == null)
        {
            Debug.LogError("BoardCamera:: FoodManager not found!");
            return;  
        }

        snakeController = FindAnyObjectByType<SnakeController>();
        if (snakeController == null)
        {
            Debug.LogError("BoardCamera:: SnakeController not found!");
            return;
        }
        
        boardCamera = FindAnyObjectByType<BoardCamera>();
        if (boardCamera == null)
        {
            Debug.LogError("BoardCamera:: BoardCamera not found!");
            return;   
        }

        Initialize();
    }

    private void Initialize()
    {
        ChangeState(GameState.Ready);
        foodCount = 0;
    }

    public void StartGame()
    {
        if (boardManager == null || foodManager == null || snakeController == null || boardCamera == null)
        {
            return;
        }
        
        boardManager.GenerateMap();
        boardCamera.UpdateCamera();
        snakeController.Initialize();
        foodManager.Initialize();
        ResetCount();

        ChangeState(GameState.Playing);
    }

    public void OnGameOver()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        // ChangeState(GameState.GameOver);
        ChangeState(GameState.Ready);
    }

    public void OnGameClear()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        ChangeState(GameState.Clear);
        Debug.Log("Clear!");
        Debug.Log("count : " + foodCount + " !");
        // todo : state.ready로 변경 + 사운드 추가 or clear 화면 제작
    }
    
    public void RestartGame()
    {
        if (gameState != GameState.GameOver)
        {
            return;
        }
        
        StartGame();
    }

    public bool IsPlaying()
    {
        return gameState == GameState.Playing;
    }

    public void AddFoodCount()
    {
        foodCount++;
        OnCountChanged?.Invoke(foodCount);

        if (foodCount > bestCount)
        {
            bestCount = foodCount;
            OnBestCountChanged?.Invoke(bestCount);
        }
    }
    
    public void ResetCount()
    {
        foodCount = 0;
        OnCountChanged?.Invoke(foodCount);
        OnBestCountChanged?.Invoke(bestCount);
    }
    
    public void ChangeState(GameState newState)
    {
        if (gameState == newState)
        {
            return;
        }
        
        gameState = newState;
        OnGameStateChanged?.Invoke(gameState);
    }
}
