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
    /// 일시정지를 포함해서 지연을 기다림
    /// </summary>
    /// <param name="seconds">지연 대기 시간</param>
    /// <param name="getIsPaused">일시정지를 나타낼 bool 값을 가리키는 델리게이트</param>
    /// <returns></returns>
    public static IEnumerator C_WaitIfNotPaused(float seconds, Func<bool> getIsPaused)
    {
        float timer = seconds;

        // 남은 시간이 0보다 큰 동안 반복
        while (timer > 0f)
        {
            // 일시 정지 상태인지 확인
            if (getIsPaused())
            {
                yield return null; // 일시정지 중이므로, 다음 프레임까지 대기
            }
            else
            {
                timer -= Time.deltaTime; // 일시정지가 아니면, 현재 프레임의 시간만큼 타이머 감소
                yield return null; // 다음 프레임까지 대기
            }
        }
    }

    /// <summary>
    /// 지정 시간 동안 대기하되, 중간에 조건을 만족하면 즉시 종료
    /// </summary>
    /// <param name="seconds">대기 시간</param>
    /// <param name="interruptCheck">중단 조건</param>
    public static IEnumerator WaitWithInterrupt(float seconds, Func<bool> interruptCheck)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (interruptCheck != null && interruptCheck())
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }
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
