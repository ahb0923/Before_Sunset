using UnityEngine;

public static class GlobalState
{
    /// <summary>
    /// 스타트씬 애니메이션 관련 불값
    /// </summary>
    public static bool HasPlayedIntro;
    
    /// <summary>
    /// 오프닝씬 진입 관련 불값
    /// </summary>
    public static bool HasPlayedOpening;

    /// <summary>
    /// 튜토리얼 진입 관련 불값
    /// </summary>
    public static bool ToTutorial;

    /// <summary>
    /// 세이브로드 데이터 호출용 슬롯 인덱스
    /// </summary>
    public static int SaveIndex;
    
    /// <summary>
    /// 전체화면 체크 관련 불값
    /// </summary>
    public static bool IsFullScreen = true;
    
    /// <summary>
    /// 현재 해상도 관련 정수값
    /// 기본은 1920x1080
    /// </summary>
    public static int ResolutionIndex = 1;
    
    /// <summary>
    /// 해상도 목록
    /// </summary>
    public static readonly Vector2Int[] Resolutions =
    {
        new Vector2Int(1366, 768),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };
}