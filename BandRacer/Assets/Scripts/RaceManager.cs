using System;
using System.Globalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public enum RaceGameState
    {
        Idle,
        WaitingForPlayers,
        CountDownToRace,
        Racing,
        RaceFinishedWaitForRestart
    }

    public static RaceManager Instance { get; private set; }

    public Text Player1Name;
    public Text Player2Name;

    public Text Player1Time;
    public Text Player2Time;

    public Text WinnerText;

    public Text CountDown;

    public GameObject RedLight;
    public GameObject GreenLight;

    public Collider2D GoalLineCollider2D;
    public GameObject Player1;
    public GameObject Player2;

    public Animator TapToRestartAnimator;

    public int NumberOfLapsToRace = 1;
    public float CoundDownCounter;
    private bool m_RestartGame;

    public bool IsPlayer1OnStartLine;
    public bool IsPlayer2OnStartLine;

    private int m_Player1Laps;
    private int m_Player2Laps;

    private float m_Player1Time;
    private float m_Player2Time;

    public RaceGameState m_RaceGameState;
    private Vector3 m_Player1Position;
    private Vector3 m_player2Position;

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RetriveInitialPlayerPositions();
    }

    void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (m_RaceGameState)
        {
            case RaceGameState.Idle:
                RaceGameStateProperty = GotoNextState(ResetGameData, m_RaceGameState);
                break;
            case RaceGameState.WaitingForPlayers:
                RaceGameStateProperty = GotoNextState(UpdateAllPlayersJoined, m_RaceGameState);
                break;
            case RaceGameState.CountDownToRace:
                RaceGameStateProperty = GotoNextState(UpdateCountDownCounter, m_RaceGameState);
                break;
            case RaceGameState.Racing:
                RaceGameStateProperty = GotoNextState(UpdateRacers, m_RaceGameState);
                break;
            case RaceGameState.RaceFinishedWaitForRestart:
                RaceGameStateProperty = GotoNextState(ShowRaceResult, m_RaceGameState);
                break;
            default:
                RaceGameStateProperty = RaceGameState.Idle;
                break;
        }
    }

    private bool ShowRaceResult()
    {
        if (m_Player1Time < m_Player2Time)
        {
            WinnerTextProperty = string.Format("{0} wins!", Player1NameText);
        }
        if (m_Player2Time < m_Player1Time)
        {
            WinnerTextProperty = string.Format("{0} wins!", Player2NameText);
        }

        TapToRestartAnimator.SetTrigger("StartAnimation");

        return m_RestartGame;
    }

    private bool UpdateRacers()
    {
        bool hasAllRacersFinished = HasPlayer1Finished() && HasPlayer2Finished();

        bool player1LeftStartLine = IsPlayer1OnStartLine && !IsPlayerOnGoalLine(Player1);
        if (player1LeftStartLine) // Player left the goal line
        {
            IsPlayer1OnStartLine = false;
        }
        else
        {
            bool player1EnteredGoalLine = !IsPlayer1OnStartLine && IsPlayerOnGoalLine(Player1);
            if (player1EnteredGoalLine)
            {
                IsPlayer1OnStartLine = true;
                m_Player1Laps++;
            }
        }

        bool player2LeftStartLine = IsPlayer2OnStartLine && !IsPlayerOnGoalLine(Player2);
        if (player2LeftStartLine)
        {
            IsPlayer2OnStartLine = false;
        }
        else
        {
            bool player2EnteredGoalLine = !IsPlayer2OnStartLine && IsPlayerOnGoalLine(Player2);
            if (player2EnteredGoalLine)
            {
                IsPlayer2OnStartLine = true;
                m_Player2Laps++;
            }
        }

        UpdateRacersLapTimes();
        return hasAllRacersFinished;
    }

    private bool HasPlayer1Finished()
    {
        return m_Player1Laps >= NumberOfLapsToRace;
    }

    private bool HasPlayer2Finished()
    {
        return m_Player2Laps >= NumberOfLapsToRace;
    }

    private void UpdateRacersLapTimes()
    {
        if (!HasPlayer1Finished())
        {
            m_Player1Time += Time.deltaTime;
        }

        if (!HasPlayer2Finished())
        {
            m_Player2Time += Time.deltaTime;
        }

        var player1TimeSpan = TimeSpan.FromSeconds(m_Player1Time);
        Player1TimeText = string.Format("{0}:{1:00}.{2:000}", player1TimeSpan.Minutes, player1TimeSpan.Seconds, player1TimeSpan.Milliseconds);

        var player2TimeSpan = TimeSpan.FromSeconds(m_Player2Time);
        Player2TimeText = string.Format("{0}:{1:00}.{2:000}", player2TimeSpan.Minutes, player2TimeSpan.Seconds, player2TimeSpan.Milliseconds);
    }

    private void RetriveInitialPlayerPositions()
    {
        m_Player1Position = Player1.transform.position;
        m_player2Position = Player2.transform.position;
    }

    private bool UpdateCountDownCounter()
    {
        CoundDownCounter -= Time.deltaTime;
        if (CoundDownCounter < 0f)
        {
            SetTextValue(CountDown, string.Empty);
            SetImageAlpha(RedLight, 0.1f);
            SetImageAlpha(GreenLight, 1f);
            TapToRestartAnimator.ResetTrigger("ResetAnimation");
            RaceStarted = true;
            return true;
        }
        else if (CoundDownCounter > 2f)
        {
            SetTextValue(CountDown, "3");
        }
        else if (CoundDownCounter > 1f)
        {
            TapToRestartAnimator.ResetTrigger("ResetAnimation");
            SetTextValue(CountDown, "2");
        }

        else if (CoundDownCounter > 0f)
        {
            SetTextValue(CountDown, "1");
        }
        return false;
    }

    public RaceGameState RaceGameStateProperty
    {
        get { return m_RaceGameState; }
        set
        {
            if (m_RaceGameState == value)
                return;

            Debug.Log("Old state: " + m_RaceGameState + " New state: " + value);
            m_RaceGameState = value;
        }
    }

    private bool UpdateAllPlayersJoined()
    {
#if UNITY_EDITOR
        Player1NameText = "keyboard";
        Player2NameText = "mouse";

        IsPlayer1OnStartLine = IsPlayerOnGoalLine(Player1);
        IsPlayer2OnStartLine = IsPlayerOnGoalLine(Player2);
#else
        if (BandRacer.Platform.Platform.Current == null)
        {
            BandRacer.Plugin.PlatformBase.Current = new BandRacer.Platform.Platform();
        }

        Player1NameText = BandRacer.Platform.Platform.Current.Player1Name;
        Player2NameText = BandRacer.Platform.Platform.Current.Player2Name;

        IsPlayer1OnStartLine = !string.IsNullOrEmpty(Player1NameText);
        IsPlayer2OnStartLine = !string.IsNullOrEmpty(Player2NameText);
#endif
        return (IsPlayer1OnStartLine && IsPlayer2OnStartLine);
    }

    private bool IsPlayerOnGoalLine(GameObject player)
    {
        return player.GetComponents<Collider2D>().Any(playerCollider => GoalLineCollider2D.IsTouching(playerCollider));
    }

    private static TEnum GotoNextState<TEnum>(Func<bool> action, TEnum currentState) where TEnum : struct, IComparable, IFormattable
    {
        bool proceedToNextState = action();
        if (!proceedToNextState)
            return currentState;

        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        var enumerable = values as TEnum[] ?? values.ToArray();
        var next = enumerable.SkipWhile(i =>
        {
            return (!i.ToString().ToLower().Equals(currentState.ToString().Trim().ToLower()));
        }).Skip(1).FirstOrDefault();

        return next;
    }

    private bool ResetGameData()
    {
        SetImageAlpha(RedLight, 1f);
        SetImageAlpha(GreenLight, 0.1f);

        Player1TimeText = "0:00.000";
        Player2TimeText = "0:00.000";

        m_Player1Laps = 0;
        m_Player2Laps = 0;

        CoundDownCounter = 3f;
        RaceStarted = false;
        m_RestartGame = false;

        m_Player1Time = 0f;
        m_Player2Time = 0f;

        WinnerTextProperty = string.Empty;

        TapToRestartAnimator.ResetTrigger("StartAnimation");

        Player1.transform.position = m_Player1Position;
        Player1.transform.localEulerAngles = new Vector3(0f,0f,0f);
        Player2.transform.position = m_player2Position;
        Player2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        return true;
    }

    public void RestartGame()
    {
        if (RaceGameStateProperty == RaceGameState.RaceFinishedWaitForRestart)
        {
            TapToRestartAnimator.SetTrigger("ResetAnimation");
            m_RestartGame = true;
        }
    }

    private void SetImageAlpha(GameObject gameObjectWithImageComponent, float alpha)
    {
        if (gameObjectWithImageComponent == null)
            return;

        var image = gameObjectWithImageComponent.GetComponent<Image>();
        if (image == null)
            return;

        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    private string GetTextValue(Text text)
    {
        if (text != null)
        {
            return text.text;
        }
        return string.Empty;
    }

    private void SetTextValue(Text text, string playerName)
    {
        if (text != null)
        {
            text.text = playerName;
        }
    }

    public bool RaceStarted { get; private set; }

    private string Player1NameText
    {
        get { return GetTextValue(Player1Name); }
        set { SetTextValue(Player1Name, value); }
    }

    private string Player1TimeText
    {
        get { return GetTextValue(Player1Time); }
        set { SetTextValue(Player1Time, value); }
    }

    private string Player2NameText
    {
        get { return GetTextValue(Player2Name); }
        set { SetTextValue(Player2Name, value); }
    }

    private string Player2TimeText
    {
        get { return GetTextValue(Player2Time); }
        set { SetTextValue(Player2Time, value); }
    }

    private string WinnerTextProperty
    {
        get { return GetTextValue(WinnerText); }
        set { SetTextValue(WinnerText, value); }
    }
}
