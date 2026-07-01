using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GridView))]
public class GridController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveDuration = 0.15f;

    private GridView view;
    private GridModel model;
    
    private int boardSize = 4; 
    private bool isMoving = false;
    private bool hasWon = false;
    private WaitForSeconds waitMove;

    private void Awake()
    {
        view = GetComponent<GridView>();
        model = new GridModel(); 
        model.InitBestScore();   
        
        waitMove = new WaitForSeconds(moveDuration);
    }

    private void Start()
    {
        view.ShowStartMenu(true);
        view.HideAllPanels();
        SetupGridSystem(4);
    }

    private void SetupGridSystem(int size)
    {
        boardSize = size;
        model.Setup(boardSize);
        view.SetupGridVisuals(boardSize);
    }


    public void PlayGameFromMenu()
    {
        view.ShowStartMenu(false);
        RestartGame();
    }

    public void KeepPlaying()
    {
        view.HideAllPanels();
        int nextSize = boardSize + 1;
        if (nextSize > 8) nextSize = 4;
        
        SetupGridSystem(nextSize);
        RestartGame();
    }

    public void RestartGame()
    {
        hasWon = false;
        model.ResetScore();
        view.HideAllPanels();
        
        view.RebuildTiles(boardSize);
        
        model.Setup(boardSize); 
        model.SpawnTile();
        model.SpawnTile();

        StartCoroutine(view.DelayedUIActivation(model.CurrentScore, model.BestScore, model.Grid, boardSize));
        
        isMoving = false;
    }

    public void ToggleSettings(bool isOpen)
    {
        view.ToggleSettings(isOpen);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        Application.Quit(); 
#endif
    }

    private void Update()
    {
        if (isMoving || view.IsAnyMenuOpen()) return;

        int rot = -1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) rot = 0;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) rot = 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) rot = 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) rot = 3;

        if (rot != -1) StartCoroutine(MoveSequence(rot));
    }

    private IEnumerator MoveSequence(int rotation)
    {
        isMoving = true;
        model.RotateGrid(rotation);
        bool changed = false;

        for (int i = 0; i < boardSize; i++)
        {
            int[] row = new int[boardSize];
            for (int j = 0; j < boardSize; j++) row[j] = model.Grid[i, j];
            
            if (model.SlideAndMerge(row))
            {
                changed = true;
                for (int j = 0; j < boardSize; j++) model.Grid[i, j] = row[j];
            }
        }

        model.RotateGrid((4 - rotation) % 4);

        if (changed)
        {
            view.UpdateUI(model.CurrentScore, model.BestScore, model.Grid, boardSize);
            yield return waitMove;
            
            model.SpawnTile();
            view.UpdateUI(model.CurrentScore, model.BestScore, model.Grid, boardSize);
            
            if (!hasWon && model.CheckWinCondition())
            {
                hasWon = true;
                view.TriggerGameWin();
            }
            else if (model.CheckGameOver())
            {
                view.TriggerGameOver();
            }
        }

        isMoving = false;
    }
    public void ForceWin()
    {
        if (hasWon) return;

        hasWon = true;
        view.TriggerGameWin();
    }
}