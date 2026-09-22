using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private int startingLength = 3;    // 뱀 초기 길이
    
    [SerializeField] private BoardManager boardManager;        // 보드 관리자
    [SerializeField] private GameManager gameManager;          // 게임 관리자
    [SerializeField] private AudioManager audioManager;        // 사운드 관리자
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부
    
    private readonly Snake snake = new Snake();    // 제어할 뱀 데이터
    public Snake SnakePlayer => snake;             // 현재 제어 중인 뱀 데이터
    
    private Vector2Int currentDirection;           // 현재 뱀 이동 방향
    
    [SerializeField] private SnakeView snakeView;                  // 뱀 표시 컴포넌트
    
    [SerializeField] private float moveInterval = 0.15f;           // 뱀 이동 간격
    private float moveTimer;                                      // 다음 이동까지 누적된 시간
    private bool hasQueuedDirection;                              // 한 이동 주기 안에서 방향 변경 예약 여부
    private Vector2Int nextDirection;                             // 다음 이동에 적용할 방향
    
    // 뱀 제어에 필요한 참조를 준비하는 함수
    private void Awake()
    {
        hasValidReferences = ValidateReferences();
    }

    // 뱀 제어에 필요한 참조와 설정을 검증하는 함수
    private bool ValidateReferences()
    {
        if (boardManager == null)
        {
            Debug.LogError("SnakeController::ValidateReferences BoardManager is required.", this);
            return false;
        }

        if (gameManager == null)
        {
            Debug.LogError("SnakeController::ValidateReferences GameManager is required.", this);
            return false;
        }

        if (snakeView == null)
        {
            Debug.LogError("SnakeController::ValidateReferences SnakeView is required.", this);
            return false;
        }

        if (startingLength <= 0)
        {
            Debug.LogError("SnakeController::ValidateReferences StartingLength must be greater than zero.", this);
            return false;
        }

        if (moveInterval <= 0.0f)
        {
            Debug.LogError("SnakeController::ValidateReferences MoveInterval must be greater than zero.", this);
            return false;
        }

        return true;
    }

    // 게임 진행 중 뱀 이동 시간을 갱신하는 함수
    private void Update()
    {
        if (!hasValidReferences || !gameManager.IsPlaying())
        {
            return;
        }

        UpdateMovement();
    }

    // 뱀 데이터와 표시 상태를 시작 위치로 초기화하는 함수
    public bool Initialize()
    {
        if (!hasValidReferences)
        {
            return false;
        }
        
        // 리소스 방향이 왼쪽이라서 왼쪽으로 진행
        currentDirection = Vector2Int.left;
        nextDirection = currentDirection;

        Vector2Int startPosition = boardManager.GetCenterPosition();
        if (startPosition.x + startingLength > boardManager.Width)
        { 
            Debug.LogError("SankeController:: Initialize:: Invalid length!");
            return false;
        }
        
        snake.Initialize(startPosition, startingLength, currentDirection);

        if (!snakeView.Initialize())
        {
            return false;
        }

        snakeView.Refresh(snake.Positions, currentDirection);
        
        moveTimer = 0.0f;
        hasQueuedDirection = false;

        return true;
    }

    // 입력 시스템의 이동 입력을 방향 전환으로 처리하는 함수
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!hasValidReferences || !gameManager.IsPlaying())
        {
            return;
        }
        
        if (!context.performed)
        {
            return;
        }

        Vector2 input = context.ReadValue<Vector2>();
        Vector2Int newDirection = ConvertToDirection(input);
        
        ChangeDirection(newDirection);
    }

    // 입력 벡터를 뱀이 사용하는 상하좌우 방향으로 변환하는 함수
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

    // 유효한 입력 방향을 다음 이동 방향으로 예약하는 함수
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
        
        audioManager?.PlayMove();
    }

    // 이동 간격에 맞춰 뱀 이동을 실행하는 함수
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

    // 충돌과 성장 여부를 반영해 뱀을 한 칸 이동하는 함수
    private void Move()
    {
        if (!hasValidReferences)
        {
            return;    
        }
        
        currentDirection = nextDirection;
        hasQueuedDirection = false;
        
        Vector2Int newHeadPosition = snake.HeadPosition + currentDirection;
        
        if (!gameManager.TryProcessSnakeMove(newHeadPosition, out bool willGrow))
        {
            return;
        }
        
        snake.Move(newHeadPosition, willGrow);
        
        snakeView.Refresh(snake.Positions, currentDirection);

        gameManager.OnSnakeMoved(willGrow);
    }

    // 새 머리 위치가 이동 후 남을 몸통과 충돌하는지 확인하는 함수
    public bool CheckBodyCollision(Vector2Int newHeadPosition, bool willGrow)
    {
        IReadOnlyList<Vector2Int> positions = snake.Positions;
        if (positions == null || positions.Count == 0)
        {
            return false;
        }

        int checkCount = positions.Count;
        
        // 일반 이동에서는 기존 꼬리가 제거되므로 머리가 현재 꼬리 위치로 돌아가도 충돌하지 않습니다.
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
    
    // 게임 오버 시 뱀 표시 상태 변경 함수
    public void SetGameOverVisual(bool isGameOver)
    {
        if (snakeView == null)
        {
            return;
        }

        snakeView.SetGameOver(isGameOver);
    }

    // 특정 위치가 뱀 몸에 포함되는지 확인하는 함수
    public bool CheckContains(Vector2Int position)
    {
        return snake.CheckContains(position);
    }
}
