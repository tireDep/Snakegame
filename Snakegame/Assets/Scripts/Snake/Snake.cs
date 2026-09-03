using System.Collections.Generic;
using UnityEngine;

public class Snake
{
    private readonly List<Vector2Int> positions = new List<Vector2Int>();
    public IReadOnlyList<Vector2Int> Positions => positions;
    public int Length => positions.Count;

    public Vector2Int HeadPosition
    {
        get => positions[0];
    }
    
    public Vector2Int TailPosition
    {
        get => positions[positions.Count - 1];
    }

    // 초기화
    public bool Initialize(Vector2Int startPosition, int length, Vector2Int direction)
    {
        positions.Clear();
        
        for(int index = 0; index < length; index++)
        {
            Vector2Int addPosition = startPosition - direction * index;
            if (addPosition.x < 0 || addPosition.y < 0)
            {
                // todo :
                // 맵 크기 제한, 최초 시작 길이 제한 필요
                Debug.LogError("Sanke:: Initialize:: Invalid length!");
                positions.Clear();
                return false;
            }
            
            positions.Add(addPosition);
        }
        
        return true;
    }

    // 이동
    public void Move(Vector2Int newHeadPosition, bool isGrow)
    {
        // 새로운 머리 위치 추가
        positions.Insert(0, newHeadPosition);
        
        // 성장하지 않는 일반 이동이라면 기존 Tail 위치 제거해서 전체 길이 유지
        if (!isGrow)
        {
            positions.RemoveAt(positions.Count - 1);
        }
    }
    
    public bool CheckContains(Vector2Int position)
    {
        return positions.Contains(position);
    }

    public void Clear()
    {
        positions.Clear();
    }
}
