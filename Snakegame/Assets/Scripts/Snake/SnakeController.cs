using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private int startingLength = 3;    // 초기 길이
    
    BoardManager boardManager;
    FoodManager foodManager;
    
    private readonly Snake snake = new Snake();
    public Snake SnakePlayer => snake;
    
    private Vector2Int currentDirection;
    
    [SerializeField] private SnakeView snakeView;
    
    [SerializeField] private float moveInterval = 0.15f;
    private float moveTimer;
    private bool hasQueuedDirection;     // 한 Tick 안에서 여러 번 방향이 변경되는 것 방지
    private Vector2Int nextDirection;   // 다음 이동 시점에 반영
    private bool isMoving;
    
    private void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("SnakeController:: BoardManager not found!");
            return;   
        }

        foodManager = FindAnyObjectByType<FoodManager>();
        if (foodManager == null)
        {
            Debug.LogError("SnakeController:: FoodManager not found!");
            return;  
        }
        
        Initialize();
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }

        UpdateMovement();
    }

    public void Initialize()
    {
        // 리소스 방향이 왼쪽이라서 왼쪽으로 진행
        currentDirection = Vector2Int.left;
        nextDirection = currentDirection;

        Vector2Int startPosition = boardManager.GetCenterPosition();
        if (startPosition.x + startingLength > boardManager.Width)
        { 
            // todo :
            // 맵 크기 제한, 최초 시작 길이 제한 필요
            Debug.LogError("SankeController:: Initialize:: Invalid length!");
            return;
        }
        
        snake.Initialize(startPosition, startingLength, currentDirection);

        if (snakeView != null)
        {
            snakeView.Refresh(snake.Positions, currentDirection);
        }
        
        moveTimer = 0.0f;
        hasQueuedDirection = false;
        isMoving = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        Vector2 input = context.ReadValue<Vector2>();
        Vector2Int newDirection = ConvertToDirection(input);
        
        ChangeDirection(newDirection);
    }

    // InputSystem에서 전달받은 vector2를 snake가 사용하는 상하좌우 vector2int로 변환
    private Vector2Int ConvertToDirection(Vector2 input)
    {
        if( Math.Abs(input.x) > Math.Abs(input.y) )
        {
            return input.x > 0.0f ? Vector2Int.right : Vector2Int.left;
        }
        
        if(Mathf.Abs(input.y) > 0.0f)
        {
            return input.y > 0.0f ? Vector2Int.up : Vector2Int.down;
        }
        
        return Vector2Int.zero;
    }

    // 입력 방향 전환
    private void ChangeDirection(Vector2Int newDirection)
    {
        if (newDirection == Vector2Int.zero)
        {
            return;
        }

        // 이번 이동 틱에서 이미 방향 전환 예약 시, 추가 방향 입력 받지 않음
        if (hasQueuedDirection)
        {
            return;
        }
        
        // 현재 진행 방향의 반대 방향으로는 바로 이동 불가
        if (newDirection == -currentDirection)
        {
            return;
        }
        
        // 현재 진행 방향과 같은 방향일 경우 방향 전환 아님
        if (newDirection == currentDirection)
        {
            return;
        }
        
        // 입력을 즉시 적용하지 않고, 다음 이동 틱에서 적용
        nextDirection = newDirection;
        hasQueuedDirection = true;
    }

    // 이동 처리 업데이트
    private void UpdateMovement()
    {
        moveTimer += Time.deltaTime;

        if (moveTimer < moveInterval)
        {
            return;
        }

        // moveInterval 만큼 차감해 프레임 시간에 의한 이동 오차 누적 감소
        moveTimer -= moveInterval;

        Move();
    }

    // 실제 이동
    private void Move()
    {
        currentDirection = nextDirection;
        hasQueuedDirection = false;
        
        Vector2Int newHeadPosition = snake.HeadPosition + currentDirection;
        
        // todo :
        // >> Wall Collision
        // 새로운 head 위치가 board 범위를 벗어나면 임시 이동 중지. 추후 게임오버로 구현 필요
        if (!boardManager.IsInBounds(newHeadPosition))
        {
            isMoving = false;
            Debug.Log("SnakeController::Move Hit the wall!");
        }

        // 자기 몸 충돌
        bool willGrow = foodManager.CheckFood(newHeadPosition);
        if (CheckBodyCollision(newHeadPosition, willGrow))
        {
            isMoving = false;
            Debug.Log("SnakeController::Move Hit the itself!");
        }

        if (!isMoving)
        {
            snakeView.SetGameOver(true);
            return;
        }
        
        // 이동
        snake.Move(newHeadPosition, willGrow);
        
        // 변경된 좌표 기반으로 위치와 회전 갱신
        snakeView.Refresh(snake.Positions, currentDirection);

        // 이동 후 아이템 소비, 새로운 아이템 생성
        if (willGrow)
        {
            foodManager.ConsumeFood();
            foodManager.SpawnFood();
        }
    }

    // 자기 자신 충돌 체크
    private bool CheckBodyCollision(Vector2Int newHeadPosition, bool willGrow)
    {
        IReadOnlyList<Vector2Int> positions = snake.Positions;
        if (positions == null || positions.Count == 0)
        {
            return false;
        }

        int checkCount = positions.Count;
        
        // 일반 이동에서는 기존 Tail이 이번 틱에서 제거
        // 따라서 Head가 현재 Tail 위치로 돌아가는 경우는 충돌로 처리하지 않음
        if (!willGrow)
        {
            --checkCount;
        }

        for (int index = 0; index < checkCount; index++)
        {
            if (positions[index] == newHeadPosition)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool CheckContains(Vector2Int position)
    {
        return snake.CheckContains(position);
    }
}
