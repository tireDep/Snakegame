using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodManager : MonoBehaviour
{
    BoardManager boardManager;
    private SnakeController snakeController;
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부

    [Header("Prefabs")] 
    [SerializeField] private Food foodPrefabs;
    private Food foodView;
    
    private Vector2Int foodPosition;    // 음식 위치
    public bool ExistFood { get; private set; }
    public Vector2Int FoodPosition => foodPosition;
    
    private void Awake()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        snakeController = FindAnyObjectByType<SnakeController>();

        hasValidReferences = ValidateReferences();
    }

    // 음식 관리에 필요한 참조와 프리팹을 검증하는 함수
    private bool ValidateReferences()
    {
        if (boardManager == null)
        {
            Debug.LogError("FoodManager::ValidateReferences BoardManager is required.", this);
            return false;
        }

        if (snakeController == null)
        {
            Debug.LogError("FoodManager::ValidateReferences SnakeController is required.", this);
            return false;
        }

        if (foodPrefabs == null)
        {
            Debug.LogError("FoodManager::ValidateReferences FoodPrefab is required.", this);
            return false;
        }

        return true;
    }

    private bool CreateFood()
    {
        if (!hasValidReferences)
        {
            return false;
        }
        
        foodView = Instantiate(foodPrefabs, transform);
        if (foodView == null || !foodView.IsInitialized)
        {
            return false;
        }

        foodView.SetShow(false);
        return true;
    }

    // 아이템 소환
    public bool SpawnFood()
    {
        if (!hasValidReferences || foodView == null || !foodView.IsInitialized)
        {
            return false;
        }

        List<Vector2Int> emptyPositions = GetEmptyPositions();

        // 더 이상 Food를 생성할 공간이 없음
        if (emptyPositions.Count == 0)
        {
            ExistFood = false;
            foodView.SetShow(false);

            return false;
        }
        
        int randomIndex = Random.Range(0, emptyPositions.Count);
        foodPosition = emptyPositions[randomIndex];
        
        ExistFood = true;
        foodView.SetPosition(boardManager.GridToWorld(foodPosition));
        foodView.SetShow(true);

        return true;
    }

    // 빈 위치 체크 함수
    private List<Vector2Int> GetEmptyPositions()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();
        
        for(int y = 0; y < boardManager.Height; y++)
        {
            for(int x = 0; x < boardManager.Width; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (snakeController.CheckContains(position))
                {
                    continue;
                }
                
                emptyPositions.Add(position);
            }
        }
        
        return emptyPositions;
    }

    // 아이템 체크
    public bool CheckFood(Vector2Int position)
    {
        return ExistFood && foodPosition == position;
    }

    // 아이템 소비
    public void ConsumeFood()
    {
        if (!ExistFood)
        {
            return;
        }
        
        ExistFood = false;
        if (foodView != null)
        {
            foodView.SetShow(false);
        }
    }

    // 초기화 함수
    public bool Initialize()
    {
        if (!hasValidReferences)
        {
            return false;
        }

        ExistFood = false;
        if (foodView != null && !foodView.IsInitialized)
        {
            return false;
        }

        if (foodView != null)
        {
            foodView.SetShow(false);
        }
        else
        {
            if (!CreateFood())
            {
                return false;
            }
        }
        
        if (!SpawnFood())
        {
            return false;
        }

        SetPlayFoodAnim(true);
        return true;
    }
    
    public void SetPlayFoodAnim(bool isPlay)
    {
        if (foodView == null)
        {
            return;
        }
        
        foodView.SetPlayAnim(isPlay);
    }
}
