using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private int startingLength = 3;    // 초기 길이
    
    private BoardManager boardManager;
    private GameManager gameManager;
    private AudioManager audioManager;
    private bool isInitialized;    // 필수 참조 검증 완료 여부
    
    private readonly Snake snake = new Snake();
    public Snake SnakePlayer => snake;
    
    private Vector2Int currentDirection;
    
    [SerializeField] private SnakeView snakeView;
    
    [SerializeField] private float moveInterval = 0.15f;
    private float moveTimer;
    private bool hasQueuedDirection;     // 한 Tick 안에서 여러 번 방향이 변경되는 것 방지
    private Vector2Int nextDirection;   // 다음 이동 시점에 반영
    
    private void Awake()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();

        isInitialized = ValidateReferences();
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

    private void Update()
    {
        if (!isInitialized || !gameManager.IsPlaying())
        {
            return;
        }

        UpdateMovement();
    }

    public bool Initialize()
    {
        if (!isInitialized)
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isInitialized || !gameManager.IsPlaying())
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
        
        audioManager?.PlayMove();
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
        if (!isInitialized)
        {
            return;    
        }
        
        currentDirection = nextDirection;
        hasQueuedDirection = false;
        
        Vector2Int newHeadPosition = snake.HeadPosition + currentDirection;
        
        // GameManager에서 이동 가능 여부와 성장 여부를 판단
        if (!gameManager.TryProcessSnakeMove(newHeadPosition, out bool willGrow))
        {
            return;
        }
        
        // 이동
        snake.Move(newHeadPosition, willGrow);
        
        // 변경된 좌표 기반으로 위치와 회전 갱신
        snakeView.Refresh(snake.Positions, currentDirection);

        // 이동 후 게임 진행 결과 처리
        gameManager.OnSnakeMoved(willGrow);
    }

    // 자기 자신 충돌 체크
    public bool CheckBodyCollision(Vector2Int newHeadPosition, bool willGrow)
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
