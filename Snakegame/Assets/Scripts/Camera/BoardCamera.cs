using UnityEngine;
using UnityEngine.UI;

public class BoardCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private RectTransform boardViewport;               // 화면 표시 영역
    [SerializeField] private float boardPadding = 2.0f;                 // 카메라 패딩
    [SerializeField] private Vector2 cameraOffset = Vector2.zero;       // 카메라 오프셋
 
    BoardManager boardManager;
    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardCamera:: BoardManager not found!");
            return;   
        }
        
        UpdateCamera();
    }

    // 카메라 업데이트
    public void UpdateCamera()
    {
        if (boardManager == null || targetCamera == null || boardViewport == null)
        {
            return;   
        }
        
        UpdatePosition();
        UpdateSize();
    }

    // 위치 업데이트
    private void UpdatePosition()
    {
        Vector3 boardCenter = boardManager.GetBoardCenterWorld();
        
        Vector2 viewportOffset = GetViewportWorldOffset();
        
        transform.position = new Vector3( 
            boardCenter.x - viewportOffset.x + cameraOffset.x,
            boardCenter.y - viewportOffset.y + cameraOffset.y,
            transform.position.z);
    }
    
    // 크기 업데이트
    private void UpdateSize()
    {
        float boardWidth = boardManager.Witdh;
        float boardHeight = boardManager.Height;

        // 여백 추가
        boardWidth += boardPadding;
        boardHeight += boardPadding;
        
        Rect viewportRect = boardViewport.rect;
        if( viewportRect.width <= 0.0f || viewportRect.height <= 0.0f ) 
            return;
        
        // boardViewport 가로 세로 비율
        float viewportAspect = viewportRect.width / viewportRect.height;
        
        // 세로 기준으로 필요한 Orthographic Size
        float verticalSize = boardHeight * 0.5f;
        
        // 가로 기준으로 필요한 Orthographic Size
        float horizontalSize = (boardWidth * 0.5f) / viewportAspect;

        // 둘 중 더 큰 값 사용으로 전체 화면이 들어오도록 처리
        targetCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }

    // 비율 계산 반환
    private Vector2 GetViewportWorldOffset()
    {
        Canvas canvas = boardViewport.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return Vector2.zero;  
        }
        
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect == null)
        {
            return Vector2.zero; 
        }
        
        // Canvas의 중심과 BoardViewport 중심 사이의 화면상 위치 차이를 계산
        Vector3 canvasCenterWorld = canvasRect.TransformPoint(canvasRect.rect.center);
        Vector3 viewportCenterWorld = boardViewport.TransformPoint(boardViewport.rect.center);
        
        Vector3 screenCanvasCenter = RectTransformUtility.WorldToScreenPoint(null, canvasCenterWorld);
        Vector3 screenViewportCenter = RectTransformUtility.WorldToScreenPoint(null, viewportCenterWorld);

        Vector2 pixelOffset = screenViewportCenter - screenCanvasCenter;

        // 화면 픽셀 차이를 현재 Orthographic Camera의 World Unit 차이로 변환
        float worldHeight = targetCamera.orthographicSize * 2.0f;
        float worldPerPixel = worldHeight / Screen.height;

        return pixelOffset * worldPerPixel;
    }
}
