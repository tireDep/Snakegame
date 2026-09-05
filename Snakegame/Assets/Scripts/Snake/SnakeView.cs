using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeView : MonoBehaviour
{
    BoardManager boardManager;
    
    [SerializeField] private SnakeSegmentView segmentPrefab;
    
    [Header("Sprites")]
    [SerializeField] private Sprite headSprite;
    [SerializeField] private Sprite bodySprite;
    [SerializeField] private Sprite bodyCornerSprite;
    [SerializeField] private Sprite[] tailSprites;
    
    private readonly List<SnakeSegmentView> segments = new();
    
    private Int32 tailSpriteIndex = 0;
    
    private void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("SnakeView:: BoardManager not found!");
            return;   
        }

        tailSpriteIndex = Random.Range(0, tailSprites.Length); 
    }

    // 새로고침
    public void Refresh(IReadOnlyList<Vector2Int> positions, Vector2Int headDirection)
    {
        if (positions == null || positions.Count == 0)
        {
            return;
        }
        
        SyncSegmentCount(positions.Count);

        for (int index = 0; index < positions.Count; index++)
        {
            SnakeSegmentView segment = segments[index];
            
            Vector3 worldPosition = boardManager.GridToWorld(positions[index]);
            segment.SetPosition(worldPosition);

            SnakeSegmentType segmentType = GetSegmentType(index, positions);
            Sprite sprite = GetSprite(segmentType);
            segment.SetSprite(sprite);

            UpdateSegmentVisual(segment, segmentType, index, positions, headDirection);
        }
    }

    // 세그먼트 수 동기화
    private void SyncSegmentCount(int requiredCount)
    {
        while (segments.Count < requiredCount)
        {
            SnakeSegmentView segment = Instantiate(segmentPrefab, transform);
            segments.Add(segment);
        }

        while (segments.Count > requiredCount)
        {
            int lastIndex = segments.Count - 1;
            Destroy(segments[lastIndex].gameObject);
            segments.RemoveAt(lastIndex);
        }
    }
    
    // 세그먼트 타입 반환
    private SnakeSegmentType GetSegmentType(int index, IReadOnlyList<Vector2Int> positions)
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("SnakeView::GetSegmentType position invalid!");
            return SnakeSegmentType.Body;
        }

        if (index < 0 || index >= positions.Count)
        {
            Debug.LogError("SnakeView::GetSegmentType index invalid!");
            return SnakeSegmentType.Body;
        }
        
        if (index == 0)
        {
            return SnakeSegmentType.Head;
        }

        if (index == positions.Count - 1)
        {
            return SnakeSegmentType.Tail;
        }
        
        Vector2Int current = positions[index];
        Vector2Int frontDir = positions[index - 1] - current;
        Vector2Int backDir = positions[index + 1] - current;

        if (IsCorner(frontDir, backDir))
        {
            return SnakeSegmentType.BodyCorner;
        }

        return SnakeSegmentType.Body;
    }
    
    // 이미지 반환
    private Sprite GetSprite(SnakeSegmentType type)
    {
        switch (type)
        {
            case SnakeSegmentType.Head:
                return headSprite;

            case SnakeSegmentType.Tail:
            {
                Sprite tailSprite = tailSprites[tailSpriteIndex];
                return tailSprite;
            }
            
            case SnakeSegmentType.BodyCorner:
                return bodyCornerSprite;
            
            case SnakeSegmentType.Body:
            default:
                return bodySprite;
        }
    }
    
    // 회전 계산 업데이트
    private void UpdateSegmentVisual(SnakeSegmentView segment, SnakeSegmentType type, int index, IReadOnlyList<Vector2Int> positions, Vector2Int headDirection)
    { 
        if (segment == null)
        {
            Debug.LogError("SnakeView::UpdateSegmentVisual segment is null.");
            return;
        }

        if (positions == null || index < 0 || index >= positions.Count)
        {
            Debug.LogError($"SnakeView::UpdateSegmentVisual invalid index. " + $"Index: {index}, Count: {positions?.Count ?? 0}");
            return;
        }

        float rotation = 0.0f;
        switch (type)
        {
            case SnakeSegmentType.Head:
            {
                rotation = GetRotation(headDirection);
                break;
            }
            case SnakeSegmentType.Body:
            {
                if (!IsValidBodyIndex(index, positions.Count))
                {
                    LogInvalidBodyIndex(index, positions.Count);
                    return;
                }
        
                Vector2Int direction = positions[index - 1] - positions[index];
                rotation = GetRotation(direction);
                
                break;
            }
            case SnakeSegmentType.BodyCorner:
            {
                if (!IsValidBodyIndex(index, positions.Count))
                {
                    LogInvalidBodyIndex(index, positions.Count);
                    return;
                }
        
                Vector2Int current = positions[index];
                Vector2Int frontDirection = positions[index - 1] - current;
                Vector2Int backDirection = positions[index + 1] - current;

                rotation = GetCornerRotation(frontDirection, backDirection);
        
                break;
            }
            case SnakeSegmentType.Tail:
            {
                if (index <= 0)
                {
                    Debug.LogError($"SnakeView::UpdateSegmentVisual invalid tail index. " + $"Index: {index}");
                    return;
                }
        
                Vector2Int direction = positions[index - 1] - positions[index];
                rotation = GetRotation(direction);
                
                break;
            }
            default:
            {
                Debug.LogError($"SnakeView::UpdateSegmentVisual invalid type: {type}");
                return;
            }
        }
        
        segment.SetSprite(GetSprite(type));
        segment.SetRotation(rotation);
    }
    
    private bool IsValidBodyIndex(int index, int count)
    {
        return index > 0 && index < count - 1;
    }

    private void LogInvalidBodyIndex(int index, int count)
    {
        Debug.LogError($"SnakeView::UpdateSegmentVisual invalid body index. " + $"Index: {index}, Count: {count}");
    }

    private float GetRotation(Vector2Int direction)
    {
        // Head/Tail Sprite 기본 연결 방향 : Left(←)
        // Head : direction은 Snake의 진행 방향
        // Tail : direction은 Tail에서 앞쪽 Body가 위치한 방향
        //
        // 원본 Sprite가 [Head][Body][Tail] 형태로 연결되도록 제작되어 있으므로
        // Left 방향일 때 회전하지 않은 상태(0도)를 기준으로 사용

        if (direction == Vector2Int.left)
        {
            return 0.0f;
        }

        if (direction == Vector2Int.up)
        {
            return -90.0f;
        }

        if (direction == Vector2Int.right)
        {
            return 180.0f;
        }

        if (direction == Vector2Int.down)
        {
            return 90.0f;
        }

        return 0.0f;
    }

    private void UpdateBodyVisual(SnakeSegmentView segment, SnakeSegmentType type, IReadOnlyList<Vector2Int> positions, int index)
    {
        if (positions == null || index <= 0 || index >= positions.Count - 1)
        {
            Debug.LogError($"Invalid body index. Index: {index}, " + $"Count: {positions?.Count ?? 0}");
            return;
        }
        
        Vector2Int current = positions[index];
        Vector2Int frontDirection = positions[index - 1] - current;
        Vector2Int backDirection = positions[index + 1] - current;

        if (IsStraight(frontDirection, backDirection))
        {
            segment.SetSprite(GetSprite(type));

            if (frontDirection.x != 0)
            {
                // 좌우 연결 : 가로
                segment.SetRotation(0.0f);
            }
            else
            {
                // 상하 연결 : 세로
                segment.SetRotation(90.0f);
            }

            return;
        }
        
        // 앞, 뒤 방향이 서로 다른 축이면 코너
        segment.SetSprite(GetSprite(type));
        segment.SetRotation(GetCornerRotation(frontDirection, backDirection));
    }

    private bool IsStraight(Vector2Int frontDirection, Vector2Int backDirection)
    {
        bool horizontal = frontDirection.x != 0 && backDirection.x != 0;
        bool vertical = frontDirection.y != 0 && backDirection.y != 0;
        
        return horizontal || vertical;
    }

    private bool IsCorner(Vector2Int frontDirection, Vector2Int backDirection)
    {
        bool frontHorizontal = frontDirection.x != 0;
        bool backHorizontal = backDirection.x != 0;

        return frontHorizontal != backHorizontal;
    }

    private float GetCornerRotation(Vector2Int frontDirection, Vector2Int backDirection)
    {
        bool hasLeft = frontDirection == Vector2Int.left || backDirection == Vector2Int.left;
        bool hasRight = frontDirection == Vector2Int.right || backDirection == Vector2Int.right;
        bool hasUp = frontDirection == Vector2Int.up || backDirection == Vector2Int.up;
        bool hasDown = frontDirection == Vector2Int.down || backDirection == Vector2Int.down;
        
        // Corner 원본 기준 : Left + Down = 0도
        if (hasLeft && hasDown)
        {
            return 0.0f;
        }

        // 원본을 반시계 방향으로 90도 회전
        // Down + Right
        if (hasDown && hasRight)
        {
            return 90.0f;
        }

        // 180도 회전
        // Right + Up
        if (hasRight && hasUp)
        {
            return 180.0f;
        }

        // 시계 방향 90도 회전
        // Up + Left
        if (hasUp && hasLeft)
        {
            return -90.0f;
        }

        Debug.LogError($"Invalid corner direction. " + $"Front: {frontDirection}, " + $"Back: {backDirection}");
        return 0.0f;
    }
}
