using System;
using UnityEngine;

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
    
    private void Start()
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
        
        CreateFood();
        SpawnFood();
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
    public void SpawnFood()
    {
        if (boardManager == null || snakeController == null || foodView == null)
        {
            return;
        }
        
        // 보드가 모두 Snake로 차 있는 경우 대비, 무한 루프되지 않도록 처리
        int maxTryCount = boardManager.Width * boardManager.Height;

        for (int index = 0; index < maxTryCount; index++)
        {
            Vector2Int newPosition = boardManager.GetRandomPosition();
            
            // 이미 차지하고 있는 위치에는 생성하지 않음
            // todo : newPosition 가져올때 체크로 수정
            if (snakeController.CheckContains(newPosition))
            {
                continue;
            }
            
            foodPosition = newPosition;
            ExistFood = true;
            foodView.SetPosition(boardManager.GridToWorld(foodPosition));
            foodView.SetShow(true);
            
            return;
        }
        
        Debug.LogWarning("FoodManager::SpawnFood failed. No empty cell.");
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
}
