using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class GridView : MonoBehaviour
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

    private List<Tile> dynamicTiles = new List<Tile>();

    public void ShowStartMenu(bool show) { if (startMenuPanel != null) startMenuPanel.SetActive(show); }
    
    public void HideAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);
    }

    public bool IsAnyMenuOpen()
    {
        return (startMenuPanel != null && startMenuPanel.activeSelf) || 
               (gameOverPanel != null && gameOverPanel.activeSelf) ||
               (gameWinPanel != null && gameWinPanel.activeSelf) ||
               (settingsPanel != null && settingsPanel.activeSelf);
    }

    public void SetupGridVisuals(int boardSize)
    {
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
        }
    }

    public void RebuildTiles(int boardSize)
    {
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
    }

    public IEnumerator DelayedUIActivation(int score, int bestScore, int[,] grid, int boardSize)
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < dynamicTiles.Count; i++)
        {
            if (dynamicTiles[i] != null)
            {
                dynamicTiles[i].gameObject.SetActive(true);
            }
        }
        UpdateUI(score, bestScore, grid, boardSize);
    }

    public void UpdateUI(int score, int bestScore, int[,] grid, int boardSize)
    {
        if (scoreText != null) scoreText.text = score.ToString();
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();

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

    public void TriggerGameWin()
    {
        if (gameWinPanel == null) return;
        gameWinPanel.SetActive(true);
        CanvasGroup cg = gameWinPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, 0.5f).SetUpdate(true);
        }
    }

    public void TriggerGameOver()
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
}