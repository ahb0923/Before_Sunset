using UnityEngine;

public static class UtilityLJH
{
    /// <summary>
    /// 특정 자식의 컴포넌트를 찾아내는 메서드 (무거우니 절대 남용 금지)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindChildComponent<T>(Transform parent, string name) where T : Component
    {
        foreach (Transform child in parent)
        {
            string compareName = child.name;
            
            if (compareName == name)
            {
                T component = child.GetComponent<T>();
                return component;
            }

            T result = FindChildComponent<T>(child, name);

            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// 특정 자식을 찾는 메서드 (무거우니 절대 남용 금지)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildInChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            
            Transform result = FindChildInChild(child, name);
            
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}