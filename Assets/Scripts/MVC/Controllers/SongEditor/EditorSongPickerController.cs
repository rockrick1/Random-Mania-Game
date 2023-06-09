﻿using System;
using UnityEngine;

public class EditorSongPickerController : IDisposable
{
    readonly EditorSongPickerView view;
    readonly EditorNewSongController newSongController;
    readonly IEditorSongPickerModel model;
    readonly ISongLoaderModel songLoaderModel;

    public EditorSongPickerController (
        EditorSongPickerView view,
        EditorNewSongController newSongController,
        IEditorSongPickerModel model,
        ISongLoaderModel songLoaderModel
    )
    {
        this.view = view;
        this.newSongController = newSongController;
        this.model = model;
        this.songLoaderModel = songLoaderModel;
    }

    public void Initialize ()
    {
        AddListeners();
        RefreshOptions();
    }

    void AddListeners ()
    {
        newSongController.OnEditNewSong += HandleEditNewSong;
        songLoaderModel.OnSongCreated += HandleSongCreated;
        view.OnSongPicked += HandleSongPicked;
        view.OnOpenFolderClicked += HandleOpenFolderClicked;
        view.OnNewSongClicked += HandleNewSongClicked;
        view.OnRefreshClicked += HandleRefreshClicked;
    }

    void RemoveListeners ()
    {
        newSongController.OnEditNewSong -= HandleEditNewSong;
        songLoaderModel.OnSongCreated -= HandleSongCreated;
        view.OnSongPicked -= HandleSongPicked;
        view.OnOpenFolderClicked -= HandleOpenFolderClicked;
        view.OnNewSongClicked -= HandleNewSongClicked;
        view.OnRefreshClicked -= HandleRefreshClicked;
    }

    void HandleEditNewSong (string songId)
    {
        view.PickSong(songId);
        model.PickSong(songId);
    }

    void HandleSongCreated () => RefreshOptions();

    void HandleSongPicked (string songId)
    {
        model.PickSong(songId);
    }

    void HandleOpenFolderClicked () => Application.OpenURL($"file://{songLoaderModel.SongsPath}");

    void HandleNewSongClicked () => newSongController.Open();

    void HandleRefreshClicked () => RefreshOptions();

    void RefreshOptions () => view.LoadOptions(songLoaderModel.GetAllSongDirs());

    public void Dispose ()
    {
        RemoveListeners();
    }
}