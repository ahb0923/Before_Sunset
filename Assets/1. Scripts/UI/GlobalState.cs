public static class GlobalState
{
    /// <summary>
    /// 스타트씬 애니메이션 관련 불값
    /// </summary>
    public static bool HasPlayedIntro= true;
    
    /// <summary>
    /// 오프닝씬 진입 관련 불값
    /// </summary>
    public static bool HasPlayedOpening = true;

    /// <summary>
    /// 튜토리얼 진입 관련 불값
    /// </summary>
    public static bool ToTutorial;

    /// <summary>
    /// 세이브로드 데이터 호출용 슬롯 인덱스
    /// </summary>
    public static int Index;
}
