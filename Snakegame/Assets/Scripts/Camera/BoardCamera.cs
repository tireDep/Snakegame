using UnityEngine;

public class BoardCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private RectTransform boardViewport;               // 보드 화면 표시 영역
    [SerializeField] private float boardPadding = 2.0f;                 // 보드 주변 카메라 여백
    [SerializeField] private Vector2 cameraOffset = Vector2.zero;       // 보드 중심 기준 카메라 오프셋
 
    [SerializeField] private BoardManager boardManager;    // 보드 관리자
    private Camera targetCamera;                         // 크기와 위치를 조정할 카메라
    private RectTransform canvasRectTransform;    // 보드 표시 영역이 속한 Canvas 좌표 영역
    private bool hasValidReferences;              // 필수 참조 검증 완료 여부

    // 카메라와 필수 참조를 준비하는 함수
    private void Awake()
    {
        targetCamera = GetComponent<Camera>();

        hasValidReferences = ValidateReferences();
    }

    // 보드 카메라 갱신에 필요한 참조를 검증하는 함수
    private bool ValidateReferences()
    {
        if (targetCamera == null)
        {
            Debug.LogError("BoardCamera::ValidateReferences Camera is required.", this);
            return false;
        }

        if (boardManager == null)
        {
            Debug.LogError("BoardCamera::ValidateReferences BoardManager is required.", this);
            return false;
        }

        if (boardViewport == null)
        {
            Debug.LogError("BoardCamera::ValidateReferences BoardViewport is required.", this);
            return false;
        }

        Canvas canvas = boardViewport.GetComponentInParent<Canvas>();    // 보드 표시 영역이 속한 Canvas
        if (canvas == null)
        {
            Debug.LogError("BoardCamera::ValidateReferences BoardViewport must belong to a Canvas.", this);
            return false;
        }

        canvasRectTransform = canvas.transform as RectTransform;
        if (canvasRectTransform == null)
        {
            Debug.LogError("BoardCamera::ValidateReferences Canvas must use a RectTransform.", this);
            return false;
        }

        return true;
    }

    // 보드에 맞춰 카메라 크기와 위치를 갱신하는 함수
    public bool UpdateCamera()
    {
        if (!hasValidReferences)
        {
            return false;
        }
        
        UpdateSize();
        UpdatePosition();

        return true;
    }

    // 보드가 표시 영역 중앙에 오도록 카메라 위치를 갱신하는 함수
    private void UpdatePosition()
    {
        Vector3 boardCenter = boardManager.GetBoardCenterWorld();
        Vector2 viewportOffset = GetViewportWorldOffset();
        
        transform.position = new Vector3( 
            boardCenter.x - viewportOffset.x + cameraOffset.x,
            boardCenter.y - viewportOffset.y + cameraOffset.y,
            transform.position.z);
    }

    // 표시 영역 중심과 Canvas 중심 사이의 월드 오프셋을 계산하는 함수
    private Vector2 GetViewportWorldOffset()
    {
        Vector3 canvasCenterWorld = canvasRectTransform.TransformPoint(canvasRectTransform.rect.center);    // Canvas 중심
        Vector3 viewportCenterWorld = boardViewport.TransformPoint(boardViewport.rect.center);    // BoardViewport 중심
        
        // 서로 다른 RectTransform 중심을 같은 화면 좌표계에서 비교합니다.
        Vector2 canvasCenterScreen = RectTransformUtility.WorldToScreenPoint(null, canvasCenterWorld);
        Vector2 viewportCenterScreen = RectTransformUtility.WorldToScreenPoint(null, viewportCenterWorld);

        // Canvas 중심을 기준으로 보드 표시 영역 중심의 화면 오프셋을 계산합니다.
        Vector2 pixelOffset = viewportCenterScreen - canvasCenterScreen;

        // 현재 Orthographic Camera의
        // Pixel → World Unit 변환
        float worldHeight = targetCamera.orthographicSize * 2.0f;
        float worldPerPixel = worldHeight / Screen.height;

        return pixelOffset * worldPerPixel;
    }
    
    // 보드 전체가 표시 영역에 들어오도록 카메라 크기를 갱신하는 함수
    private void UpdateSize()
    {
        float boardWidth = boardManager.Width + boardPadding;
        float boardHeight = boardManager.Height + boardPadding;

        Rect viewportRect = boardViewport.rect;
        if (viewportRect.width <= 0.0f || viewportRect.height <= 0.0f)
        {
            return;
        }

        float viewportAspect = viewportRect.width / viewportRect.height;

        // BoardViewport가 전체 Canvas에서 차지하는 비율
        float viewportHeightRatio = viewportRect.height / canvasRectTransform.rect.height;
        float verticalSize = boardHeight * 0.5f;
        float horizontalSize = (boardWidth * 0.5f) / viewportAspect;
        float requiredSize = Mathf.Max(verticalSize, horizontalSize);

        // 전체 화면 Camera에서 BoardViewport 높이만큼
        // 사용하도록 크기를 보정
        targetCamera.orthographicSize = requiredSize / viewportHeightRatio;
    }
}
