using TMPro;
using UnityEngine;

public class ReadyPanel : MonoBehaviour
{
    private GameManager gameManager;
    
    [SerializeField] private GameObject dimmedPanel;
    [SerializeField] private TMP_Text foodCountText;    // 아이템 개수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최대 개수 텍스트
    
    [SerializeField] private TMP_Text boardSizeText;    // 맵 크기 텍스트
    
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

    private void UpdateAllCountText()
    {
        UpdateCountText(gameManager.LastFoodCount);
        UpdateBestCountText(gameManager.BestCount);
        UpdateBoardSize(gameManager.BoardSize);
    }

    private void UpdateCountText(int count)
    {
        foodCountText.text = count.ToString();
    }
    
    private void UpdateBestCountText(int count)
    {
        bestCountText.text = count.ToString();
    }
    
    private void OnEnable()
    {
        if (gameManager == null)
        {
            return;
        }
        
        gameManager.OnGameStateChanged += OnGameStateChanged;
        gameManager.OnBoardSizeChanged += UpdateBoardSize;
        gameManager.OnLastFoodCountChanged += UpdateCountText;
        gameManager.OnBestCountChanged += UpdateBestCountText;

        UpdateAllCountText();
    }
    
    public void OnDisable()
    {
        if (gameManager == null)
        {
            return;   
        }
        
        gameManager.OnGameStateChanged -= OnGameStateChanged;
        gameManager.OnBoardSizeChanged -= UpdateBoardSize;
        gameManager.OnLastFoodCountChanged -= UpdateCountText;
        gameManager.OnBestCountChanged -= UpdateBestCountText;
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
        if (gameManager == null)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(-1);
    }

    // 맵 변경 뒤로 버튼 함수
    public void OnClickNext()
    {
        if (gameManager == null)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(1);
    }

    private void OnGameStateChanged(GameState newState)
    {
        bool isReady = newState == GameState.Ready;
        if (dimmedPanel != null)
        {
            dimmedPanel.SetActive(isReady);
        }

        if (gameManager != null)
        {
            UpdateAllCountText();
        }
        
        gameObject.SetActive(isReady);
    }

    private void UpdateBoardSize(BoardSize boardSize)
    {
        switch (boardSize)
        {
            case BoardSize.Small:
            {
                boardSizeText.text = "S";
            }
                break;
            case BoardSize.Medium:
            {
                boardSizeText.text = "M";
            }
                break;
            case BoardSize.Large:
            {
                boardSizeText.text = "L";
            }
                break;
            case BoardSize.ExtraLarge:
            {
                boardSizeText.text = "XL";
            }
                break;
        }
    }
}
