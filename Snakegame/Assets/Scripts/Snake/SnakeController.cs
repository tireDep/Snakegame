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
        currentDirection = Vector2Int.right;

        nextDirection = currentDirection;

        Vector2Int startPosition = boardManager.GetCenterPosition();
        snake.Initialize(startPosition, startingLength, currentDirection);
        
        // >>
        foreach (Vector2Int position in snake.Positions)
        {
            Debug.Log(position);
        }
        // <<
    }
}
