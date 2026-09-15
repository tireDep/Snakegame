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
    
    // UI 구독 함수
    public event Action<int> OnCountChanged;
    
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
        gameState = GameState.Ready;
        StartGame();
    }

    private void Update()
    {
        // // >> todo
        // if (gameState == GameState.GameOver)
        // {
        //     RestartGame();
        // }
        // // <<
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

        gameState = GameState.Playing;
    }

    public void OnGameOver()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        gameState = GameState.GameOver;
        Debug.Log("Game Over!");
        Debug.Log("count : " + foodCount + " !");
    }

    public void OnGameClear()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        gameState = GameState.Clear;
        Debug.Log("Clear!");
        Debug.Log("count : " + foodCount + " !");
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
        Debug.Log("count : " + foodCount);
    }
    
    public void ResetCount()
    {
        foodCount = 0;
        OnCountChanged?.Invoke(foodCount);
    }
}
