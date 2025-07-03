using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper_Component
{
    //============================================
    //          << 단일 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// GameObject로부터 직접 해당하는 컴포넌트를 찾음
    /// 목표 컴포넌트를 못 찾아도 상관 없거나, 없을 경우의 로직이 필요할 때 사용
    /// </summary>
    /// <typeparam name="T">컴포넌트</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="result">목표 컴포넌트</param>
    /// <returns>컴포넌트를 찾으면 true, 찾지 못하면 false</returns>
    public static bool TryGetComponent<T>(this GameObject gameObject, out T result) where T : Component
    {
        result = gameObject.GetComponent<T>();
        return result != null;
    }

    /// <summary>
    /// 컴포넌트에서 다른 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 못 찾아도 상관 없거나, 없을 경우의 로직이 필요할 때 사용
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component">주체 컴포넌트</param>
    /// <param name="result">목표 컴포넌트</param>
    /// <returns>컴포넌트를 찾으면 true, 찾지 못하면 false</returns>
    public static bool TryGetComponent<T>(this Component component, out T result) where T : Component
    {
        result = component.GetComponent<T>();
        return result != null;
    }

    /// <summary>
    /// GameObject로부터 직접 해당하는 컴포넌트를 찾음<br/>
    /// 목표 컴포넌트를 반드시 찾아야 할 경우<br/>
    /// 못 찾을 경우에는 에러 로그 출력
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>찾은 컴포넌트 인스턴스, 없다면 null</returns>
    public static T GetComponent<T>(this GameObject gameObject, string context = "") where T : Component
    {
        var result = gameObject.GetComponent<T>();
        if (result == null)
            Debug.LogError($"[Helper_Component / {context}] Missing component: {typeof(T).Name} on {gameObject.name}");
        return result;
    }

    /// <summary>
    /// 컴포넌트에서 다른 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 반드시 찾아야 할 경우<br/>
    /// 못 찾을 경우에는 에러 로그 출력
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="component">주체 컴포넌트</param>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>찾은 컴포넌트 인스턴스, 없다면 null</returns>
    public static T GetComponent<T>(this Component component, string context = "") where T : Component
    {
        var result = component.GetComponent<T>();
        if (result == null)
            Debug.LogError($"[Helper_Component / {context}] Missing component: {typeof(T).Name} on {component.name}");
        return result;
    }



    //============================================
    //          << 자식 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// 자식 오브젝트에서 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 못 찾아도 상관 없거나, 없을 경우의 로직이 필요할 때 사용
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="result">목표 컴포넌트</param>
    /// <param name="isActive">비활성화 된 오브젝트도 찾을지에 대한 여부</param>
    /// <returns>컴포넌트를 찾으면 true, 찾지 못하면 false</returns>
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result, bool isActive = false) where T : Component
    {
        result = gameObject.GetComponentInChildren<T>(isActive);
        return result != null;
    }

    /// <summary>
    /// 자식 오브젝트에서 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 반드시 찾아야 할 경우<br/>
    /// 못 찾을 경우에는 에러 로그 출력 
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <param name="isActive">비활성화 된 오브젝트도 찾을지에 대한 여부</param>
    /// <returns>찾은 컴포넌트 인스턴스, 없다면 null</returns>
    public static T GetComponentInChildren<T>(this GameObject gameObject, string context = "", bool isActive = false) where T : Component
    {
        var component = gameObject.GetComponentInChildren<T>(isActive);
        if (component == null)
            Debug.LogError($"[Helper_Component / {context}] Missing : {typeof(T).Name} in children of {gameObject.name}");
        return component;
    }

    /// <summary>
    /// 자식 오브젝트 중에서 목표 컴포넌트 타입을 전부 찾음
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="isActive">비활성화 된 오브젝트도 찾을지에 대한 여부</param>
    /// <returns>자식 계층에서 찾은 모든 컴포넌트 배열, 없다면 배열 길이 0</returns>
    public static T[] GetAllInChildren<T>(this GameObject gameObject, bool isActive = false) where T : Component
    {
        return gameObject.GetComponentsInChildren<T>(isActive);
    }




    //============================================
    //          << 부모 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// 부모 오브젝트에서 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 못 찾아도 상관 없거나, 없을 경우의 로직이 필요할 때 사용
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="result">목표 컴포넌트</param>
    /// <returns>컴포넌트를 찾으면 true, 찾지 못하면 false</returns>
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result) where T : Component
    {
        result = gameObject.GetComponentInParent<T>();
        return result != null;
    }

    /// <summary>
    /// 부모 오브젝트에서 컴포넌트를 찾을 경우<br/>
    /// 목표 컴포넌트를 반드시 찾아야 할 경우<br/>
    /// 못 찾을 경우에는 에러 로그 출력 
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>찾은 컴포넌트 인스턴스, 없다면 null</returns>
    public static T GetComponentInParent<T>(this GameObject gameObject, string context = "") where T : Component
    {
        var component = gameObject.GetComponentInParent<T>();
        if (component == null)
            Debug.LogError($"[Helper_Component / {context}] Missing : {typeof(T).Name} in children of {gameObject.name}");
        return component;
    }

    /// <summary>
    /// 부모 오브젝트 중에서 목표 컴포넌트 타입을 전부 찾음
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="gameObject">주체 오브젝트</param>
    /// <returns>부모 계층에서 찾은 모든 컴포넌트 배열, 없다면 배열 길이 0</returns>
    public static T[] GetAllInParent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponentsInParent<T>();
    }



    //============================================
    //          << 복수 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// GameObject로부터 해당하는 컴포넌트를 모두 찾음
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns>GameObject에 붙은 모든 컴포넌트 배열, 없다면 배열 길이 0</returns>
    public static T[] GetAll<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponents<T>();
    }
    /// <summary>
    /// 컴포넌트에서 다른 컴포넌트를 찾을 경우<br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns>같은 GameObject에 붙은 모든 컴포넌트 배열, 없다면 배열 길이 0</returns>
    public static T[] GetAll<T>(this Component component) where T : Component
    {
        return component.GetComponents<T>();
    }



    //============================================
    //          << 전체 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// 전체 오브젝트중 가장 먼저 발견되는 컴포넌트를 찾음
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>가장 먼저 발견된 컴포넌트 인스턴스, 없다면 null</returns>
    public static T Find<T>(string context = "") where T : UnityEngine.Object
    {
        var result = UnityEngine.Object.FindObjectOfType<T>();
        if (result == null)
            Debug.LogError($"[Helper_Component / {context}] No object of type : {typeof(T).Name}");
        return result;
    }

    /// <summary>
    /// 전 체 오브젝트에서 목표 컴포넌트 타입을 전부 찾음
    /// </summary>
    /// <typeparam name="T">목표 컴포넌트 타입</typeparam>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>가장 먼저 발견된 컴포넌트 배열, 없다면 배열 길이 0</returns>
    public static T[] FindAll<T>(string context = "") where T : UnityEngine.Object
    {
        var results = UnityEngine.Object.FindObjectsOfType<T>();
        if (results == null || results.Length == 0)
            Debug.LogError($"[Helper_Component / {context}] No object of type : {typeof(T).Name}");
        return results;
    }



    //============================================
    //          << 태그 컴포넌트 탐색 >>
    //============================================
    /// <summary>
    /// 목표 태그를 갖고 있는 가장 먼저 발견되는 게임 오브젝트를 찾음
    /// </summary>
    /// <param name="tag">목표 태그</param>
    /// <param name="context">디버깅 시 체크용도, 호출 위치 기록</param>
    /// <returns>해당 태그를 가진 가장 먼저 발견되는 GameObejct, 없다면 null</returns>
    public static GameObject FindWithTag(string tag, string context = "")
    {
        var result = GameObject.FindWithTag(tag);
        if (result == null)
            Debug.LogError($"[Helper_Component / {context}] No GameObject with tag : {tag}");
        return result;
    }

    /// <summary>
    /// 목표 태그를 갖고 있는 모든 게임 오브젝트를 찾음
    /// </summary>
    /// <param name="tag">목표 태그</param>
    /// <returns>해당 태그를 가진 모든 GameObject 배열, 없다면 배열 길이 0</returns>
    public static GameObject[] FindAllWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag);
    }



    //============================================
    //          << 이름 컴포넌트 탐색 >>       
    //============================================
    /// <summary>
    /// 해당 이름을 가진 자식을 찾음(Transform)<br/>
    /// 하부 계층의 자식들도 재귀적으로 모두 찾음
    /// </summary>
    /// <param name="parent">주체 부모 오브젝트</param>
    /// <param name="name">목표 자식 오브젝트 이름</param>
    /// <param name="isActive">비활성화 된 오브젝트도 찾을지에 대한 여부</param>
    /// <returns>찾은 Transfrom, 없다면 null</returns>
    public static Transform FindChildByName(this Transform parent, string name, bool isActive = false)
    {
        foreach (Transform child in parent)
        {
            if (!isActive && !child.gameObject.activeSelf) continue;
            if (child.name == name)
                return child;

            var found = child.FindChildByName(name, isActive);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// 해당 이름을 가진 자식을 찾음(GameObject)<br/>
    /// 하부 계층의 자식들도 재귀적으로 모두 찾음
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name">목표 자식 오브젝트 이름</param>
    /// <param name="isActive">비활성화 된 오브젝트도 찾을지에 대한 여부</param>
    /// <returns>찾은 GameObject, 없다면 null</returns>
    public static GameObject FindChildGameObjectByName(this GameObject gameObject, string name, bool isActive = false)
    {
        var foundTransform = gameObject.transform.FindChildByName(name, isActive);
        return foundTransform?.gameObject;
    }

    /// <summary>
    /// 특정 자식의 컴포넌트를 찾아내는 메서드 (무거우니 절대 남용 금지)
    /// </summary>
    /// <param name="parent">주체 부모 오브젝트</param>
    /// <param name="name">목표 자식 오브젝트 이름</param>
    /// <typeparam name="T">찾고싶은 컴포넌트</typeparam>
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
}
