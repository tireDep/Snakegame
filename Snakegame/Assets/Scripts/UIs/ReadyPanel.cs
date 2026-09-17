using TMPro;
using UnityEngine;

public class ReadyPanel : MonoBehaviour
{
    private GameManager gameManager;
    
    [SerializeField] private GameObject dimmedPanel;
    [SerializeField] private TMP_Text foodCountText;    // 아이템 개수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최대 개수 텍스트
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("ReadyPanel:: GameManager not found!");
            return;   
        }

        if (dimmedPanel == null)
        {
            Debug.LogError("ReadyPanel:: DimmedPanel not found!");
            return;  
        }

        if (foodCountText == null)
        {
            Debug.LogError("ReadyPanel:: FoodCountText not found!");
            return;   
        }

        if (bestCountText == null)
        {
            Debug.LogError("ReadyPanel:: BestCountText not found!");
            return;  
        }
    }

    private void Start()
    {
        if (gameManager == null)
            return;

        OnGameStateChanged(gameManager.GameState);
    }

    private void Initialize()
    {
        foodCountText.text = gameManager.FoodCount.ToString();
        bestCountText.text = gameManager.BestCount.ToString();
    }
    
    private void OnEnable()
    {
        if (gameManager == null)
        {
            return;
        }
        
        gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    // 시작 버튼 함수
    public void OnClickPlay()
    {
        if (gameManager == null)
        {
            return;
        }
        
        gameManager.StartGame();
    }
    
    // 종료 버튼 함수
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 맵 변경 앞으로 버튼 함수
    public void OnClickPrev()
    {
        
    }

    // 맵 변경 뒤로 버튼 함수
    public void OnClickNext()
    {
        
    }

    private void OnGameStateChanged(GameState newState)
    {
        bool isReady = newState == GameState.Ready;
        if (dimmedPanel != null)
        {
            dimmedPanel.SetActive(isReady);
        }

        gameObject.SetActive(isReady);
        Initialize();   
    }
    
}
