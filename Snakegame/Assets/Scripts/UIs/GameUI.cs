using UnityEngine;

public class GameUI : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject dimmedObject;
    [SerializeField] private GameObject readyPanel;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameUI::Awake GameManager not found!");
        }
    }

    private void OnEnable()
    {
        if (gameManager == null)
        {
            return;   
        }

        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        if (gameManager == null)
        {
            return;   
        }

        Initialize();
    }

    private void OnDisable()
    {
        if (gameManager == null)
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
        if (topPanel == null || dimmedObject == null || readyPanel == null)
        {
            return;  
        }

        bool showReady = newState == GameState.Ready;
        topPanel.SetActive(!showReady);
        readyPanel.SetActive(showReady);
        dimmedObject.SetActive(showReady);
    }
}
