using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodManager : MonoBehaviour
{
    BoardManager boardManager;
    private SnakeController snakeController;

    [Header("Prefabs")] 
    [SerializeField] private Food foodPrefabs;
    private Food foodView;
    
    private Vector2Int foodPosition;    // 음식 위치
    public bool ExistFood { get; private set; }
    public Vector2Int FoodPosition => foodPosition;
    
    private void Awake()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("FoodManager:: BoardManager not found!");
            return;   
        }
        
        snakeController = FindAnyObjectByType<SnakeController>();
        if (snakeController == null)
        {
            Debug.LogError("FoodManager:: SnakeController not found!");
            return;
        }
    }

    private void Start()
    {
        
    }

    private void CreateFood()
    {
        if(foodPrefabs == null)
        {
            Debug.LogError("FoodManager:: FoodPrefab not found!");
            return;
        }
        
        foodView = Instantiate(foodPrefabs, transform);
        foodView.SetShow(false);
    }

    // 아이템 소환
    public bool SpawnFood()
    {
        if (foodView == null)
        {
            Debug.LogError("FoodManager::SpawnFood FoodView is null.");
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
    public void Initialize()
    {
        ExistFood = false;
        if (foodView != null)
        {
            foodView.SetShow(false);
        }
        else
        {
            CreateFood();    
        }
        
        SpawnFood();
    }
}
