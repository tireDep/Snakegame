using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;             // 보드 관리자
    [SerializeField] private FoodManager foodManager;               // 음식 관리자
    [SerializeField] private SnakeController snakeController;       // 뱀 제어 컴포넌트
    [SerializeField] private BoardCamera boardCamera;               // 보드 카메라
    [SerializeField] private AudioManager audioManager;             // 사운드 관리자
    private bool hasValidReferences;     // 필수 참조 검증 완료 여부
    
    private GameState gameState;
    public GameState GameState => gameState;

    private int foodCount = 0;  // 아이템 획득 횟수
    public int FoodCount => foodCount;

    private int bestCount = 0;  // 최대 카운트
    public int BestCount => bestCount;
    
    private int[] lastFoodCounts = new int[Enum.GetValues(typeof(BoardSize)).Length];   // 직전 맵 별로 획득 개수
    public int LastFoodCount => lastFoodCounts[(int)boardSize];
    
    [Header("Board Size")]
    [SerializeField] private BoardSize boardSize = BoardSize.Small;
    public BoardSize BoardSize => boardSize;

    [SerializeField] private int SmallBoardSize = 5;
    [SerializeField] private int MediumBoardSize = 10;
    [SerializeField] private int LargeBoardSize = 15;
    [SerializeField] private int ExtarLargeBoardSize = 25;
    
    
    // UI 구독 함수
    public event Action<int> OnCountChanged;
    public event Action<int> OnBestCountChanged;
    public event Action<GameState> OnGameStateChanged;
    public event Action<BoardSize> OnBoardSizeChanged;
    public event Action<int> OnLastFoodCountChanged;

    private void Awake()
    {
        LoadGameData();
    }
    
    private void Start()
    {
        hasValidReferences = ValidateReferences();
        if (hasValidReferences)
        {
            Initialize();
        }
    }

    // 게임 시작에 필요한 핵심 컴포넌트를 검증하는 함수
    private bool ValidateReferences()
    {
        if (boardManager == null)
        {
            Debug.LogError("GameManager::ValidateReferences BoardManager is required.", this);
            return false;
        }

        if (foodManager == null)
        {
            Debug.LogError("GameManager::ValidateReferences FoodManager is required.", this);
            return false;
        }

        if (snakeController == null)
        {
            Debug.LogError("GameManager::ValidateReferences SnakeController is required.", this);
            return false;
        }

        if (boardCamera == null)
        {
            Debug.LogError("GameManager::ValidateReferences BoardCamera is required.", this);
            return false;
        }

        return true;
    }

    private void Initialize()
    {
        ChangeState(GameState.Ready);
        foodCount = 0;
    }

    public void StartGame()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        ResetCount();
        
        int boardSize = GetBoardSize();
        if (!boardManager.GenerateMap(boardSize, boardSize))
        {
            return;
        }

        if (!boardCamera.UpdateCamera())
        {
            return;
        }

        if (!snakeController.Initialize())
        {
            return;
        }

        if (!foodManager.Initialize())
        {
            return;
        }

        ChangeState(GameState.Playing);
    }

    public void OnGameOver()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        SaveLastPlayRecord();
        snakeController.SetGameOverVisual(true);
        foodManager.SetPlayFoodAnim(false);
        audioManager?.PlayCollision();

        ChangeState(GameState.Ready);
    }

    public void OnGameClear()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        SaveLastPlayRecord();
        foodManager.SetPlayFoodAnim(false);
        audioManager?.PlayClear();

        ChangeState(GameState.Ready);

        Debug.Log("Clear!");
        Debug.Log("count : " + foodCount + " !");
    }

    public void RestartGame()
    {
        if (gameState != GameState.Ready)
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

    // 뱀 이동 가능 여부와 성장 여부를 처리하는 함수
    public bool TryProcessSnakeMove(Vector2Int newHeadPosition, out bool willGrow)
    {
        willGrow = false;

        if (gameState != GameState.Playing)
        {
            return false;
        }

        if (boardManager == null || foodManager == null || snakeController == null)
        {
            return false;
        }

        // 보드 밖으로 이동하면 게임 오버 처리
        if (!boardManager.IsInBounds(newHeadPosition))
        {
            OnGameOver();
            return false;
        }

        // 음식 위치로 이동하는지 먼저 확인해서 성장 여부를 결정
        willGrow = foodManager.CheckFood(newHeadPosition);

        // 자기 몸과 충돌하면 게임 오버 처리
        if (snakeController.CheckBodyCollision(newHeadPosition, willGrow))
        {
            OnGameOver();
            return false;
        }

        return true;
    }

    // 뱀 이동 후 음식 소비와 클리어 여부를 처리하는 함수
    public void OnSnakeMoved(bool willGrow)
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        if (!willGrow)
        {
            return;
        }

        if (foodManager == null)
        {
            return;
        }

        // 음식 소비 처리 후 점수와 사운드 갱신
        foodManager.ConsumeFood();
        AddFoodCount();
        audioManager?.PlayFood();

        // 새 음식을 생성할 공간이 없으면 게임 클리어 처리
        if (!foodManager.SpawnFood())
        {
            OnGameClear();
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

    public void ChangeBoardSize(int selectIndex)
    {
        int sizeCount = Enum.GetValues(typeof(BoardSize)).Length;
        int newSizeIndex = (int)boardSize + selectIndex;
        
        if (newSizeIndex < 0)
        {
            newSizeIndex = sizeCount - 1;
        }
        else if (newSizeIndex >= sizeCount)
        {
            newSizeIndex = 0;
        }

        boardSize = (BoardSize)newSizeIndex;
        OnBoardSizeChanged?.Invoke(boardSize);
        
        bestCount = GameSaveData.LoadBestCount(boardSize);
        OnBestCountChanged?.Invoke(bestCount);
        
        OnLastFoodCountChanged?.Invoke(lastFoodCounts[(int)boardSize]);
        
        // 데이터 저장
        GameSaveData.SaveBoardSize(boardSize);
    }

    private int GetBoardSize()
    {
        switch (boardSize)
        {
            case BoardSize.Small:
            {
                return SmallBoardSize;
            }
            case BoardSize.Medium:
            {
                return MediumBoardSize;
            }
            case BoardSize.Large:
            {
                return LargeBoardSize;
            }
            case BoardSize.ExtraLarge:
            {
                return ExtarLargeBoardSize;
            }
        }
        
        Debug.LogError($"GameManager::GetBoardSize Invalid BoardSize: {boardSize}");
        return SmallBoardSize; 
    }

    private void LoadGameData()
    {
        boardSize = GameSaveData.LoadBoardSize();
        bestCount = GameSaveData.LoadBestCount(boardSize);
    }

    private void SaveLastPlayRecord()
    {
        GameSaveData.SaveBestCount(boardSize, bestCount);
        lastFoodCounts[(int)boardSize] = foodCount;
    }
}
