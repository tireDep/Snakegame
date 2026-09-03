using System;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private int startingLength = 3;    // 초기 길이
    
    BoardManager boardManager;
    
    private readonly Snake snake = new Snake();
    public Snake SnakePlayer => snake;
    
    private Vector2Int currentDirection;
    private Vector2Int nextDirection;
    
    [SerializeField] private SnakeView snakeView;
    
    private void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("SnakeController:: BoardManager not found!");
            return;   
        }
        
        Initialize();
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
    }
}
