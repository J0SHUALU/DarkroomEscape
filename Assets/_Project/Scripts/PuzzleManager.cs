using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Progress")]
    public int totalPuzzles = 5;
    public int completedPuzzles = 0;

    [Header("UI")]
    public TMP_Text progressText;
    public TMP_Text timerText;
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Door")]
    public Interactable door;

    [Header("Timer")]
    public float timeLimit = 300f;
    private float timeRemaining;
    private bool gameActive = true;

    [Header("Sequence Reset - drag references here")]
    public Light redLightRef;
    public GameObject candleHintRef;       // the Point on your puzzle 1 candle
    public GameObject puzzle2RevealsRef;   // the parent containing book + switch hints
    public int sequenceProgress = 0;

    private HashSet<string> solvedIds = new HashSet<string>();
    private int switchesFlipped = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timeRemaining = timeLimit;
        UpdateProgressUI();
    }

    void Update()
    {
        if (!gameActive) return;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0) TriggerLose();
        UpdateTimerUI();
    }

    public void CompletePuzzle(string id)
    {
        if (solvedIds.Contains(id)) return;
        solvedIds.Add(id);
        completedPuzzles++;
        Debug.Log($"Solved: {id} ({completedPuzzles}/{totalPuzzles})");
        UpdateProgressUI();
        if (completedPuzzles >= totalPuzzles && door != null) door.isLocked = false;
    }

    public bool IsSolved(string id) => solvedIds.Contains(id);

    // For ordered switch sequence
    public bool TryFlipSwitch(int order)
    {
        if (!IsSolved("film_collected")) return false;

        sequenceProgress++;
        Debug.Log($"Attempted switch {order}, expected {sequenceProgress}");

        if (order != sequenceProgress)
        {
            // WRONG ORDER - reset everything
            Debug.Log("WRONG ORDER - sequence reset, room going dark");
            ResetSequence();
            return false;
        }

        switchesFlipped++;
        Debug.Log($"Switches: {switchesFlipped}/3");
        if (switchesFlipped >= 3) CompletePuzzle("switches_done");
        return true;
    }

    public void ResetSequence()
    {
        // Turn off red light
        if (redLightRef != null) redLightRef.enabled = false;

        // Re-enable candle's own hint glow
        if (candleHintRef != null) candleHintRef.SetActive(true);

        // Re-hide book + switch hints (the puzzle 2 reveal container)
        if (puzzle2RevealsRef != null) puzzle2RevealsRef.SetActive(false);

        // Reset counters and solved IDs
        solvedIds.Remove("red_on");
        solvedIds.Remove("film_collected");
        completedPuzzles = solvedIds.Count;
        switchesFlipped = 0;
        sequenceProgress = 0;

        // Reset all Interactables' used flags so they can be clicked again
        Interactable[] all = Object.FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var i in all)
        {
            if (i.type == Interactable.Type.RedChain ||
                i.type == Interactable.Type.Tray ||
                i.type == Interactable.Type.Switch)
            {
                i.ResetUsed();
            }
        }

        UpdateProgressUI();
    }

    public void TriggerWin()
    {
        gameActive = false;
        if (winScreen != null) winScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TriggerLose()
    {
        gameActive = false;
        if (loseScreen != null) loseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"Puzzle Progress: {completedPuzzles} / {totalPuzzles}";
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int m = Mathf.FloorToInt(timeRemaining / 60);
            int s = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{m:00}:{s:00}";
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}