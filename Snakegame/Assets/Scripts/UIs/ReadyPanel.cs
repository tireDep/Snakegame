using TMPro;
using UnityEngine;

public class ReadyPanel : MonoBehaviour
{
    private GameManager gameManager;
    private bool isInitialized;    // 필수 참조 검증 완료 여부
    
    [SerializeField] private GameObject dimmedPanel;
    [SerializeField] private TMP_Text foodCountText;    // 아이템 개수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최대 개수 텍스트
    
    [SerializeField] private TMP_Text boardSizeText;    // 맵 크기 텍스트
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        isInitialized = ValidateReferences();
    }

    // 준비 화면 표시에 필요한 참조를 검증하는 함수
    private bool ValidateReferences()
    {
        if (gameManager == null)
        {
            Debug.LogError("ReadyPanel::ValidateReferences GameManager is required.", this);
            return false;
        }

        if (dimmedPanel == null)
        {
            Debug.LogError("ReadyPanel::ValidateReferences DimmedPanel is required.", this);
            return false;
        }

        if (foodCountText == null)
        {
            Debug.LogError("ReadyPanel::ValidateReferences FoodCountText is required.", this);
            return false;
        }

        if (bestCountText == null)
        {
            Debug.LogError("ReadyPanel::ValidateReferences BestCountText is required.", this);
            return false;
        }

        if (boardSizeText == null)
        {
            Debug.LogError("ReadyPanel::ValidateReferences BoardSizeText is required.", this);
            return false;
        }

        return true;
    }

    private void Start()
    {
        if (!isInitialized)
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
        if (!isInitialized)
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
        if (!isInitialized)
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
        if (!isInitialized)
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
        if (!isInitialized)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(-1);
    }

    // 맵 변경 뒤로 버튼 함수
    public void OnClickNext()
    {
        if (!isInitialized)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(1);
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (!isInitialized)
        {
            return;
        }

        bool isReady = newState == GameState.Ready;
        dimmedPanel.SetActive(isReady);

        UpdateAllCountText();
        
        gameObject.SetActive(isReady);
    }

    private void UpdateBoardSize(BoardSize boardSize)
    {
        if (!isInitialized)
        {
            return;
        }

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
