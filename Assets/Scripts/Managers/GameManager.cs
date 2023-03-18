﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameView view;
    [SerializeField] InputManager inputManager;

    public GameModel Model { get; private set; }
    public GameController Controller { get; private set; }
    public GameView View => view;

    void Start ()
    {
        Model = GameModelFactory.Create(inputManager);
        Controller = GameControllerFactory.Create(View, Model);
        Initialize();
    }

    void OnDestroy ()
    {
        Model.Dispose();
        Controller.Dispose();
    }

    void Initialize ()
    {
        Model.Initialize();
        Controller.Initialize();
    }
}