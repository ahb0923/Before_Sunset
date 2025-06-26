using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainSingleton<T> where T: class, new()
{
    private static T _instance;
    public static T Instance => _instance ??= new T();
}
