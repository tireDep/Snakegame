using UnityEngine;

public class GameUI : MonoBehaviour
{
    private GameManager gameManager;
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부

    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject dimmedObject;
    [SerializeField] private GameObject readyPanel;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
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

    private void OnEnable()
    {
        if (!hasValidReferences)
        {
            return;   
        }

        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        if (!hasValidReferences)
        {
            return;   
        }

        Initialize();
    }

    private void OnDisable()
    {
        if (!hasValidReferences)
        {
            return;
        }

        gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Initialize()
    {
        OnGameStateChanged(gameManager.GameState);
    }

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
