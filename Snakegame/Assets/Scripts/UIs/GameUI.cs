using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;    // 게임 관리자
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부

    [SerializeField] private GameObject topPanel;         // 게임 진행 중 상단 패널
    [SerializeField] private GameObject dimmedObject;     // 준비 화면 배경 음영
    [SerializeField] private GameObject readyPanel;       // 게임 준비 패널

    // 게임 UI에 필요한 참조를 준비하는 함수
    private void Awake()
    {
        hasValidReferences = ValidateReferences();
    }

    // 게임 UI 전환에 필요한 참조를 검증하는 함수
    private bool ValidateReferences()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameUI::ValidateReferences GameManager is required.", this);
            return false;
        }

        if (topPanel == null)
        {
            Debug.LogError("GameUI::ValidateReferences TopPanel is required.", this);
            return false;
        }

        if (dimmedObject == null)
        {
            Debug.LogError("GameUI::ValidateReferences DimmedObject is required.", this);
            return false;
        }

        if (readyPanel == null)
        {
            Debug.LogError("GameUI::ValidateReferences ReadyPanel is required.", this);
            return false;
        }

        return true;
    }

    // 게임 상태 변경 이벤트를 구독하는 함수
    private void OnEnable()
    {
        if (!hasValidReferences)
        {
            return;   
        }

        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    // 현재 게임 상태를 UI에 처음 반영하는 함수
    private void Start()
    {
        if (!hasValidReferences)
        {
            return;   
        }

        Initialize();
    }

    // 게임 상태 변경 이벤트 구독을 해제하는 함수
    private void OnDisable()
    {
        if (!hasValidReferences)
        {
            return;
        }

        gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    // 현재 게임 상태로 UI 표시를 초기화하는 함수
    private void Initialize()
    {
        OnGameStateChanged(gameManager.GameState);
    }

    // 게임 상태에 맞춰 진행 화면과 준비 화면을 전환하는 함수
    private void OnGameStateChanged(GameState newState)
    {
        if (!hasValidReferences)
        {
            return;  
        }

        bool showReady = newState == GameState.Ready;
        topPanel.SetActive(!showReady);
        readyPanel.SetActive(showReady);
        dimmedObject.SetActive(showReady);
    }
}
