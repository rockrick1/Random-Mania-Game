﻿using System;

public interface IEditorSongModel : IDisposable
{
    int SelectedSignature { get; }
    float SignedBeatInterval { get; }

    void Initialize ();
    void StartCreatingNote (int pos, float songPlayerTime, float viewHeight);
    NoteCreationResult? CreateNote (int pos, float songProgress, float height);
    void RemoveNoteAt (int index);
    int GetSeparatorColorByIndex (int i);
    float GetNextBeat (float time, int direction);
    void ChangeBpm (float val);
    void ChangeAr (float val);
    void ChangeDiff (float val);
    void ChangeStartingTime (float val);
    void ChangeSignature (int signature);
}