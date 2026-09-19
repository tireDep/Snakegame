using UnityEngine;

public class GameSaveData : MonoBehaviour
{
    private const string BEST_COUNT_KEY = "BestCount";
    private const string BOARD_SIZE_KEY = "BoardSize";
    
    public static void SaveBestCount( BoardSize boardSize, int bestCount)
    {
        int prevBestCount = GameSaveData.LoadBestCount(boardSize);
        if (bestCount <= prevBestCount)
        {
            return;
        }
        
        string key = GetBestCountKey(boardSize);
        PlayerPrefs.SetInt(key, bestCount);
        PlayerPrefs.Save();
    }
    
    public static int LoadBestCount(BoardSize boardSize)
    {
        string key = GetBestCountKey(boardSize);
        return PlayerPrefs.GetInt(key, 0);
    }
    
    private static string GetBestCountKey(BoardSize boardSize)
    {
        return $"BestCount_{boardSize}";
    }
    
    public static int LoadBestCount()
    {
        return PlayerPrefs.GetInt(BEST_COUNT_KEY, 0);
    }
    
    public static void SaveBoardSize(BoardSize boardSize)
    {
        BoardSize prevBoardSize = GameSaveData.LoadBoardSize();
        if (boardSize == prevBoardSize)
        {
            return;
        }
        
        PlayerPrefs.SetInt(BOARD_SIZE_KEY, (int)boardSize);
        PlayerPrefs.Save();
    }
    
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
}
