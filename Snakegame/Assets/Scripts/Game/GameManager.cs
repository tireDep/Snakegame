using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;             // 보드 관리자
    [SerializeField] private FoodManager foodManager;               // 음식 관리자
    [SerializeField] private SnakeController snakeController;       // 뱀 제어 컴포넌트
    [SerializeField] private BoardCamera boardCamera;               // 보드 카메라
    [SerializeField] private AudioManager audioManager;             // 사운드 관리자
    private bool hasValidReferences;     // 필수 참조 검증 완료 여부
    
    private GameState gameState;                    // 현재 게임 상태
    public GameState GameState => gameState;         // 현재 게임 상태

    private int foodCount = 0;                       // 현재 음식 획득 점수
    public int FoodCount => foodCount;                // 현재 음식 획득 점수

    private int bestCount = 0;                       // 현재 보드 크기의 최고 점수
    public int BestCount => bestCount;                // 현재 보드 크기의 최고 점수
    
    private int[] lastFoodCounts = new int[Enum.GetValues(typeof(BoardSize)).Length];   // 보드 크기별 직전 점수
    public int LastFoodCount => lastFoodCounts[(int)boardSize];                         // 현재 보드 크기의 직전 점수
    
    [Header("Board Size")]
    [SerializeField] private BoardSize boardSize = BoardSize.Small;    // 현재 선택한 보드 크기
    public BoardSize BoardSize => boardSize;                              // 현재 선택한 보드 크기

    [FormerlySerializedAs("SmallBoardSize")]
    [SerializeField] private int smallBoardSize = 5;         // 소형 보드 한 변 길이
    [FormerlySerializedAs("MediumBoardSize")]
    [SerializeField] private int mediumBoardSize = 10;       // 중형 보드 한 변 길이
    [FormerlySerializedAs("LargeBoardSize")]
    [SerializeField] private int largeBoardSize = 15;        // 대형 보드 한 변 길이
    [FormerlySerializedAs("ExtarLargeBoardSize")]
    [SerializeField] private int extraLargeBoardSize = 25;   // 초대형 보드 한 변 길이
    
    
    public event Action<int> OnCountChanged;                 // 현재 점수 변경 이벤트
    public event Action<int> OnBestCountChanged;             // 최고 점수 변경 이벤트
    public event Action<GameState> OnGameStateChanged;       // 게임 상태 변경 이벤트
    public event Action<BoardSize> OnBoardSizeChanged;       // 보드 크기 변경 이벤트
    public event Action<int> OnLastFoodCountChanged;         // 직전 점수 변경 이벤트

    // 저장된 게임 데이터를 불러오는 함수
    private void Awake()
    {
        LoadGameData();
    }
    
    // 필수 참조를 검증하고 게임을 초기화하는 함수
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

    // 게임을 준비 상태와 초기 점수로 설정하는 함수
    private void Initialize()
    {
        ChangeState(GameState.Ready);
        foodCount = 0;
    }

    // 선택한 보드 크기로 새 게임을 시작하는 함수
    public void StartGame()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        ResetCount();
        
        boardSize = GetValidBoardSize(boardSize);
        int boardDimension = GetBoardDimension(boardSize);    // 생성할 정사각형 보드의 한 변 길이
        if (!boardManager.GenerateMap(boardDimension, boardDimension))
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

    // 충돌 종료 결과를 한 번 처리하는 함수
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

    // 보드 클리어 결과를 한 번 처리하는 함수
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

    // 준비 상태에서 게임을 다시 시작하는 함수
    public void RestartGame()
    {
        if (gameState != GameState.Ready)
        {
            return;
        }
        
        StartGame();
    }

    // 게임이 진행 중인지 확인하는 함수
    public bool IsPlaying()
    {
        return gameState == GameState.Playing;
    }

    // 현재 점수를 올리고 최고 점수를 갱신하는 함수
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

    // 현재 점수를 초기화하고 점수 변경을 알리는 함수
    public void ResetCount()
    {
        foodCount = 0;
        
        OnCountChanged?.Invoke(foodCount);
        OnBestCountChanged?.Invoke(bestCount);
    }
    
    // 게임 상태를 변경하고 구독자에게 알리는 함수
    public void ChangeState(GameState newState)
    {
        if (gameState == newState)
        {
            return;
        }
        
        gameState = newState;
        OnGameStateChanged?.Invoke(gameState);
    }

    // 선택 방향에 따라 보드 크기를 순환 변경하는 함수
    public void ChangeBoardSize(int selectIndex)
    {
        boardSize = GetValidBoardSize(boardSize);
        int sizeCount = Enum.GetValues(typeof(BoardSize)).Length;
        int newSizeIndex = ((int)boardSize + selectIndex) % sizeCount;    // 순환 후 선택할 보드 크기 인덱스

        // 음수 나머지를 마지막 보드 크기 인덱스로 순환시킵니다.
        if (newSizeIndex < 0)
        {
            newSizeIndex += sizeCount;
        }

        boardSize = (BoardSize)newSizeIndex;
        OnBoardSizeChanged?.Invoke(boardSize);
        
        bestCount = GameSaveData.LoadBestCount(boardSize);
        OnBestCountChanged?.Invoke(bestCount);
        
        OnLastFoodCountChanged?.Invoke(lastFoodCounts[(int)boardSize]);
        
        GameSaveData.SaveBoardSize(boardSize);
    }

    // 보드 크기 설정을 실제 한 변 길이로 변환하는 함수
    private int GetBoardDimension(BoardSize selectedBoardSize)
    {
        BoardSize validBoardSize = GetValidBoardSize(selectedBoardSize);    // 숫자 크기를 조회할 유효한 보드 크기
        switch (validBoardSize)
        {
            case BoardSize.Small:
            {
                return smallBoardSize;
            }
            case BoardSize.Medium:
            {
                return mediumBoardSize;
            }
            case BoardSize.Large:
            {
                return largeBoardSize;
            }
            case BoardSize.ExtraLarge:
            {
                return extraLargeBoardSize;
            }
        }

        return smallBoardSize;
    }

    // 잘못된 보드 크기를 소형으로 보정하는 함수
    private BoardSize GetValidBoardSize(BoardSize selectedBoardSize)
    {
        if (Enum.IsDefined(typeof(BoardSize), selectedBoardSize))
        {
            return selectedBoardSize;
        }

        Debug.LogError(
            $"GameManager::GetValidBoardSize Invalid BoardSize: {(int)selectedBoardSize}. Falling back to Small.",
            this);
        return BoardSize.Small;
    }

    // 저장된 보드 크기와 해당 최고 점수를 불러오는 함수
    private void LoadGameData()
    {
        boardSize = GameSaveData.LoadBoardSize();
        bestCount = GameSaveData.LoadBestCount(boardSize);
    }

    // 현재 최고 점수와 직전 점수를 저장하는 함수
    private void SaveLastPlayRecord()
    {
        GameSaveData.SaveBestCount(boardSize, bestCount);
        lastFoodCounts[(int)boardSize] = foodCount;
    }
}
