public enum SnakeSegmentType
{
    Head,          // 뱀 머리 세그먼트
    Body,          // 뱀 직선 몸통 세그먼트
    BodyCorner,    // 뱀 모서리 몸통 세그먼트
    Tail           // 뱀 꼬리 세그먼트
}

public enum GameState
{
    Ready,      // 게임 시작 대기 상태
    Playing     // 게임 진행 상태
}

public enum BoardSize
{
    Small,         // 소형 보드
    Medium,        // 중형 보드
    Large,         // 대형 보드
    ExtraLarge     // 초대형 보드
}
