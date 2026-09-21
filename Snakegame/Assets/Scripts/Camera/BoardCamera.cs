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
        if (targetCamera == null)
        {
            Debug.LogError("BoardCamera:: Camera not found!");
            return; 
        }
        
        boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardCamera:: BoardManager not found!");
            return;   
        }
    }

    // 카메라 업데이트
    public void UpdateCamera()
    {
        if (boardManager == null || targetCamera == null || boardViewport == null)
        {
            return;   
        }
        
        UpdateSize();
        UpdatePosition();
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

    private Vector2 GetViewportWorldOffset()
    {
        Canvas canvas = boardViewport.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BoardCamera:: BoardViewport Canvas not found!");
            return Vector2.zero;   
        }
        
        RectTransform canvasRectTransform = canvas.transform as RectTransform;
        if (canvasRectTransform == null)
        {
            return Vector2.zero;
        }
        
        Vector3 canvasCenterWorld = canvasRectTransform.TransformPoint(canvasRectTransform.rect.center);    // Canvas 중심
        Vector3 viewportCenterWorld = boardViewport.TransformPoint(boardViewport.rect.center);    // BoardViewport 중심
        
        // UI 좌표 → 화면 좌표
        Vector2 canvasCenterScreen = RectTransformUtility.WorldToScreenPoint(null, canvasCenterWorld);
        Vector2 viewportCenterScreen = RectTransformUtility.WorldToScreenPoint(null, viewportCenterWorld);

        // 화면 중심에서 BoardViewport 중심까지의 Pixel 차이
        Vector2 pixelOffset = viewportCenterScreen - canvasCenterScreen;

        // 현재 Orthographic Camera의
        // Pixel → World Unit 변환
        float worldHeight = targetCamera.orthographicSize * 2.0f;
        float worldPerPixel = worldHeight / Screen.height;

        return pixelOffset * worldPerPixel;
    }
    
    // 크기 업데이트
    private void UpdateSize()
    {
        float boardWidth = boardManager.Width + boardPadding;
        float boardHeight = boardManager.Height + boardPadding;

        Rect viewportRect = boardViewport.rect;
        if (viewportRect.width <= 0.0f || viewportRect.height <= 0.0f)
        {
            return;
        }

        Canvas canvas = boardViewport.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect == null)
            return;

        float viewportAspect = viewportRect.width / viewportRect.height;

        // BoardViewport가 전체 Canvas에서 차지하는 비율
        float viewportHeightRatio = viewportRect.height / canvasRect.rect.height;
        float verticalSize = boardHeight * 0.5f;
        float horizontalSize = (boardWidth * 0.5f) / viewportAspect;
        float requiredSize = Mathf.Max(verticalSize, horizontalSize);

        // 전체 화면 Camera에서 BoardViewport 높이만큼
        // 사용하도록 크기를 보정
        targetCamera.orthographicSize = requiredSize / viewportHeightRatio;
    }
}
