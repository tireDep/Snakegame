using System.Collections.Generic;
using UnityEngine;

public class SnakeView : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;    // 보드 관리자
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부
    
    [SerializeField] private SnakeSegmentView segmentPrefab;    // 생성할 뱀 세그먼트 프리팹
    
    [Header("Sprites")]
    [SerializeField] private Sprite headSprite;          // 뱀 머리 스프라이트
    [SerializeField] private Sprite bodySprite;          // 뱀 직선 몸통 스프라이트
    [SerializeField] private Sprite bodyCornerSprite;    // 뱀 모서리 몸통 스프라이트
    [SerializeField] private Sprite[] tailSprites;       // 뱀 꼬리 스프라이트 목록
    
    private readonly List<SnakeSegmentView> segments = new();    // 현재 표시 중인 뱀 세그먼트 목록
    
    private int tailSpriteIndex = 0;    // 현재 선택한 꼬리 스프라이트 인덱스

    // 뱀 표시에 필요한 참조를 준비하는 함수
    private void Awake()
    {
        hasValidReferences = ValidateReferences();
    }

    // 뱀 표시에 필요한 참조와 에셋을 검증하는 함수
    private bool ValidateReferences()
    {
        if (boardManager == null)
        {
            Debug.LogError("SnakeView::ValidateReferences BoardManager is required.", this);
            return false;
        }

        if (segmentPrefab == null)
        {
            Debug.LogError("SnakeView::ValidateReferences SegmentPrefab is required.", this);
            return false;
        }

        if (headSprite == null)
        {
            Debug.LogError("SnakeView::ValidateReferences HeadSprite is required.", this);
            return false;
        }

        if (bodySprite == null)
        {
            Debug.LogError("SnakeView::ValidateReferences BodySprite is required.", this);
            return false;
        }

        if (bodyCornerSprite == null)
        {
            Debug.LogError("SnakeView::ValidateReferences BodyCornerSprite is required.", this);
            return false;
        }

        if (tailSprites == null || tailSprites.Length == 0)
        {
            Debug.LogError("SnakeView::ValidateReferences At least one TailSprite is required.", this);
            return false;
        }

        for (int index = 0; index < tailSprites.Length; index++)    // 검증할 꼬리 스프라이트 인덱스
        {
            if (tailSprites[index] == null)
            {
                Debug.LogError($"SnakeView::ValidateReferences TailSprites[{index}] is required.", this);
                return false;
            }
        }

        return true;
    }
    
    // 초기 뱀 표시 상태를 준비하는 함수
    private void Start()
    {
         Initialize();
    }

    // 꼬리 스프라이트와 게임 오버 표시를 초기화하는 함수
    public bool Initialize()
    {
        if (!hasValidReferences)
        {
            return false;
        }

        tailSpriteIndex = Random.Range(0, tailSprites.Length);
        SetGameOver(false);

        return true;
    }

    // 뱀 좌표와 진행 방향에 맞춰 모든 세그먼트 표시를 갱신하는 함수
    public void Refresh(IReadOnlyList<Vector2Int> positions, Vector2Int headDirection)
    {
        if (!hasValidReferences || positions == null || positions.Count == 0)
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
            UpdateSegmentVisual(segment, segmentType, index, positions, headDirection);
        }
    }

    // 데이터의 뱀 길이에 맞춰 표시 세그먼트 수를 동기화하는 함수
    private void SyncSegmentCount(int requiredCount)
    {
        while (segments.Count < requiredCount)
        {
            SnakeSegmentView segment = Instantiate(segmentPrefab, transform);
            segments.Add(segment);
        }

        // 남는 세그먼트는 항상 마지막 요소부터 제거합니다.
        while (segments.Count > requiredCount)
        {
            int lastIndex = segments.Count - 1;
            SnakeSegmentView segment = segments[lastIndex];
            segments.RemoveAt(lastIndex);
            Destroy(segment.gameObject);
        }
    }
    
    // 위치 관계를 바탕으로 세그먼트 종류를 반환하는 함수
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
    
    // 세그먼트 종류에 맞는 스프라이트를 반환하는 함수
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
    
    // 세그먼트 종류와 이웃 방향에 맞춰 스프라이트와 회전을 갱신하는 함수
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
    
    // 몸통 세그먼트가 양쪽 이웃을 가질 수 있는 인덱스인지 확인하는 함수
    private bool IsValidBodyIndex(int index, int count)
    {
        return index > 0 && index < count - 1;
    }

    // 잘못된 몸통 세그먼트 인덱스를 기록하는 함수
    private void LogInvalidBodyIndex(int index, int count)
    {
        Debug.LogError($"SnakeView::UpdateSegmentVisual invalid body index. " + $"Index: {index}, Count: {count}");
    }

    // 직선 세그먼트 방향을 스프라이트 회전 각도로 변환하는 함수
    private float GetRotation(Vector2Int direction)
    {
        // 머리는 뱀의 진행 방향, 꼬리는 앞쪽 몸통 방향을 사용합니다.
        // 원본 스프라이트가 왼쪽 방향일 때 연결되므로 회전하지 않은 상태를 기준으로 사용합니다.

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

    // 앞뒤 방향이 서로 다른 축인지 확인하는 함수
    private bool IsCorner(Vector2Int frontDirection, Vector2Int backDirection)
    {
        bool frontHorizontal = frontDirection.x != 0;
        bool backHorizontal = backDirection.x != 0;

        return frontHorizontal != backHorizontal;
    }

    // 모서리 세그먼트의 앞뒤 방향을 회전 각도로 변환하는 함수
    private float GetCornerRotation(Vector2Int frontDirection, Vector2Int backDirection)
    {
        bool hasLeft = frontDirection == Vector2Int.left || backDirection == Vector2Int.left;
        bool hasRight = frontDirection == Vector2Int.right || backDirection == Vector2Int.right;
        bool hasUp = frontDirection == Vector2Int.up || backDirection == Vector2Int.up;
        bool hasDown = frontDirection == Vector2Int.down || backDirection == Vector2Int.down;
        
        // 모서리 원본은 왼쪽과 아래쪽 연결을 0도로 사용합니다.
        if (hasLeft && hasDown)
        {
            return 0.0f;
        }

        // 아래쪽과 오른쪽 연결은 원본을 반시계 방향으로 90도 회전합니다.
        if (hasDown && hasRight)
        {
            return 90.0f;
        }

        // 오른쪽과 위쪽 연결은 180도 회전합니다.
        if (hasRight && hasUp)
        {
            return 180.0f;
        }

        // 위쪽과 왼쪽 연결은 시계 방향으로 90도 회전합니다.
        if (hasUp && hasLeft)
        {
            return -90.0f;
        }

        Debug.LogError($"Invalid corner direction. " + $"Front: {frontDirection}, " + $"Back: {backDirection}");
        return 0.0f;
    }

    // 게임 오버 여부에 따라 모든 뱀 세그먼트 색상을 설정하는 함수
    public void SetGameOver(bool setGameOver)
    {
        if (segments == null || segments.Count == 0)
        {
            return;
        }

        foreach (SnakeSegmentView segment in segments)
        {
            if (segment == null || segment.SpriteRenderer == null)
            {
                continue;
            }
            
            segment.SpriteRenderer.color = setGameOver ? new Color(1.0f, 100.0f / 255.0f, 110.0f / 255.0f, 1.0f) : Color.white;
        }
    }
}
