using TMPro;
using UnityEngine;

public class ReadyPanel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;    // 게임 관리자
    private bool hasValidReferences;    // 필수 참조 검증 완료 여부
    
    [SerializeField] private GameObject dimmedPanel;    // 준비 화면 배경 음영 패널
    [SerializeField] private TMP_Text foodCountText;    // 직전 점수 텍스트
    [SerializeField] private TMP_Text bestCountText;    // 최고 점수 텍스트
    
    [SerializeField] private TMP_Text boardSizeText;    // 보드 크기 텍스트
    
    // 준비 화면 표시에 필요한 참조를 준비하는 함수
    private void Awake()
    {
        hasValidReferences = ValidateReferences();
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

    // 현재 게임 상태를 준비 화면에 처음 반영하는 함수
    private void Start()
    {
        if (!hasValidReferences)
            return;

        OnGameStateChanged(gameManager.GameState);
    }

    // 직전 점수와 최고 점수, 보드 크기를 함께 갱신하는 함수
    private void UpdateAllCountText()
    {
        UpdateCountText(gameManager.LastFoodCount);
        UpdateBestCountText(gameManager.BestCount);
        UpdateBoardSize(gameManager.BoardSize);
    }

    // 직전 점수 텍스트를 갱신하는 함수
    private void UpdateCountText(int count)
    {
        foodCountText.text = count.ToString();
    }
    
    // 최고 점수 텍스트를 갱신하는 함수
    private void UpdateBestCountText(int count)
    {
        bestCountText.text = count.ToString();
    }
    
    // 준비 화면 이벤트를 구독하고 표시를 갱신하는 함수
    private void OnEnable()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        gameManager.OnGameStateChanged += OnGameStateChanged;
        gameManager.OnBoardSizeChanged += UpdateBoardSize;
        gameManager.OnLastFoodCountChanged += UpdateCountText;
        gameManager.OnBestCountChanged += UpdateBestCountText;

        UpdateAllCountText();
    }
    
    // 준비 화면 이벤트 구독을 해제하는 함수
    public void OnDisable()
    {
        if (!hasValidReferences)
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
        if (!hasValidReferences)
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

    // 이전 보드 크기를 선택하는 함수
    public void OnClickPrev()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(-1);
    }

    // 다음 보드 크기를 선택하는 함수
    public void OnClickNext()
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        gameManager.ChangeBoardSize(1);
    }

    // 게임 상태에 따라 준비 화면 표시를 전환하는 함수
    private void OnGameStateChanged(GameState newState)
    {
        if (!hasValidReferences)
        {
            return;
        }

        bool isReady = newState == GameState.Ready;
        dimmedPanel.SetActive(isReady);

        UpdateAllCountText();
        
        gameObject.SetActive(isReady);
    }

    // 선택한 보드 크기를 축약 텍스트로 표시하는 함수
    private void UpdateBoardSize(BoardSize boardSize)
    {
        if (!hasValidReferences)
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
