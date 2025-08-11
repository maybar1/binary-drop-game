using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Panels")]
    public GameObject welcomePanel;
    public GameObject instructionsPanel;
    public GameObject gameUIPanel;
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject panelLevel;
    public GameObject finalMessagePanel;

    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI sequenceText;
    public TextMeshProUGUI[] bitSlots;
    public TextMeshProUGUI[] numberLabels;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI[] targetLabels;

    [Header("Managers")]
    public GridManager gridManager;

    [Header("Gameplay Settings")]
    public int requiredSelections;
    public List<int> targetNumbers;

    private int currentLevel = 1;
    private List<BinaryTile> currentSelection = new List<BinaryTile>();
    private HashSet<int> foundNumbers = new HashSet<int>();
    private bool isDragging = false;
    private float levelTimer = 60f;
    private bool timerRunning = false;
    private bool isBlinking = false;
    private bool hasShownInstructions = false;

    void Awake()
    {
        Instance = this;
    }
    private void InitializePanels()
    {
        welcomePanel.SetActive(true);
        instructionsPanel.SetActive(false);
        gameUIPanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        panelLevel.SetActive(false);
        finalMessagePanel.SetActive(false);
    }


    void Start() => InitializePanels();

    public void OnStartButton()
    {
        welcomePanel.SetActive(false);
        gameUIPanel.SetActive(true);
        panelLevel.SetActive(true);

        SetLevel(1);

        if (!hasShownInstructions)
        {
            instructionsPanel.SetActive(true);
            hasShownInstructions = true;
            timerRunning = false;
        }
        else
        {
            timerRunning = true;
        }
    }

    public void OnContinueButton()
    {
        instructionsPanel.SetActive(false);
        timerRunning = true;
    }

    //Restart Game 
    public void OnRestartGame()
    {
        InitializePanels();
        currentLevel = 1;
        hasShownInstructions = false;
        timerRunning = false;
    }

    //Restart Level after loss
    public void OnLoseRestart()
    {
        losePanel.SetActive(false);
        SetLevel(currentLevel);
        timerRunning = true;
    }

    // Move on to next level
    public void OnNextLevelButton()
    {
        winPanel.SetActive(false);
        currentLevel++;

        if (currentLevel > 3)
        {
            ShowFinalMessage();
            return;
        }

        SetLevel(currentLevel);
        timerRunning = true;
    }
    public void ShowFinalMessage()
    {
        finalMessagePanel.SetActive(true);
    }

    // Set the level and initialize the game state
    public void SetLevel(int levelIndex)
    {
        ResetNumberLabelColors();
        levelText.text = "Level " + levelIndex;
        currentLevel = levelIndex;
        foundNumbers.Clear();

        int maxValue = 0;
        int numTargets = 0;

        if (levelIndex == 1)
        {
            requiredSelections = 3;
            maxValue = 7;
            numTargets = 5;
            UpdateBitLabels(new List<string> { "4", "2", "1" });
        }
        else if (levelIndex == 2)
        {
            requiredSelections = 4;
            maxValue = 15;
            numTargets = 5;
            UpdateBitLabels(new List<string> { "8", "4", "2", "1" });
        }
        else if (levelIndex == 3)
        {
            requiredSelections = 5;
            maxValue = 30;
            numTargets = 5;
            UpdateBitLabels(new List<string> { "16", "8", "4", "2", "1" });
        }

        gridManager.GenerateGrid(maxValue, requiredSelections);
        targetNumbers = gridManager.GenerateTargetsFromGrid(requiredSelections, numTargets);
        UpdateTargetLabels();

        levelTimer = 60f;
        UpdateTimerUI();
    }
    public void StartSelection(BinaryTile tile)
    {
        currentSelection.Clear();
        isDragging = true;
        currentSelection.Add(tile);
        tile.Highlight();
        UpdateBitSlots();
        ShowCurrentSelectionResult();
    }
    public void ContinueSelection(BinaryTile tile)
    {
        if (!isDragging || currentSelection.Contains(tile)) return;
        if (currentSelection.Count >= requiredSelections) return;

        BinaryTile last = currentSelection.Last();
        if (AreAdjacent(last, tile))
        {
            currentSelection.Add(tile);
            tile.Highlight();
            UpdateBitSlots();
            ShowCurrentSelectionResult();
        }
    }
    public void EndSelection()
    {
        isDragging = false;

        if (currentSelection.Count == requiredSelections)
            EvaluateSelection(currentSelection);
        else
        {
            foreach (var tile in currentSelection) tile.ResetHighlight();
            currentSelection.Clear();
            UpdateBitSlots();
            sequenceText.text = "";
        }
    }

    void ShowCurrentSelectionResult()
    {
        if (currentSelection.Count > 0)
        {
            string binaryString = string.Join("", currentSelection.Select(t => t.BitValue));
            if (binaryString.Length < requiredSelections)
                binaryString = binaryString.PadRight(requiredSelections, '0');

            int decimalValue = System.Convert.ToInt32(binaryString, 2);
            sequenceText.text = "=" + decimalValue.ToString();
        }
        else
            sequenceText.text = "";
    }

    void EvaluateSelection(List<BinaryTile> tiles)
    {
        string binaryString = string.Join("", tiles.Select(t => t.BitValue));
        int decimalValue = System.Convert.ToInt32(binaryString, 2);

        if (foundNumbers.Contains(decimalValue))
        {
            // Already found number, no action needed
        }

        else if (targetNumbers.Contains(decimalValue))
        {
            AudioManager.Instance.PlaySuccessSFX();

            foreach (var tile in tiles)
                StartCoroutine(RotateTile(tile));

            HighlightNumber(decimalValue);
            foundNumbers.Add(decimalValue);

            if (foundNumbers.Count == targetNumbers.Count)
            {
                winPanel.SetActive(true);
                timerRunning = false;
            }
        }

        foreach (var tile in tiles) tile.ResetHighlight();
        currentSelection.Clear();
        UpdateBitSlots();
    }


    void Update()
    {
        if (timerRunning)
        {
            levelTimer -= Time.deltaTime;
            if (levelTimer <= 0)
            {
                levelTimer = 0;
                timerRunning = false;
                OnTimeOut();
            }
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(levelTimer / 60f);
        int seconds = Mathf.FloorToInt(levelTimer % 60f);
        timerText.text = $"{minutes}:{seconds:00}";

        if (levelTimer > 30f)
            timerText.color = Color.green;
        else if (levelTimer > 10f)
            timerText.color = new Color(1f, 0.64f, 0f);
        else
        {
            timerText.color = Color.red;
            StartCoroutine(BlinkTimer());
        }
    }

    private IEnumerator BlinkTimer()
    {
        if (isBlinking) yield break;
        isBlinking = true;

        while (levelTimer > 0 && levelTimer <= 10f)
        {
            timerText.enabled = !timerText.enabled;
            yield return new WaitForSeconds(0.5f);
        }

        timerText.enabled = true;
        isBlinking = false;
    }

    void OnTimeOut()
    {
        losePanel.SetActive(true);
        timerRunning = false;
    }
    void UpdateTargetLabels()
    {
        for (int i = 0; i < targetLabels.Length; i++)
        {
            if (i < targetNumbers.Count)
            {
                targetLabels[i].gameObject.SetActive(true);
                targetLabels[i].text = targetNumbers[i].ToString();
            }
            else
            {
                targetLabels[i].gameObject.SetActive(false);
            }
        }
    }

    bool AreAdjacent(BinaryTile a, BinaryTile b)
    {
        return Mathf.Abs(a.Row - b.Row) + Mathf.Abs(a.Col - b.Col) == 1;
    }

    void HighlightNumber(int number)
    {
        for (int i = 0; i < targetNumbers.Count; i++)
            if (targetNumbers[i] == number)
                numberLabels[i].color = Color.green;
    }

    void ResetNumberLabelColors()
    {
        for (int i = 0; i < numberLabels.Length; i++)
            numberLabels[i].color = Color.black;
    }

    private IEnumerator RotateTile(BinaryTile tile)
    {
        float duration = 0.3f;
        float halfDuration = duration / 2f;
        Quaternion startRot = tile.transform.rotation;
        Quaternion midRot = Quaternion.Euler(0, 90, 0);

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            tile.transform.rotation = Quaternion.Slerp(startRot, midRot, t / halfDuration);
            yield return null;
        }

        tile.Toggle();

        Quaternion endRot = Quaternion.Euler(0, 0, 0);
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            tile.transform.rotation = Quaternion.Slerp(midRot, endRot, t / halfDuration);
            yield return null;
        }
    }

    private void UpdateBitLabels(List<string> labels)
    {
        int offset = numberLabels.Length - labels.Count;
        for (int i = 0; i < numberLabels.Length; i++)
        {
            if (i >= offset)
            {
                int labelIndex = i - offset;
                numberLabels[i].gameObject.SetActive(true);
                numberLabels[i].text = labels[labelIndex];
                bitSlots[i].gameObject.SetActive(true);
            }
            else
            {
                numberLabels[i].gameObject.SetActive(false);
                bitSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateBitSlots()
    {
        int slotIdx = 0;
        for (int i = 0; i < bitSlots.Length; i++)
        {
            if (bitSlots[i].gameObject.activeSelf)
            {
                if (slotIdx < currentSelection.Count)
                    bitSlots[i].text = currentSelection[slotIdx].BitValue.ToString();
                else
                    bitSlots[i].text = "";
                slotIdx++;
            }
            else
                bitSlots[i].text = "";
        }
    }

}
