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

    public void FlipSwitch()
    {
        if (!IsSolved("film_collected")) return;
        switchesFlipped++;
        if (switchesFlipped >= 3) CompletePuzzle("switches_done");
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