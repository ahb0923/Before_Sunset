using System;
using System.Collections.Generic;
using System.Diagnostics;
#nullable enable

public static class DictionaryExtensions
{
    /// <summary>
    /// 딕셔너리의 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="TK">키</typeparam>
    /// <typeparam name="TV">값</typeparam>
    /// <param name="dict">딕셔너리</param>
    /// <param name="key">키</param>
    /// <param name="defaultValue">값을 찾지 못했을 때 반환할 값</param>
    /// <returns></returns>
    public static TV? Get<TK, TV>(this IDictionary<TK, TV> dict, TK? key, TV? defaultValue = default)
    {
        if (key == null)
        {
            return defaultValue;
        }

        return dict.TryGetValue(key, out TV tv) ? tv : defaultValue;
    }

    /// <summary>
    /// 딕셔너리 키-값에 다른 딕셔너리의 키-값을 모두 추가합니다.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
    {
        if (source == null)
        {
            throw new NullReferenceException("Source is null");
        }

        if (target == null)
        {
            return;
        }

        foreach (KeyValuePair<TKey, TValue> keyValuePair in target)
        {
            source[keyValuePair.Key] = keyValuePair.Value;
        }
    }
}

