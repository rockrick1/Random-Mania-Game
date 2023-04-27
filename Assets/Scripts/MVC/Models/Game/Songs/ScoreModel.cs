﻿using System;

public class ScoreModel : IScoreModel
{
    public event Action<int> OnComboChanged;
    
    readonly ISongModel songModel;

    public int Combo
    {
        get => combo;
        set
        {
            if (combo != value)
                OnComboChanged?.Invoke(value);
            combo = value;
        }
    }
    
    int combo;

    public ScoreModel (ISongModel songModel)
    {
        this.songModel = songModel;
    }

    public void Initialize ()
    {
        AddListeners();
    }

    void AddListeners ()
    {
        songModel.OnNoteHit += HandleNoteHit;
        songModel.OnNoteMissed += HandleNoteMissed;
    }

    void RemoveListeners ()
    {
        songModel.OnNoteHit -= HandleNoteHit;
        songModel.OnNoteMissed -= HandleNoteMissed;
    }

    void HandleNoteHit (Note _, HitScore __)
    {
        //TODO calculate score here
        Combo++;
    }

    void HandleNoteMissed (Note _) => Combo = 0;

    public void Dispose ()
    {
        RemoveListeners();
    }
}