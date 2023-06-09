﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongModel : ISongModel
{
    public event Action<Note> OnNoteSpawned;
    public event Action<Note, HitScore> OnNoteHit;
    public event Action<Note, HitScore> OnLongNoteHit;
    public event Action<Note, HitScore> OnLongNoteReleased;
    public event Action<Note> OnNoteMissed;
    public event Action OnAudioStartTimeReached;
    public event Action OnSongFinished;

    public ISongSettings CurrentSongSettings => songLoaderModel.Settings;
    public AudioClip CurrentSongAudio => songLoaderModel.Audio;
    public bool AllNotesRead { get; private set; }

    readonly IGameInputManager inputManager;
    readonly ISongLoaderModel songLoaderModel;
    
    float perfectHitWindow;
    float greatHitWindow;
    float okayHitWindow;

    double dspSongStart;
    double pauseOffset;

    public SongModel (IGameInputManager inputManager, ISongLoaderModel songLoaderModel)
    {
        this.inputManager = inputManager;
        this.songLoaderModel = songLoaderModel;
    }

    public void Initialize ()
    {
        songLoaderModel.Initialize();
        AddListeners();
    }

    void AddListeners ()
    {
    }

    void RemoveListeners ()
    {
    }
    
    void HandlePause ()
    {
        // throw new NotImplementedException();
    }

    void HandleResume ()
    {
        // throw new NotImplementedException();
    }

    public void LoadSong (string songId)
    {
        songLoaderModel.LoadSong(songId);
        perfectHitWindow = (80 - 6 * CurrentSongSettings.Difficulty) / 1000f;
        greatHitWindow = (140 - 8 * CurrentSongSettings.Difficulty) / 1000f;
        okayHitWindow = (200 - 10 * CurrentSongSettings.Difficulty) / 1000f;
    }

    public void Play ()
    {
        dspSongStart = AudioSettings.dspTime + GetStartingElapsed();
        
        CoroutineRunner.Instance.StartRoutine(nameof(AudioStartRoutine), AudioStartRoutine());
        CoroutineRunner.Instance.StartRoutine(nameof(NoteSpawnRotutine), NoteSpawnRotutine());
        CoroutineRunner.Instance.StartRoutine(nameof(NotesHitRoutine), NotesHitRoutine());
        CoroutineRunner.Instance.StartRoutine(nameof(PauseOffsetRoutine), PauseOffsetRoutine());
    }

    double GetStartingElapsed () => CurrentSongSettings.StartingTime < CurrentSongSettings.ApproachRate
        ? CurrentSongSettings.ApproachRate + CurrentSongSettings.StartingTime
        : CurrentSongSettings.StartingTime;

    IEnumerator AudioStartRoutine ()
    {
        if (CurrentSongSettings.ApproachRate > CurrentSongSettings.StartingTime)
            yield return new WaitForSeconds(CurrentSongSettings.ApproachRate);
        OnAudioStartTimeReached?.Invoke();
    }

    IEnumerator NoteSpawnRotutine ()
    {
        IReadOnlyList<Note> notes = CurrentSongSettings.Notes;
        int noteIndex = 0;
        while (true)
        {
            yield return null;
            
            double elapsed = AudioSettings.dspTime - dspSongStart - pauseOffset;
            double noteSpawnTime = notes[noteIndex].Time - CurrentSongSettings.ApproachRate;
            
            if (elapsed > noteSpawnTime)
            {
                OnNoteSpawned?.Invoke(notes[noteIndex]);
                if (++noteIndex >= notes.Count)
                    break;
            }
        }
    }

    IEnumerator NotesHitRoutine ()
    {
        IReadOnlyList<Note> notes = CurrentSongSettings.Notes;
        int noteIndex = 0;
        bool hittingCurrentLongNote = false;
        
        while (true)
        {
            if (noteIndex >= notes.Count)
                break;
            
            yield return null;

            Note currentNote = notes[noteIndex];

            double elapsed = AudioSettings.dspTime - dspSongStart - pauseOffset;
            double timeToNote = currentNote.Time - elapsed;
            double timeToNoteEnd = currentNote.EndTime - elapsed;

            if (!currentNote.IsLong)
            {
                if (timeToNote < okayHitWindow && inputManager.GetPositionPressed(currentNote.Position))
                {
                    OnNoteHit?.Invoke(currentNote, GetHitScore(timeToNote));
                    noteIndex++;
                    continue;
                }
            }
            else
            {
                if (hittingCurrentLongNote)
                {
                    if (timeToNoteEnd < 0 || !inputManager.GetPositionHeld(currentNote.Position))
                    {
                        hittingCurrentLongNote = false;
                        OnLongNoteReleased?.Invoke(currentNote, GetHitScore(timeToNoteEnd));
                        noteIndex++;
                    }
                    continue;
                }
                
                if (timeToNote < okayHitWindow && inputManager.GetPositionPressed(currentNote.Position))
                {
                    // Debug.Log($"{DateTime.Now}");
                    OnLongNoteHit?.Invoke(currentNote, GetHitScore(timeToNote));
                    hittingCurrentLongNote = true;
                    continue;
                }
            }
            
            if (timeToNote < -okayHitWindow)
            {
                OnNoteMissed?.Invoke(currentNote);
                noteIndex++;
            }
        }

        AllNotesRead = true;
        yield return new WaitForSeconds(okayHitWindow * 3);
        OnSongFinished?.Invoke();
    }

    IEnumerator PauseOffsetRoutine ()
    {
        while (true)
        {
            double lastDspTime = AudioSettings.dspTime;
            yield return null;
            if (GameManager.IsPaused)
                pauseOffset += AudioSettings.dspTime - lastDspTime;
        }
    }
    
    HitScore GetHitScore (double timeToNoteHit)
    {
        double absValue = Math.Abs(timeToNoteHit);
        if (absValue <= perfectHitWindow)
            return HitScore.Perfect;
        if (absValue <= greatHitWindow)
            return HitScore.Great;
        if (absValue <= okayHitWindow)
            return HitScore.Okay;
        return HitScore.Miss;
    }

    public void Dispose ()
    {
        RemoveListeners();
        CoroutineRunner.Instance.StopRoutine(nameof(AudioStartRoutine));
        CoroutineRunner.Instance.StopRoutine(nameof(NoteSpawnRotutine));
        CoroutineRunner.Instance.StopRoutine(nameof(NotesHitRoutine));
        CoroutineRunner.Instance.StopRoutine(nameof(PauseOffsetRoutine));
    }
}