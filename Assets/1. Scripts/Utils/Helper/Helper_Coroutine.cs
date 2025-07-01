using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper_Coroutine
{
    private static readonly Dictionary<float, WaitForSeconds> _waitList = new();
    /// <summary>
    /// 캐싱용 메서드
    /// </summary>
    /// <param name="seconds">지연 대기 시간</param>
    /// <returns></returns>
    public static WaitForSeconds WaitSeconds(float seconds)
    {
        if (!_waitList.TryGetValue(seconds, out var wait))
        {
            wait = new WaitForSeconds(seconds);
            _waitList[seconds] = wait;
        }
        return wait;
    }

    /// <summary>
    /// 코루틴 지연 시작
    /// </summary>
    /// <param name="delay">지연 대기 시간</param>
    /// <param name="action">행동할 함수</param>
    /// <returns></returns>
    public static IEnumerator C_Delay(float delay, Action action)
    {
        yield return WaitSeconds(delay);
        action?.Invoke();
    }

    /// <summary>
    /// 일정 간격을 두고 반복할 메서드
    /// </summary>
    /// <param name="interval">행동 간격</param>
    /// <param name="action">행동할 함수</param>
    /// <param name="repeatCount">반복 횟수, 디폴트면 무한루프</param>
    /// <returns></returns>
    public static IEnumerator C_Repeat(float interval, Action action, int repeatCount = -1)
    {
        int count = 0;
        while (repeatCount < 0 || count < repeatCount)
        { 
            action?.Invoke();
            yield return WaitSeconds(interval);
            count++;
        }
    }
}
