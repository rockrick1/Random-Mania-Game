﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSongView : MonoBehaviour
{
    public event Action<int> OnFieldButtonLeftClicked;
    public event Action<int> OnFieldButtonLeftReleased;
    
    [SerializeField] UIClickHandler fieldButtonLeft;
    [SerializeField] UIClickHandler fieldButtonCenter;
    [SerializeField] UIClickHandler fieldButtonRight;
    [SerializeField] RectTransform songObjects;

    [Header("Notes")]
    [SerializeField] EditorNoteView editorNoteViewPrefab;
    [SerializeField] EditorLongNoteView editorLongNoteViewPrefab;
    [SerializeField] Transform notesParent;
    [SerializeField] Transform leftNotesPosition;
    [SerializeField] Transform centerNotesPosition;
    [SerializeField] Transform rightNotesPosition;

    [Header("Separators")]
    [SerializeField] GameObject separatorPrefab;
    [SerializeField] VerticalLayoutGroup separatorsParent;
    [SerializeField] Color beatColor;
    [SerializeField] Color halfBeatColor;
    [SerializeField] Color thirdBeatColor;
    [SerializeField] Color quarterBeatColor;

    public float Height { get; private set; }
    public float ObjectsSpeed { get; private set; }

    readonly List<GameObject> separatorInstances = new();

    IAudioManager audioManager;
    float progress;
    float zoomScale;
    float songLength;
    float totalHeight;
    float approachRate;
    float beatInterval;
    
    void Start ()
    {
        audioManager = AudioManager.GetOrCreate();
        fieldButtonLeft.OnLeftClick.AddListener(() => OnFieldButtonLeftClicked?.Invoke(0));
        fieldButtonCenter.OnLeftClick.AddListener(() => OnFieldButtonLeftClicked?.Invoke(1));
        fieldButtonRight.OnLeftClick.AddListener(() => OnFieldButtonLeftClicked?.Invoke(2));
        
        fieldButtonLeft.OnLeftRelease.AddListener(() => OnFieldButtonLeftReleased?.Invoke(0));
        fieldButtonCenter.OnLeftRelease.AddListener(() => OnFieldButtonLeftReleased?.Invoke(1));
        fieldButtonRight.OnLeftRelease.AddListener(() => OnFieldButtonLeftReleased?.Invoke(2));
        
        Height = ((RectTransform) transform).rect.height;
    }

    public void SetupSong (ISongSettings settings, float songLength)
    {
        progress = 0;
        approachRate = Mathf.Approximately(settings.ApproachRate, default) ? 1 : settings.ApproachRate;
        this.songLength = songLength;
        
        beatInterval = 60f / settings.Bpm;
        ObjectsSpeed = Height / approachRate;
        totalHeight = (Height * songLength) / approachRate;
        songObjects.sizeDelta = new Vector2(songObjects.sizeDelta.x, totalHeight);
        
        float startingPosition = settings.StartingTime * ObjectsSpeed;
        ((RectTransform) separatorsParent.transform).localPosition = new Vector3(0, startingPosition, 0);
        ((RectTransform) notesParent.transform).localPosition = new Vector3(0, startingPosition, 0);
    }

    public EditorNoteView CreateNote (Note note)
    {
        EditorNoteView instance = Instantiate(editorNoteViewPrefab, notesParent);
        instance.transform.localPosition = new Vector3(GetNoteXPosition(note.Position), GetNoteYPosition(note.Time));
        instance.Note = note;
        return instance;
    }

    public EditorLongNoteView CreateLongNote (Note note)
    {
        EditorLongNoteView instance = Instantiate(editorLongNoteViewPrefab, notesParent);
        
        instance.transform.localPosition = new Vector3(GetNoteXPosition(note.Position), GetNoteYPosition(note.Time));
        instance.Note = note;
        instance.SetHeight(ObjectsSpeed * (note.EndTime - note.Time));
        return instance;
    }

    public void ClearSeparators ()
    {
        foreach (GameObject separator in separatorInstances)
            Destroy(separator);
        separatorInstances.Clear();
    }

    public void CreateSeparator (int color)
    {
        GameObject instance = Instantiate(separatorPrefab, separatorsParent.transform);
        instance.GetComponentInChildren<Image>().color = color switch
        {
            1 => beatColor,
            2 => halfBeatColor,
            3 => thirdBeatColor,
            4 => quarterBeatColor,
            _ => instance.GetComponentInChildren<Image>().color
        };
        instance.transform.SetAsFirstSibling();
        separatorInstances.Add(instance);
    }

    public void ChangeSeparatorsDistance (int interval)
    {
        separatorsParent.spacing = (beatInterval * Height) / approachRate / interval;
    }

    float GetNoteXPosition (int pos)
    {
        return pos switch
        {
            0 => leftNotesPosition.transform.localPosition.x,
            1 => centerNotesPosition.transform.localPosition.x,
            2 => rightNotesPosition.transform.localPosition.x,
            _ => throw new ArgumentOutOfRangeException($"Note position out of range! {pos}")
        };
    }

    float GetNoteYPosition (float time)
    {
        return (totalHeight * time) / songLength;
    }
    
    void Update ()
    {
        progress = ObjectsSpeed * audioManager.MusicTime;
        songObjects.anchoredPosition = new Vector2(0, -progress);
    }
}