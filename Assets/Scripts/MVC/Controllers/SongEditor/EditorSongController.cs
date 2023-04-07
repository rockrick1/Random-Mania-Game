﻿using System;

public class EditorSongController : IDisposable
{
    readonly EditorSongView view;
    readonly IEditorSongModel model;
    readonly ISongLoaderModel songLoaderModel;

    public EditorSongController (EditorSongView view, IEditorSongModel model, ISongLoaderModel songLoaderModel)
    {
        this.view = view;
        this.model = model;
        this.songLoaderModel = songLoaderModel;
    }

    public void Initialize ()
    {
        AddListeners();
    }

    void AddListeners ()
    {
        view.OnFieldButtonClicked += HandleFieldButtonClicked;
        songLoaderModel.OnSongLoaded += HandleSongLoaded;
    }

    void RemoveListeners ()
    {
        view.OnFieldButtonClicked -= HandleFieldButtonClicked;
        songLoaderModel.OnSongLoaded -= HandleSongLoaded;
    }

    void HandleFieldButtonClicked (int pos)
    {
        model.ButtonClicked(pos);
    }

    void HandleSongLoaded ()
    {
        CreateNotes(songLoaderModel.Settings);
    }

    void CreateNotes (ISongSettings settings)
    {
        foreach (Note note in settings.Notes)
            view.SpawnNote(note);
    }

    public void Dispose ()
    {
        RemoveListeners();
    }
}