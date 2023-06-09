using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] SongView songView;
    [SerializeField] ComboView comboView;
    [SerializeField] PauseView pauseView;
    [SerializeField] ScoreView scoreView;
    [SerializeField] ResultsView resultsView;

    public SongView SongView => songView;
    public ComboView ComboView => comboView;
    public PauseView PauseView => pauseView;
    public ScoreView ScoreView => scoreView;
    public ResultsView ResultsView => resultsView;
}