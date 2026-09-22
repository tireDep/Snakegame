using System.Collections.Generic;
using UnityEngine;

public class Snake
{
    private readonly List<Vector2Int> positions = new List<Vector2Int>();    // 머리부터 꼬리 순서의 세그먼트 좌표
    public IReadOnlyList<Vector2Int> Positions => positions;                 // 읽기 전용 세그먼트 좌표 목록
    public int Length => positions.Count;                                    // 현재 뱀 길이

    public Vector2Int HeadPosition    // 현재 뱀 머리 좌표
    {
        get => positions[0];
    }
    
    public Vector2Int TailPosition    // 현재 뱀 꼬리 좌표
    {
        get => positions[positions.Count - 1];
    }

    // 시작 위치와 방향을 기준으로 뱀 세그먼트를 초기화하는 함수
    public bool Initialize(Vector2Int startPosition, int length, Vector2Int direction)
    {
        positions.Clear();
        
        for(int index = 0; index < length; index++)
        {
            Vector2Int addPosition = startPosition - direction * index;
            positions.Add(addPosition);
        }
        
        return true;
    }

    // 새 머리를 추가하고 성장 여부에 따라 꼬리를 제거하는 함수
    public void Move(Vector2Int newHeadPosition, bool isGrow)
    {
        positions.Insert(0, newHeadPosition);
        
        if (!isGrow)
        {
            positions.RemoveAt(positions.Count - 1);
        }
    }
    
    // 지정한 위치가 뱀 세그먼트에 포함되는지 확인하는 함수
    public bool CheckContains(Vector2Int position)
    {
        return positions.Contains(position);
    }

    // 모든 뱀 세그먼트 좌표를 제거하는 함수
    public void Clear()
    {
        positions.Clear();
    }
}
