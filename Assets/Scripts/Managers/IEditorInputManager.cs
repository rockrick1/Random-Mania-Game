﻿using System;
using UnityEngine;

public interface IEditorInputManager
{
    event Action OnSongPlayPause;
    event Action<float> OnSongScroll;
    event Action<float> OnZoomScroll;

    event Action OnSavePressed;

    Vector3 GetMousePos ();
}