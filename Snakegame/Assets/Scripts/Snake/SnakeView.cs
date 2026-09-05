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

            SnakeSegmentType segmentType = GetSegmentType(index, positions.Count);
            Sprite sprite = GetSprite(segmentType);
            segment.SetSprite(sprite);

            UpdateRotation(segment, segmentType, index, positions, headDirection);
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
    private SnakeSegmentType GetSegmentType(int index, int count)
    {
        if (index == 0)
        {
            return SnakeSegmentType.Head;
        }

        if (index == count - 1)
        {
            return SnakeSegmentType.Tail;
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
            
            case SnakeSegmentType.Body:
            default:
                return bodySprite;
        }
    }
    
    // 회전 계산 업데이트
    private void UpdateRotation(SnakeSegmentView segment, SnakeSegmentType type, int index, IReadOnlyList<Vector2Int> positions, Vector2Int headDirection)
    {
        if (type == SnakeSegmentType.Head)
        {
            segment.SetRotation(GetRotation(headDirection));
            return;
        }
        
        // 자신보다 앞에 있는 세그먼트의 위치를 기준으로 방향 결정
        Vector2Int curDirection = GetDirection(positions, index);
        segment.SetRotation(GetRotation(curDirection));
    }
    
    // 연결된 방향 반환
    private Vector2Int GetDirection(IReadOnlyList<Vector2Int> positions, int curIndex)
    {
        return positions[curIndex - 1] - positions[curIndex];
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
}
