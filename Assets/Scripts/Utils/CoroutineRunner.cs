﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    readonly Dictionary<string, Coroutine> routines = new();
    public static CoroutineRunner Instance { get; private set; }

    void Awake ()
    {
        Instance = this;
    }

    public void StartCoroutine (string id, IEnumerator routine)
    {
        if (routines.TryGetValue(id, out var existing))
            StopCoroutine(existing);
        routines[id] = StartCoroutine(routine);
    }
}