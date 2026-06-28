using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.UI; 

public class GridManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel; 
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject settingsPanel;   

    [Header("Dynamic Grid Setup")]
    [SerializeField] private GridLayoutGroup boardLayoutGroup; 
    [SerializeField] private GameObject tilePrefab;           

    [Header("Score & Text References")]
    [SerializeField] private TMP_Text scoreText;        
    [SerializeField] private TMP_Text bestScoreText;    

    [Header("Settings")]
    [SerializeField] private float moveDuration = 0.15f;

    private int boardSize = 4; 
    private int[,] grid;
    private int[,] tempGrid; 
    private List<Tile> dynamicTiles = new List<Tile>(); 

    private bool isMoving = false;
    private int currentScore = 0;
    private int bestScore = 0;
    private bool hasWon = false;

    private WaitForSeconds waitMove;        
    
    private List<int> tilesCache;
    private int[] rowCache;
    private List<Vector2Int> emptyPositionsCache;

    private void Awake()
    {
        waitMove = new WaitForSeconds(moveDuration);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateScoreUI();
    }

    private void Start()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        SetupGridSystem(4);
    }

    private void SetupGridSystem(int size)
    {
        boardSize = size;

        grid = new int[boardSize, boardSize];
        tempGrid = new int[boardSize, boardSize];
        tilesCache = new List<int>(boardSize);
        rowCache = new int[boardSize];
        emptyPositionsCache = new List<Vector2Int>(boardSize * boardSize);

        if (boardLayoutGroup != null)
        {
            boardLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            boardLayoutGroup.constraintCount = boardSize;
            float totalSpace = 1050f;
            float dynamicSpacing = 30f;
            if (boardSize == 4) dynamicSpacing = 30f;
            else if (boardSize == 5) dynamicSpacing = 22f;
            else if (boardSize == 6) dynamicSpacing = 16f;
            else dynamicSpacing = 12f;

            float totalPadding = 40f;
            float dynamicCellSize = (totalSpace - totalPadding - ((boardSize - 1) * dynamicSpacing)) / boardSize;

            boardLayoutGroup.spacing = new Vector2(dynamicSpacing, dynamicSpacing);
            boardLayoutGroup.cellSize = new Vector2(dynamicCellSize, dynamicCellSize);
            // -------------------------------------------------------------
        }
    }

    public void PlayGameFromMenu()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        RestartGame();
    }

    public void KeepPlaying()
    {
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        int nextSize = boardSize + 1;
        if (nextSize > 8) nextSize = 4;
        SetupGridSystem(nextSize);
        RestartGame();
    }

    public void RestartGame()
    {
        currentScore = 0;
        hasWon = false;
        UpdateScoreUI();
    
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        if (boardLayoutGroup != null)
        {
            foreach (Transform child in boardLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }
        dynamicTiles.Clear();

        for (int i = 0; i < boardSize * boardSize; i++)
        {
            GameObject newTileObj = Instantiate(tilePrefab, boardLayoutGroup.transform);
            Tile tileComponent = newTileObj.GetComponent<Tile>();
            if (tileComponent != null)
            {
                dynamicTiles.Add(tileComponent);
                tileComponent.gameObject.SetActive(false); 
            }
        }

        System.Array.Clear(grid, 0, grid.Length);

        SpawnTile();
        SpawnTile();

        StartCoroutine(DelayedUIActivation());
    
        isMoving = false;
    }

    private IEnumerator DelayedUIActivation()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < dynamicTiles.Count; i++)
        {
            if (dynamicTiles[i] != null)
            {
                dynamicTiles[i].gameObject.SetActive(true);
            }
        }

        UpdateUI();
    }

    private void Update()
    {
        if (isMoving) return;
        if ((startMenuPanel != null && startMenuPanel.activeSelf) || 
            (gameOverPanel != null && gameOverPanel.activeSelf) ||
            (gameWinPanel != null && gameWinPanel.activeSelf) ||
            (settingsPanel != null && settingsPanel.activeSelf)) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))  StartCoroutine(MoveSequence(0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(MoveSequence(2));
        if (Input.GetKeyDown(KeyCode.UpArrow))    StartCoroutine(MoveSequence(3));
        if (Input.GetKeyDown(KeyCode.DownArrow))  StartCoroutine(MoveSequence(1));
    }

    private IEnumerator MoveSequence(int rotation)
    {
        isMoving = true;
        RotateGrid(rotation);

        bool changed = false;

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++) rowCache[j] = grid[i, j];

            if (SlideAndMerge(rowCache)) changed = true;

            for (int j = 0; j < boardSize; j++) grid[i, j] = rowCache[j];
        }

        RotateGrid((4 - rotation) % 4);

        if (changed)
        {
            UpdateUI(); 
            yield return waitMove;
            SpawnTile();
            UpdateUI();
            
            if (!hasWon && CheckWinCondition())
            {
                TriggerGameWin();
            }
            else if (CheckGameOver())
            {
                TriggerGameOver();
            }
        }

        isMoving = false;
    }

    private bool SlideAndMerge(int[] line)
    {
        bool changed = false;
        tilesCache.Clear();

        for (int i = 0; i < boardSize; i++)
            if (line[i] != 0) tilesCache.Add(line[i]);

        for (int i = 0; i < tilesCache.Count - 1; i++)
        {
            if (tilesCache[i] == tilesCache[i + 1])
            {
                tilesCache[i] *= 2;
                currentScore += tilesCache[i];
                tilesCache.RemoveAt(i + 1);
                changed = true;
                i++;
            }
        }

        for (int i = 0; i < boardSize; i++)
        {
            int newVal = i < tilesCache.Count ? tilesCache[i] : 0;
            if (newVal != line[i])
            {
                changed = true;
                line[i] = newVal;
            }
        }

        return changed;
    }

    private bool CheckGameOver()
    {
        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize; j++)
                if (grid[i, j] == 0) return false;

        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize - 1; j++)
                if (grid[i, j] == grid[i, j + 1]) return false;

        for (int i = 0; i < boardSize - 1; i++)
            for (int j = 0; j < boardSize; j++)
                if (grid[i, j] == grid[i + 1, j]) return false;

        return true;
    }

    private void RotateGrid(int times)
    {
        for (int t = 0; t < times; t++)
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    tempGrid[j, (boardSize - 1) - i] = grid[i, j];

            System.Array.Copy(tempGrid, grid, grid.Length);
        }
    }

    private void SpawnTile()
    {
        emptyPositionsCache.Clear();
        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize; j++)
                if (grid[i, j] == 0) emptyPositionsCache.Add(new Vector2Int(i, j));

        if (emptyPositionsCache.Count == 0) return;

        Vector2Int pos = emptyPositionsCache[Random.Range(0, emptyPositionsCache.Count)];
        grid[pos.x, pos.y] = Random.value < 0.9f ? 2 : 4;
    }

    private void UpdateUI()
    {
        UpdateScoreUI();
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                int index = i * boardSize + j;
                if (index < dynamicTiles.Count && dynamicTiles[index] != null)
                {
                    dynamicTiles[index].SetValue(grid[i, j]);
                }
            }
        }
    }

    private bool CheckWinCondition()
    {
        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize; j++)
                if (grid[i, j] == 2048) return true;
        return false;
    }

    private void TriggerGameWin()
    {
        hasWon = true;
        if (gameWinPanel == null) return;
        gameWinPanel.SetActive(true);
        CanvasGroup cg = gameWinPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, 0.5f).SetUpdate(true);
        }
    }

    private void TriggerGameOver()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);
        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, 0.5f).SetUpdate(true);
        }
    }

    private void UpdateScoreUI()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        if (scoreText != null) scoreText.text = currentScore.ToString();
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
    }

    public void ToggleSettings(bool isOpen)
    {
        if (settingsPanel == null) return;
        if (isOpen)
        {
            settingsPanel.SetActive(true);
            CanvasGroup cg = settingsPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.DOFade(1f, 0.25f).SetUpdate(true);
            }
        }
        else
        {
            CanvasGroup cg = settingsPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() => settingsPanel.SetActive(false));
            }
            else
            {
                settingsPanel.SetActive(false);
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        Application.Quit(); 
#endif
    }
}