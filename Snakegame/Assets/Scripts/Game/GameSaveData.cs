using UnityEngine;

public static class GameSaveData
{
    private const string BOARD_SIZE_KEY = "BoardSize";        // 선택한 보드 크기 저장 키
    private const string SOUND_ENABLE_KEY = "SoundEnable";    // 사운드 설정 저장 키
    
    // 보드별 최고 점수가 증가했을 때 저장하는 함수
    public static void SaveBestCount(BoardSize boardSize, int bestCount)
    {
        int prevBestCount = LoadBestCount(boardSize);
        if (bestCount <= prevBestCount)
        {
            return;
        }
        
        string key = GetBestCountKey(boardSize);
        PlayerPrefs.SetInt(key, bestCount);
        PlayerPrefs.Save();
    }
    
    // 지정한 보드 크기의 최고 점수를 불러오는 함수
    public static int LoadBestCount(BoardSize boardSize)
    {
        string key = GetBestCountKey(boardSize);
        return PlayerPrefs.GetInt(key, 0);
    }
    
    // 보드 크기별 최고 점수 저장 키를 생성하는 함수
    private static string GetBestCountKey(BoardSize boardSize)
    {
        return $"BestCount_{boardSize}";
    }
    
    // 선택한 보드 크기가 바뀌었을 때 저장하는 함수
    public static void SaveBoardSize(BoardSize boardSize)
    {
        BoardSize prevBoardSize = LoadBoardSize();
        if (boardSize == prevBoardSize)
        {
            return;
        }
        
        PlayerPrefs.SetInt(BOARD_SIZE_KEY, (int)boardSize);
        PlayerPrefs.Save();
    }
    
    // 저장된 보드 크기를 불러오는 함수
    public static BoardSize LoadBoardSize()
    {
        int value = PlayerPrefs.GetInt(BOARD_SIZE_KEY, (int)BoardSize.Small);
        
        if (!System.Enum.IsDefined(typeof(BoardSize), value))
        {
            Debug.LogWarning($"GameSaveData::LoadBoardSize Invalid BoardSize: {value}");

            return BoardSize.Small;
        }
        
        return (BoardSize)value;
    }

    // 사운드 설정이 바뀌었을 때 저장하는 함수
    public static void SaveSoundEnable(bool isEnable)
    {
        bool previousSoundEnabled = LoadSoundEnabled();    // 현재 저장된 사운드 사용 여부

        // 저장된 값과 같으면 불필요한 PlayerPrefs 쓰기를 생략합니다.
        if (isEnable == previousSoundEnabled)
        {
            return;
        }

        PlayerPrefs.SetInt(SOUND_ENABLE_KEY, isEnable? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // 저장된 사운드 사용 여부를 불러오는 함수
    public static bool LoadSoundEnabled()
    {
        return PlayerPrefs.GetInt(SOUND_ENABLE_KEY, 1) == 1;
    }
}
