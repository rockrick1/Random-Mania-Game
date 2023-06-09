﻿using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorSongDetailsView : MonoBehaviour
{
    public event Action OnApplyClicked;
    public event Action<string> OnSignatureChanged;
    public event Action<string> OnPlaybackSpeedChanged;
    public event Action<bool> OnShowWaveClicked;

    [SerializeField] TMP_InputField bpmInput;
    [SerializeField] TMP_InputField arInput;
    [SerializeField] TMP_InputField diffInput;
    [SerializeField] TMP_InputField startingTimeInput;
    [SerializeField] Button setStartingTimeButton;
    [SerializeField] Button applyButton;
    [SerializeField] TMP_Dropdown signatureDropdown;
    [SerializeField] TMP_Dropdown playbackSpeedDropdown;
    [SerializeField] Toggle showWaveToggle;

    public string BpmValue => bpmInput.text;
    public string ArValue => arInput.text;
    public string DiffValue => diffInput.text;
    public string StartingTimeValue => startingTimeInput.text;

    AudioManager audioManager;

    void Awake ()
    {
        audioManager = AudioManager.GetOrCreate();
        setStartingTimeButton.onClick.AddListener(HandleSetStartingTime);
        applyButton.onClick.AddListener(() => OnApplyClicked?.Invoke());
        showWaveToggle.onValueChanged.AddListener((value) => OnShowWaveClicked?.Invoke(value));
    }

    public void SetBPM (float val) => bpmInput.text = val.ToString(CultureInfo.CurrentCulture);
    public void SetAR (float val) => arInput.text = val.ToString(CultureInfo.CurrentCulture);
    public void SetDiff (float val) => diffInput.text = val.ToString(CultureInfo.CurrentCulture);
    public void SetStartingTime (float val) => startingTimeInput.text = val.ToString(CultureInfo.CurrentCulture);

    public void HandleSignatureChanged ()
    {
        OnSignatureChanged?.Invoke(signatureDropdown.options[signatureDropdown.value].text);
    }

    public void HandlePlaybackSpeedChanged ()
    {
        OnPlaybackSpeedChanged?.Invoke(playbackSpeedDropdown.options[playbackSpeedDropdown.value].text);
    }

    void HandleSetStartingTime ()
    {
        startingTimeInput.text = audioManager.MusicTime.ToString(CultureInfo.CurrentCulture);
    }
}