using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")]
    const int DEFAULT_MAP_SIZE = 25;
    
    [SerializeField] private int width = DEFAULT_MAP_SIZE;     // 맵 넓이
    [SerializeField] private int height = DEFAULT_MAP_SIZE;   // 맵 높이
    
    [Header("Tilemap")]
    [SerializeField] private Tilemap floorTilemap;            // 맵 타일맵
    [SerializeField] private Tilemap wallTilemap;             // 벽 타일맵

    [Header("Tiles")] 
    [SerializeField] private List<TileBase> floorTiles;    // 맵 타일
    [SerializeField] private TileBase wallTile;             // 벽 타일
    
    public int Witdh => width;
    public int Height => height;

    private void Start()
    {
        GenerateMap(width, height);
    }
    
    // 타일맵 생성 함수
    public void GenerateMap(int xSize = DEFAULT_MAP_SIZE, int ySize = DEFAULT_MAP_SIZE)
    {
        // 생성 전에 기존 생성된 맵 정보들 모두 삭제
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // 맵 생성
        GenerateFloor(xSize, ySize);
        GenerateWall(xSize, ySize);
    }

    // 바닥 생성 함수
    public void GenerateFloor(int xSize, int ySize)
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // 현재 좌표 기준으로 바둑판 타일 패턴 처리
                int tileIndex = 0;  // 맵 타일 인덱스
                if (floorTiles.Count > 0)
                {
                    tileIndex = (x + y) % floorTiles.Count;  // 맵 타일 인덱스
                }
                else
                {
                    Debug.LogError("GenerateFloor:: No floor tiles in the list!");
                }
                
                floorTilemap.SetTile(tilePosition, floorTiles[tileIndex]);
            }
        }
    }

    // 벽 생성 함수
    public void GenerateWall(int xSize, int ySize)
    {
        for (int x = -1; x <= xSize; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, -1, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(x, ySize, 0), wallTile);
        }

        for (int y = -1; y <= ySize; y++)
        {
            wallTilemap.SetTile(new Vector3Int(-1, y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(xSize, y, 0), wallTile);
        }
    }

    // 내부에 있는지 확인 함수
    public bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }
    
    // 타일맵의 특정 셀 중심점 월드 좌표 반환 함수
    public Vector3 GridToWorld(Vector2Int position)
    {
        Vector3Int gridPosition = new Vector3Int(position.x, position.y, 0);
        return floorTilemap.GetCellCenterWorld(gridPosition);
    }

    // 보드의 중앙 좌표 계산 함수
    public Vector3 GetBoardCenterWorld()
    {
        Vector3 minPosition = GridToWorld(Vector2Int.zero);
        Vector3 maxPosition = GridToWorld(new Vector2Int(width - 1, height - 1));
        
        return (minPosition + maxPosition) * 0.5f;
    }

    public Vector2Int GetCenterPosition()
    {
        return new Vector2Int(width / 2, height / 2);
    }
}
