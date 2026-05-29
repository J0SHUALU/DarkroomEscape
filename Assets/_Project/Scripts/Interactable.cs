using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum Type { RedChain, Tray, Switch, WhiteChain, Dial, Door }

    [Header("Config")]
    public Type type;
    public string interactionPrompt = "Click";
    public bool isLocked = false;

    [Header("References (optional)")]
    public GameObject objectToReveal;
    public GameObject objectToHide;
    public Light lightToToggle;
    public AudioClip sound;

    [Header("Switch settings (only for Type.Switch)")]
    public int switchOrder = 1; // 1, 2, or 3 — order this switch must be flipped

    [Header("Dial settings")]
    public int correctDigit;
    public int currentDigit = 0;
    public TMPro.TMP_Text digitDisplay;

    private bool used = false;

    public void ResetUsed() { used = false; }

    public void Interact()
    {
        if (isLocked) return;

        switch (type)
        {
            case Type.RedChain:
                if (used) return;
                used = true;
                if (lightToToggle != null) lightToToggle.enabled = true;
                if (objectToHide != null) objectToHide.SetActive(false);
                if (objectToReveal != null) objectToReveal.SetActive(true);
                Play();
                PuzzleManager.Instance.CompletePuzzle("red_on");
                break;

            case Type.Tray:
                if (used) return;
                if (!PuzzleManager.Instance.IsSolved("red_on")) return;
                used = true;
                if (objectToReveal != null) objectToReveal.SetActive(true);
                if (objectToHide != null) objectToHide.SetActive(false);
                Play();
                PuzzleManager.Instance.CompletePuzzle("film_collected");
                break;

            case Type.Switch:
                if (used) return;
                if (!PuzzleManager.Instance.IsSolved("film_collected")) return;
                bool correct = PuzzleManager.Instance.TryFlipSwitch(switchOrder);
                if (correct)
                {
                    used = true;
                    transform.Rotate(0, 0, 30);
                    if (objectToHide != null) objectToHide.SetActive(false);
                    if (objectToReveal != null) objectToReveal.SetActive(true);
                    Play();
                }
                break;

            case Type.WhiteChain:
                if (used) return;
                if (!PuzzleManager.Instance.IsSolved("switches_done")) return;
                used = true;
                if (lightToToggle != null) lightToToggle.enabled = true;
                if (objectToHide != null) objectToHide.SetActive(false);
                if (objectToReveal != null) objectToReveal.SetActive(true);
                Play();
                PuzzleManager.Instance.CompletePuzzle("white_on");
                break;

            case Type.Dial:
                if (!PuzzleManager.Instance.IsSolved("white_on")) return;
                currentDigit = (currentDigit + 1) % 10;
                if (digitDisplay != null) digitDisplay.text = currentDigit.ToString();
                transform.Rotate(0, 36, 0);
                if (objectToHide != null) objectToHide.SetActive(false);
                if (objectToReveal != null) objectToReveal.SetActive(true);
                Play();
                CheckCombination();
                break;

            case Type.Door:
                if (isLocked) return;
                transform.Rotate(0, 90, 0);
                Play();
                Invoke(nameof(WinAfterDelay), 1.5f);
                break;
        }
    }

    void CheckCombination()
    {
        Interactable[] all = Object.FindObjectsByType<Interactable>();
        foreach (var i in all)
            if (i.type == Type.Dial && i.currentDigit != i.correctDigit) return;
        PuzzleManager.Instance.CompletePuzzle("combo_set");
    }

    void Play()
    {
        if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
    }

    void WinAfterDelay() => PuzzleManager.Instance.TriggerWin();
}