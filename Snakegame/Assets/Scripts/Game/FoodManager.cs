using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;            // 보드 관리자
    [SerializeField] private SnakeController snakeController;      // 뱀 제어 컴포넌트
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부

    [Header("Prefabs")] 
    [SerializeField] private Food foodPrefabs;    // 생성할 음식 프리팹
    private Food foodView;                        // 현재 음식 표시 오브젝트
    
    private Vector2Int foodPosition;    // 음식 위치
    public bool ExistFood { get; private set; }       // 보드에 음식이 존재하는지 여부
    public Vector2Int FoodPosition => foodPosition;    // 현재 음식의 보드 좌표
    
    // 음식 관리에 필요한 참조를 준비하는 함수
    private void Awake()
    {
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

    // 음식 표시 오브젝트를 생성하고 숨기는 함수
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

    // 비어 있는 보드 위치에 음식을 배치하고 표시하는 함수
    public bool SpawnFood()
    {
        if (!hasValidReferences || foodView == null || !foodView.IsInitialized)
        {
            return false;
        }

        List<Vector2Int> emptyPositions = GetEmptyPositions();

        // 더 이상 음식을 생성할 공간이 없으면 표시를 종료합니다.
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

    // 뱀이 차지하지 않은 보드 위치를 반환하는 함수
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

    // 지정한 위치에 음식이 있는지 확인하는 함수
    public bool CheckFood(Vector2Int position)
    {
        return ExistFood && foodPosition == position;
    }

    // 현재 음식을 소비하고 숨기는 함수
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

    // 음식 상태를 초기화하고 첫 음식을 생성하는 함수
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
    
    // 음식 애니메이션의 재생 여부를 설정하는 함수
    public void SetPlayFoodAnim(bool isPlay)
    {
        if (foodView == null)
        {
            return;
        }
        
        foodView.SetPlayAnim(isPlay);
    }
}
